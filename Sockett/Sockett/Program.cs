using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
//using System.Text.Json;
using System.Threading.Tasks;

class Servidor
{
    private static Dictionary<int, TimeSpan> cpuTimesAntigos = new Dictionary<int, TimeSpan>();
    private static DateTime ultimaAtualizacao = DateTime.Now;
    static async Task Main()
     {
        // Configura o servidor para escutar na porta 12345
        TcpListener server = new TcpListener(IPAddress.Any, 12345);
        server.Start();
        Console.WriteLine("Servidor iniciado na porta 12345. \n Escutando...");

        while (true)
        {
            // Aceita conexão do cliente
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = Task.Run(() => HandleClient(client)); // Processa cliente em uma nova tarefa
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        TimeSpan intervalo = DateTime.Now - ultimaAtualizacao;
        ultimaAtualizacao = DateTime.Now;

        try
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Lê o comando do cliente
            string comando = await reader.ReadLineAsync();
            if (comando == "listar processos")
            {
                // Obtém a lista de processos
                var processos = Process.GetProcesses()
                    .Select(p =>
                    {
                        double memoriaMB = 0;
                        int prioridade = 0;
                        double usoCpu = 0;
                        TimeSpan cpuAtual = TimeSpan.Zero;
                        try
                        {
                            memoriaMB = p.WorkingSet64 / (1024.0 * 1024.0);
                            prioridade = p.BasePriority;
                            cpuAtual = p.TotalProcessorTime;

                            if (cpuTimesAntigos.TryGetValue(p.Id, out TimeSpan cpuAntigo))
                            {
                                usoCpu = (cpuAtual - cpuAntigo).TotalMilliseconds / intervalo.TotalMilliseconds;
                                usoCpu = usoCpu * 100 / Environment.ProcessorCount;
                                
                            }
                            usoCpu = 1;
                            // Atualiza para próxima leitura
                            cpuTimesAntigos[p.Id] = cpuAtual;
                        }
                        catch
                        {

                        }
                        return new
                        {
                            ProcessName = p.ProcessName,
                            Id = p.Id,
                            BasePriority = prioridade,
                            Memoria = memoriaMB,
                            usoCpu = Math.Round(usoCpu, 2)
                        };
                    })
                    .ToList();

                // Serializa para JSON
                string jsonResponse = JsonConvert.SerializeObject(processos);

                // Envia a resposta
                await writer.WriteLineAsync(jsonResponse);
            }
            else
            {
                if (comando.StartsWith("buscar:"))
                {
                    string termoBusca = comando.Substring("buscar:".Length).Trim().ToLower();

                    TimeSpan inter = DateTime.Now - ultimaAtualizacao;
                    ultimaAtualizacao = DateTime.Now;

                    var processos = Process.GetProcesses()
                        .Where(p => p.ProcessName.ToLower().Contains(termoBusca))
                        .Select(p =>
                        {
                            double memoriaMB = 0;
                            int prioridade = 0;
                            double usoCpu = 0;
                            TimeSpan cpuAtual = TimeSpan.Zero;

                            try
                            {
                                memoriaMB = p.WorkingSet64 / (1024.0 * 1024.0);
                                prioridade = p.BasePriority;
                                cpuAtual = p.TotalProcessorTime;

                                if (cpuTimesAntigos.TryGetValue(p.Id, out TimeSpan cpuAntigo))
                                {
                                    usoCpu = (cpuAtual - cpuAntigo).TotalMilliseconds / inter.TotalMilliseconds;
                                    usoCpu = usoCpu * 100 / Environment.ProcessorCount;
                                }

                                cpuTimesAntigos[p.Id] = cpuAtual;
                            }
                            catch { }

                            return new
                            {
                                ProcessName = p.ProcessName,
                                Id = p.Id,
                                BasePriority = prioridade,
                                Memoria = memoriaMB,
                                Cpu = Math.Round(usoCpu, 2)
                            };
                        })
                        .ToList();

                    string jsonResponse = JsonConvert.SerializeObject(processos);
                    await writer.WriteLineAsync(jsonResponse);

                } else {
                    if (comando.StartsWith("set_priority:"))
                    {
                        try
                        {
                            string[] partes = comando.Split(':');
                            if (partes.Length == 3 &&
                                int.TryParse(partes[1], out int pid) &&
                                Enum.TryParse(partes[2], out ProcessPriorityClass novaPrioridade))
                            {
                                var processo = Process.GetProcessById(pid);
                                processo.PriorityClass = novaPrioridade;

                                await writer.WriteLineAsync("Prioridade Atualizada.");
                            }
                            else
                            {
                                await writer.WriteLineAsync("Erro: Comando invalido. (Lenght != 3)");
                            }
                        }
                        catch (Exception ex)
                        {
                            await writer.WriteLineAsync("erro: " + ex.Message);
                        }
                    } else {
                        if (comando.StartsWith("kill:"))
                        {
                            try
                            {
                                string[] partes = comando.Split(':');
                                if (partes.Length == 2 && int.TryParse(partes[1], out int pid))
                                {
                                    var processo = Process.GetProcessById(pid);
                                    processo.Kill();

                                    await writer.WriteLineAsync("Processo Finalizado.");
                                }
                                else
                                {
                                    await writer.WriteLineAsync("Erro: comando inválido");
                                }
                            }
                            catch (Exception ex)
                            {
                                await writer.WriteLineAsync("Erro: " + ex.Message);
                            }
                        }
                    }
                 }
                    //comand invalido
                }
            }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }   
        finally
        {
            client.Close();
        }
    }
}
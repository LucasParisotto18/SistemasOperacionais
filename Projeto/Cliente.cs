using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto
{
    internal class Cliente
    {
        public static async Task<List<ProcessoRemoto>> ObterProcessos(string ip)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ip, 12345); // porta do servidor remoto

                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync("listar processos");

                        string jsonResponse = await reader.ReadLineAsync();

                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            var processos = JsonConvert.DeserializeObject<List<ProcessoRemoto>>(jsonResponse);
                            return processos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar: " + ex.Message);
            }

            return new List<ProcessoRemoto>();
        }

        public static async Task<List<ProcessoRemoto>> BuscarProcessosPorNome(string ip, string termo)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ip, 12345);

                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync($"buscar:{termo}");

                        string jsonResponse = await reader.ReadLineAsync();

                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            var processos = JsonConvert.DeserializeObject<List<ProcessoRemoto>>(jsonResponse);
                            return processos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar: " + ex.Message);
            }

            return new List<ProcessoRemoto>();
        }

        public static async Task<string> AlterarPrioridade(string ip, int pid, string novaPrioridade)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ip, 12345);

                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string comando = $"set_priority:{pid}:{novaPrioridade}";
                        await writer.WriteLineAsync(comando);

                        string resposta = await reader.ReadLineAsync();
                        return resposta;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        public static async Task<string> EncerrarProcesso(string ip, int pid)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ip, 12345);

                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync($"kill:{pid}");

                        string resposta = await reader.ReadLineAsync();
                        return resposta;
                    }
                }
            }
            catch (Exception ex)
            {
                return "erro: " + ex.Message;
            }
        }

    }
}

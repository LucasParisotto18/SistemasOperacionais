using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto
{
    internal class AtualizadorRemoto
    {
        private readonly Timer timer;
        private readonly DataGridView grid;
        private string ip;
        private string filtro;

        public AtualizadorRemoto(DataGridView grid)
        {
            this.grid = grid;
            timer = new Timer();
            timer.Interval = 8000;
            timer.Tick += async (s, e) => await Atualizar();
        }

        public void Iniciar(string ip, string filtro = "")
        {
            this.ip = ip;
            this.filtro = filtro;

            if (!timer.Enabled)
                timer.Start();
        }

        public void Parar()
        {
            timer.Stop();
        }

        private async Task Atualizar()
        {
            if (string.IsNullOrWhiteSpace(ip)) return;

            List<ProcessoRemoto> lista;

            try
            {
                if (string.IsNullOrWhiteSpace(filtro))
                    lista = await Cliente.ObterProcessos(ip);
                else
                    lista = await Cliente.BuscarProcessosPorNome(ip, filtro);
                grid.Invoke(new MethodInvoker(() =>
                {
                    grid.Rows.Clear();
                    foreach (var p in lista)
                    {
                        grid.Rows.Add(
                            p.ProcessName,
                            p.Id,
                            p.Memoria.ToString("F2"),
                            p.usoCpu.ToString("F2"),
                            p.BasePriority
                        );
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar processos remotos: " + ex.Message);
            }
        }
    }
}


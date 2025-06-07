using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto
{
    internal class ProcessoRemoto
    {
        public string ProcessName { get; set; }
        public int Id { get; set; }
        public int BasePriority { get; set; }
        public double Memoria { get; set; } // em MB

        public double usoCpu { get; set; } // em porcentagem
    }
}

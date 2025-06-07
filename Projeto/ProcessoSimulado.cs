using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto
{

    internal class ProcessoSimulado
    {
        public string Nome { get; set; }
        public int TempoChegada { get; set; }// Em segundos ou ticks
        public int Duracao { get; set; }// Tempo de execução em segundos
        public int Prioridade { get; set; }// Quanto menor, maior a prioridade

        public ProcessoSimulado(string nome, int tempoChegada, int duracao, int prioridade)
        {
            Nome = nome;
            TempoChegada = tempoChegada;
            Duracao = duracao;
            Prioridade = prioridade;
            
        }


        public override string ToString()
        {
            return $"{Nome} (Chegada: {TempoChegada}'s, Duração: {Duracao}'s, Prioridade: {Prioridade})";
        }
    }
}

using Accord.Genetic;
using Accord.Math.Random;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;



namespace GeneticPetri.Genetic
{
    public class Algoritmo
    {

        /// <summary>
        /// Método de selecao
        /// </summary>
        public MetodoSelecao selectionMethod { get; set; }

        /// <summary>
        /// Tamanho da população
        /// </summary>
        public int populationSize { get; set; }

        /// <summary>
        /// Criterio de parada com iterações
        /// </summary>
        public int CritStopIteracoes { get; set; }

        /// <summary>
        /// Criterio de parada por variação de taxa
        /// </summary>
        public double CritStopTaxa { get; set; }

        public double TaxaCruzamento { get; set; }

        public double TaxaMutacao { get; set; }

        public int NumVariaveis { get; set; }


        public Algoritmo()
        {
 
            selectionMethod = MetodoSelecao.Roleta;
            populationSize = 100;
            CritStopIteracoes = 1000;
            CritStopTaxa = 0.1;
            TaxaCruzamento = 0.9;
            TaxaMutacao = 0.5;
            NumVariaveis = 40;//ver quantos precisa 6
        }
        public class Stopwatch { }

        public void Execute()
        {

            //Geradores aleatorios
            IRandomNumberGenerator<double> cromoGerador = new ZigguratUniformGenerator(0.0, 30.0, 14554);

            //Cromossomo modelo para a população
            PetriChromosome modeloCromossomo = new PetriChromosome(cromoGerador, cromoGerador, cromoGerador, NumVariaveis);

            //método de selecao dos cromossomos
            ISelectionMethod selecao;
            if (selectionMethod == MetodoSelecao.Elitista)
            {
                selecao = (ISelectionMethod)new EliteSelection();
            }
            else if (selectionMethod == MetodoSelecao.Ranking)
            {
                selecao = (ISelectionMethod)new RankSelection();
            }
            else if (selectionMethod == MetodoSelecao.Roleta)
            {
                selecao = (ISelectionMethod)new RouletteWheelSelection();
            }
            else
            {
                selecao = null;
            }

            //Tem que implementar a avaliaçao com rede de petri
            PetriFitness fitness = new PetriFitness();
            //Posicoes dos valores da equação
            fitness.Esqueleto = new int[,] { { 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },//1
                                             { 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0 },//2
                                             { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0 },//3
                                             { 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 },//4
                                             { 1, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0 },//5
                                             { 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0 },//6
                                             { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },//7
                                             { 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1 },//8
                                             { 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },//9
                                             { 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1 },//10
                                             { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 },//11
                                             { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 }};//12 (40 var)

            //Importancia das chances da equacao
            fitness.pesos = new double[] { 0.5, 1, -1, 0.5, 0.5, 0.5, 0.5, 1, -1, 0.5, 0.5, 0.5 };

            /*
            fitness.Esqueleto = new int[,] { { 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },//1
                                             { 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0 },//2
                                             { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0 },//3
                                             { 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 },//4
                                             { 1, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0 },//5
                                             { 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0 },//6
                                             { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },//7
                                             { 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 1, 1 },//8
                                             { 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },//9
                                             { 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1 },//10
                                             { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 },//11
                                             { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1 }};//12 (40 var)

            //Importancia das chances da equacao
            fitness.pesos = new double[] { 0.5, 1, -1, 0.5, 0.5, 0.5, 0.5, 1, -1, 0.5, 0.5, 0.5 };

            */

            // create population
            Population population = new Population(populationSize, modeloCromossomo, fitness, selecao)
            {
                MutationRate = TaxaMutacao,
                CrossoverRate = TaxaCruzamento
            };



            //iteração de cálculo
            for (int i = 0; i < CritStopIteracoes; i++)
            {
                //Executa
                population.RunEpoch();

                Console.Write("Iteração={0}", i + 1);
                Console.Write(" FitAvg={0}", population.FitnessAvg);
                Console.Write(" Best={0} Values={1} X={2}",
                    population.BestChromosome.Fitness,
                    string.Join(";", ((PetriChromosome)population.BestChromosome).Value),
                    string.Join(";", fitness.Resultado(population.BestChromosome)));
                Console.WriteLine();

                //double taxa = Math.Abs(FitAvg - population.FitnessAvg) / Math.Abs(population.FitnessAvg);
                //if (taxa < CritStopTaxa) break;
                //FitAvg = population.FitnessAvg;
            }

            Console.WriteLine("Best={0} Values={1} X={2}",
                population.BestChromosome.Fitness,
                string.Join(";", ((PetriChromosome)population.BestChromosome).Value),
                string.Join(";", fitness.Resultado(population.BestChromosome)));

            /*for (int i = 0; i < population.Size; i++)
            {
                Console.WriteLine("Chromo_{0:0000}={1} Values={2} X={3}",
                    i + 1,
                    population[i].Fitness,
                    string.Join(";", ((PetriChromosome)population[i]).Value),
                    string.Join(";", fitness.Resultado(population[i])));
            }
            */
        }



    }

    public enum MetodoSelecao
    {
        Elitista = 1,
        Ranking = 0,
        Roleta = 0
    }
}

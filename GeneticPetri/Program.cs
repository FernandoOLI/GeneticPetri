using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace GeneticPetri
{
    class Program
    {
        static void Main(string[] args)
        {
            //Inicia o cronometro
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Algoritmo

            Genetic.Algoritmo algoritmo = new Genetic.Algoritmo();

            algoritmo.Execute();

            //Para o cronometro
            stopWatch.Stop();
            // Recebe o tempo decorrido
            TimeSpan ts = stopWatch.Elapsed;
            // Formata o valor para ser lido
            string elapsedTime = String.Format("{0:00}:{1:00}",
                ts.Seconds, ts.Milliseconds / 10);
            //Tempo gasto para o programa rodar: elapsedTime
            Console.WriteLine(elapsedTime);




            Console.Read();
        }
    }
}

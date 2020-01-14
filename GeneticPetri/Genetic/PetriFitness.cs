using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Genetic;
using MathNet.Numerics.LinearAlgebra;

namespace GeneticPetri.Genetic
{
    public class PetriFitness : IFitnessFunction
    {
        /// <summary>
        /// Matriz que informa posicao do cromossomos
        /// Os cromossomos são valores '!=0' da matriz A sem diagonal
        /// </summary>
        public int[,] Esqueleto { get; set; }

        /// <summary>
        /// Pesos para as chances
        /// </summary>
        public double[] pesos { get; set; }

        //função de avaliação do cromossomo
        public double Evaluate(IChromosome chromosome)
        {
            double[] x = Resultado(chromosome);

            //Calculo da nota com pesos
            double nota = 0.0;
            for (int i = 0; i < pesos.Length; i++)
            {
                nota += pesos[i] * x[i];
            }

            //Nota do individuo
            return nota;
        }

        public double[] Resultado(IChromosome chromosome)
        {
            //Cromossomo a ser avaliado
            double[] valores = ((PetriChromosome)chromosome).Value;

            //Esqueleto
            //1 1 1 0
            //1 1 0 1
            //0 1 1 0
            //0 0 1 1
            //Fixa 1 1 1 1
            //Valores
            //4 2 0.1 10 3 2

            //Montando matrizes
            int tam = Esqueleto.GetLength(0);
            double[,] mA = new double[tam , tam];
            double[,] mA1 = new double[tam, tam];
            double[] vB = new double[tam];
            //Carregando valores do cromossomo
            int p = 0;
            for (int i = 0; i < tam; i++)
            {
                double s = 0.0;
                for (int j = 0; j < tam; j++)
                {
                    if (Esqueleto[i, j] == 1 && i != j)
                    {
                        mA[i, j] = valores[p++];
                        s += mA[i, j];
                    }
                    else
                    {
                        mA[i, j] = 0.0;
                    }
                }
                mA[i, i] = -s;
            }
            for (int j = 0; j < tam; j++)
            {
                mA[tam-1, j] = 1.0;
            }
            for (int i = 0; i < tam; i++)
            {
                vB[i] = 0.0;
            }
            vB[tam-1] = 1.0;

            //Transpor matrix
            for (int j = 0; j < tam; j++)
            {
                for (int i = 0; i < tam; i++)
                {
                    mA1[i, j] = mA[j, i];
                }
            }
            for (int j = 0; j < tam; j++)
            {
                mA1[tam - 1, j] = 1.0;
            }

            //Solver xA=b
            Matrix<double> A = Matrix<double>.Build.DenseOfArray(mA1);
            Vector<double> b = Vector<double>.Build.Dense(vB);
            Vector<double> x = A.Solve(b);

            return x.ToArray();
        }
    }
}

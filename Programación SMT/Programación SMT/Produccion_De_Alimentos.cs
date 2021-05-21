using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static string inputFilePath = "..\\..\\Input.txt";   // sustituir
        int numAceitesV;
        int numAceitesN;
        int numAceitesTotales;
        int numMeses;
        int VALOR;
        int MAXV;
        int MAXN;
        int[] MCAP;
        int[,] CA;
        int MinD;
        int MaxD;
        int[] toneladas;
        int MinB;
        int MaxDurezas;
        int[] durezas;
        int[,] precios;
        string Produces(int i, int j) { return "Produces_" + i.ToString() + "_" + j.ToString(); }
        string Compras(int i, int j) { return "Compras_" + i.ToString() + "_" + j.ToString(); }
        string AceiteDisponible(int i, int j) { return "AceiteDisponible_" + i.ToString() + "_" + j.ToString(); }

        public  void ProduccionDeAlimentos()
        {
            LeeProduccionDeAlimentos();
            List<string> lines = new List<string> {
                 ProduceModels(),
                SetLogic("QF_LRA")
            };

            // declaracion variables
            for (int i = 1; i <= numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(intvar(Produces(i, j)));
                    lines.Add(intvar(Compras(i, j)));
                    lines.Add(intvar(AceiteDisponible(i, j)));
                }
            }

            //constraint forall(m in 1..numMeses)
 //(sum(j in 1..numAceitesTotales)(durezas[j] * Produces[m, j]) <= MaxD * (sum(j in 1..numAceitesTotales)(Produces[m, j])) /\ sum(j in 1..numAceitesTotales)
      //(durezas[j] * Produces[m, j]) >= MinD * (sum(j in 1..numAceitesTotales)(Produces[m, j])));


            lines.Add(checksat());

            for (int i = 1; i <= numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(getvalue(Compras(i, j)));
                }
            }
            for (int i = 1; i <= numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(getvalue(Produces(i, j)));
                }
            }
            for (int i = 1; i <= numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(getvalue(AceiteDisponible(i, j)));
                }
            }

            RellenaFichero(lines, "ProduccionDeAlimentos.smt2");
        }
        void LeeProduccionDeAlimentos()
        {
            StreamReader file = new StreamReader(inputFilePath);
            numAceitesV = int.Parse(file.ReadLine());
            numAceitesN = int.Parse(file.ReadLine());
            numAceitesTotales = numAceitesV + numAceitesN;
            numMeses = int.Parse(file.ReadLine());
            VALOR = int.Parse(file.ReadLine());
            MAXV = int.Parse(file.ReadLine());
            MAXN = int.Parse(file.ReadLine());
            string buff = file.ReadLine();
            string[] split = buff.Split(' ');
            MCAP = new int[numAceitesTotales];
            for (int i = 0; i < numAceitesTotales; i++)
            {
                MCAP[i] = int.Parse(split[i]);
            }
            MaxDurezas = int.Parse(file.ReadLine());
            CA = new int[numMeses, numAceitesTotales];
            for (int i = 0; i < numMeses; i++)
            {
                buff = file.ReadLine();
                split = buff.Split(' ');
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    CA[i, j] = int.Parse(split[j]);
                }
            }
            MinD = int.Parse(file.ReadLine());
            MaxD = int.Parse(file.ReadLine());
            toneladas = new int[numAceitesTotales];
            buff = file.ReadLine();
            split = buff.Split(" ");
            for (int i = 0; i < numAceitesTotales; i++)
            {
                toneladas[i] = int.Parse(split[i]);
            }
            MinB = int.Parse(file.ReadLine());
            durezas = new int[numAceitesTotales];
            buff = file.ReadLine();
            split = buff.Split(" ");
            for (int i = 0; i < numAceitesTotales; i++)
            {
                durezas[i] = int.Parse(split[i]);
            }
            precios = new int[numMeses, numAceitesTotales];
            for (int i = 0; i < numMeses; i++)
            {
                buff = file.ReadLine();
                split = buff.Split(' ');
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    precios[i, j] = int.Parse(split[j]);
                }
            }
            file.Close();
        }
    }
}


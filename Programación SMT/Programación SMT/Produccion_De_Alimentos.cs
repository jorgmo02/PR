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
        string Produces() { return "Produces_"; }
        string Compras(int i, int j) { return Compras() + i.ToString() + "_" + j.ToString(); }
        string Compras() { return "Compras_"}
        string AceiteDisponible(int i, int j) { return AceiteDisponible() + i.ToString() + "_" + j.ToString(); }
        string AceiteDisponible() { return "AceiteDisponible_"; }

        public void ProduccionDeAlimentos()
        {
            LeeProduccionDeAlimentos();

            // asserts  -------------------------------------------------
            if (!(numAceitesV >= 0 &&
                numAceitesN >= 0 &&
                numMeses > 0 &&
                VALOR > 0 &&
                MAXV > 0 &&
                MAXN > 0 &&
                MinD > 0 &&
                MaxD > MinD &&
                MinB >= 0
            ))
                Console.WriteLine("ERROR: assertion failed. Check input.");

            foreach (int i in MCAP)
                if (!(i > 0))
                {
                    Console.WriteLine("ERROR: assertion failed. Check input at MCAP.");
                    return;
                }
            foreach (int i in ton)
                if (!(i > 0))
                {
                    Console.WriteLine("ERROR: assertion failed. Check input at ton.");
                    return;
                }
            foreach (int i in durezas)
                if (!(i > 0))
                {
                    Console.WriteLine("ERROR: assertion failed. Check input at dur.");
                    return;
                }
            foreach (int i in CA)
                if (!(i > 0))
                {
                    Console.WriteLine("ERROR: assertion failed. Check input at CA.");
                    return;
                }
            foreach (int i in precios)
                if (!(i > 0))
                {
                    Console.WriteLine("ERROR: assertion failed. Check input at precios.");
                    return;
                }
            // fin asserts  -------------------------------------------------            



            List<string> lines = new List<string> {
                 ProduceModels(),
                SetLogic("QF_LRA")
            };

            // declaracion variables
            for (int i = 1; i <= numMeses; i++)
            {
                for (int j = 1; j <= numAceitesTotales; j++)
                {
                    lines.Add(intvar(Produces(i, j)));
                    lines.Add(intvar(Compras(i, j)));
                    lines.Add(intvar(AceiteDisponible(i, j)));
                }
            }

            //constraint forall(m in 1..numMeses)
            //(sum(j in 1..numAceitesTotales)(durezas[j] * Produces[m, j]) <= MaxD * (sum(j in 1..numAceitesTotales)(Produces[m, j])) /\ sum(j in 1..numAceitesTotales)
            //(durezas[j] * Produces[m, j]) >= MinD * (sum(j in 1..numAceitesTotales)(Produces[m, j])));
            List<int> sumatorio = new List<int>();
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    sumatorio.Add(durezas[j]);

            addassert(addle(addsumOperacion(sumatorio, Produces(), "*", numMeses, numAceitesTotales),
                addmul(MaxD.ToString(), addSumVar(Produces(), numMeses, numAceitesTotales))));

            addassert(addge(addsumOperacion(sumatorio, Produces(), "*", numMeses, numAceitesTotales),
                addmul(MinD.ToString(), addSumVar(Produces(), numMeses, numAceitesTotales))));


            //constraint(sum(m in 1..numMeses, a in 1..numAceitesTotales)
            //((Produces[m, a] * VALOR) - (Compras[m, a] * precios[m, a] + CA[m, a] * AceiteDisponible[m, a]))) >= MinB;
            List<int> sumatorioValor = new List<int>();
            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; j <= numAceitesTotales; j++)
                    sumatorioValor.Add(VALOR);

            List<int> sumatorioPrecios = new List<int>();
            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; j <= numAceitesTotales; j++)
                    sumatorioPrecios.Add(precios[i, j]);

            List<int> sumatorioCA = new List<int>();
            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; j <= numAceitesTotales; j++)
                    sumatorioCA.Add(CA[i, j]);

            addassert(addge(addminus(addsumOperacion(sumatorioValor, Produces(), "*", numMeses, numAceitesTotales),
                addplus(addsumOperacion(sumatorioPrecios, Compras(), "*", numMeses, numAceitesTotales),
                addsumOperacion(sumatorioCA, AceiteDisponible(), "*", numMeses, numAceitesTotales))), MinB.ToString()));


            // % 3. Al final del año tienen que quedar "toneladas" toneladas de aceite
            // constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[numMeses,a] = toneladas[a]);
            for (int i = 1; i <= numAceitesTotales; i++)
            {
                addassert(
                    addeq(AceiteDisponible(numMeses, i).ToString(), toneladas[i - 1].ToString())
                );
            }

            // % 4. cada mes nos queda lo que compramos menos lo que producimos más lo que teníamos almacenado anteriormente
            // constraint forall(m in 2..numMeses,a in 1..numAceitesTotales) (AceiteDisponible[m,a]=Compras[m,a]-Produces[m,a] +AceiteDisponible[m-1,a]);
            for (int i = 2; i <= numMeses; i++)
            {
                for (int j = 1; j <= numAceitesTotales; j++)
                {
                    addassert(
                        addeq(
                            AceiteDisponible(i, j).ToString(),
                            addplus(
                                addminus(Compras(i, j).ToString(), Produces(i, j).ToString()),
                                AceiteDisponible(i - 1, j)
                                )
                            )
                    );
                }
            }
            // %El primer mes no contamos con lo que teníamos almacenado anteriormente
            //constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[1,a]=Compras[1,a]-Produces[1,a]);
            for (int i = 1; i <= numAceitesTotales; i++)
            {
                addassert(addeq(AceiteDisponible(1, i), addminus(Compras(1, i), Produces(1, i))));
            }

            //% 5.cada mes produces como mucho MAXV aceites vegetales
            //constraint forall(m in 1..numMeses)(
            //sum(a in 1..numAceitesV)(Produces[m, a]) <= MAXV
            //);

            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; j <= numAceitesV; j++)
                    addassert(addle(addSumVar(Produces(), numMeses, numAceitesV), MAXV.ToString()));

            // % 6. cada mes produces como mucho MAXN aceites animales
            // constraint forall(m in 1..numMeses)(
            //     sum(a in numAceitesV..numAceitesV + numAceitesN)(Produces[m,a])  <= MAXN
            // );
            string myLine = "( + ";
            for (int i = 1; i <= numMeses; i++)
            {
                for (int j = numAceitesV; j <= numAceitesV + numAceitesN; j++)
                    myLine += Produces(i, j) + " ";
            }
            myLine += ")";
            lines.Add(addle(myLine, MAXN.ToString()));

            // % 7. En ningún mes se supera el almacenamiento disponible para cada aceite
            // constraint forall(m in 1..numMeses,a in 1..numAceitesTotales)((AceiteDisponible[m,a])<=MCAP[a]);
            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; j <= numAceitesTotales; j++)
                    addassert(addle(AceiteDisponible(i, j), MCAP[j - 1].ToString()));

            // % 8.Las compras tienen que ser positivas o 0
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(Compras[m, a] >= 0);
            for (int i = 1; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    addassert(addge(Compras(i, j), "0"));

            // %9.Los aceites disponibles tienen que ser positivas o 0
            // constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(AceiteDisponible[m,a]>=0);
            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; j <= numAceitesTotales; j++)
                    addassert(addge(AceiteDisponible(i, j), "0"));

            //% 10.La producción tiene que ser positiva o 0
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(Produces[m, a] >= 0);
            for (int i = 1; i <= numMeses; i++)
                for (int j = 1; i <= numAceitesTotales; i++)
                    addassert(addge(Produces(i, j), "0"));


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



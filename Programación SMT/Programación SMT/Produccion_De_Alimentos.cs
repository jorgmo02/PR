using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static string inputFilePath = "PruebaConstraint12.txt";   // sustituir
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
        int[] durezas;
        int[,] precios;
        int[,] LimitesAceites;
        int[] toneladasMinimas;
        int K;

        string Produces(int i, int j) { return "Produces_" + i.ToString() + "_" + j.ToString(); }
        string Produces() { return "Produces_"; }
        string Compras(int i, int j) { return Compras() + i.ToString() + "_" + j.ToString(); }
        string Compras() { return "Compras_"; }
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
            foreach (int i in toneladas)
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
                SetLogic("QF_LIA"),
                ProduceModels()
            };

            // declaracion variables
            for (int i = 0; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(intvar(Produces(i + 1, j + 1)));
                    lines.Add(intvar(Compras(i + 1, j + 1)));
                    lines.Add(intvar(AceiteDisponible(i + 1, j + 1)));
                }
            }
            lines.Add(intvar("Beneficio"));

            lines.Add(addComent("0. Acotamos produces para ahorrarle trabajo al resolutor"));
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)
            //(Produces[m, a] < MAXV + MAXN);
            for (int i = 0; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(addassert(addlt(Produces(i + 1, j + 1), (MAXV + MAXN).ToString())));
                }
            }

            lines.Add(addComent("1. El producto está dentro de su dureza"));
            //constraint forall(m in 1..numMeses)
            //(sum(j in 1..numAceitesTotales)(durezas[j] * Produces[m, j]) <= MaxD * (sum(j in 1..numAceitesTotales)(Produces[m, j])) /\ sum(j in 1..numAceitesTotales)
            //(durezas[j] * Produces[m, j]) >= MinD * (sum(j in 1..numAceitesTotales)(Produces[m, j])));

            List<int> sumatorio = new List<int>();
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    sumatorio.Add(durezas[j]);

            lines.Add(addComent("   <= dureza maxima"));
            lines.Add(addassert(addle(addsumOperacion(sumatorio, Produces(), "*", 0, 0, numMeses, numAceitesTotales),
                addmul(MaxD.ToString(), addSumVar(Produces(), numMeses, numAceitesTotales)))));

            sumatorio.Clear();
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    sumatorio.Add(durezas[j]);

            lines.Add(addComent("   >= dureza minima"));
            lines.Add(addassert(addge(addsumOperacion(sumatorio, Produces(), "*", 0, 0, numMeses, numAceitesTotales),
                addmul(MinD.ToString(), addSumVar(Produces(), numMeses, numAceitesTotales)))));


            lines.Add(addComent("2. Se consigue el beneficio mínimo"));
            //constraint(sum(m in 1..numMeses, a in 1..numAceitesTotales)
            //((Produces[m, a] * VALOR) - (Compras[m, a] * precios[m, a] + CA[m, a] * AceiteDisponible[m, a]))) >= MinB;
            List<int> sumatorioValor = new List<int>();
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    sumatorioValor.Add(VALOR);

            List<int> sumatorioPrecios = new List<int>();
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    sumatorioPrecios.Add(precios[i, j]);

            List<int> sumatorioCA = new List<int>();
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    sumatorioCA.Add(CA[i, j]);

            string beneficioAux = addminus(
                addsumOperacion(sumatorioValor, Produces(), "*", 0, 0, numMeses, numAceitesTotales),
                addplus(
                    addsumOperacion(sumatorioPrecios, Compras(), "*", 0, 0, numMeses, numAceitesTotales),
                    addsumOperacion(sumatorioCA, AceiteDisponible(), "*", 0, 0, numMeses, numAceitesTotales)
                ));

            lines.Add(addassert(addge(beneficioAux, MinB.ToString())));


            lines.Add(addComent("3. Al final del año tienen que quedar \"toneladas\" toneladas de aceite"));
            // constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[numMeses,a] = toneladas[a]);
            for (int i = 0; i < numAceitesTotales; i++)
            {
                lines.Add(addassert(
                    addeq(AceiteDisponible(numMeses, i + 1).ToString(), toneladas[i].ToString())
                ));
            }

            lines.Add(addComent("4. cada mes nos queda lo que compramos menos lo que producimos más lo que teníamos almacenado anteriormente"));
            // constraint forall(m in 2..numMeses,a in 1..numAceitesTotales) (AceiteDisponible[m,a]=Compras[m,a]-Produces[m,a] +AceiteDisponible[m-1,a]);
            for (int i = 1; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(addassert(
                        addeq(
                            AceiteDisponible(i + 1, j + 1).ToString(),
                            addplus(
                                addminus(Compras(i + 1, j + 1).ToString(), Produces(i + 1, j + 1).ToString()),
                                AceiteDisponible(i, j + 1)
                                )
                            )
                        )
                    );
                }
            }
            lines.Add(addComent("   El primer mes no contamos con lo que teníamos almacenado anteriormente"));
            //constraint forall(a in 1..numAceitesTotales) (AceiteDisponible[1,a]=Compras[1,a]-Produces[1,a]);
            for (int i = 0; i < numAceitesTotales; i++)
            {
                lines.Add(addassert(addeq(AceiteDisponible(1, i + 1), addminus(Compras(1, i + 1), Produces(1, i + 1)))));
            }

            lines.Add(addComent("5. Cada mes produces como mucho MAXV aceites vegetales"));
            //constraint forall(m in 1..numMeses)(
            //sum(a in 1..numAceitesV)(Produces[m, a]) <= MAXV
            //);
            for (int i = 0; i < numMeses; i++)
            {
                lines.Add(addassert(addle(addSumVar(Produces() + (i + 1).ToString() + "_", numAceitesV - 1), MAXV.ToString())));
            }

            lines.Add(addComent("6. Cada mes produces como mucho MAXN aceites animales"));
            // constraint forall(m in 1..numMeses)(
            //     sum(a in numAceitesV..numAceitesV + numAceitesN)(Produces[m,a])  <= MAXN
            // );
            //            lines.Add(addassert(addle(addSumVarInicio(Produces(), numMeses, numAceitesN + numAceitesV, 0, numAceitesV), MAXN.ToString())));
            for (int i = 0; i < numMeses; i++)
            {
                lines.Add(addassert(addle(addSumVarInicio(Produces() + (i + 1).ToString() + "_", numAceitesN + numAceitesV - 1, numAceitesV - 1), MAXN.ToString())));
            }

            lines.Add(addComent("7. En ningún mes se supera el almacenamiento disponible para cada aceite"));
            //// constraint forall(m in 1..numMeses,a in 1..numAceitesTotales)((AceiteDisponible[m,a])<=MCAP[a]);
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    lines.Add(addassert(addle(AceiteDisponible(i + 1, j + 1), MCAP[j].ToString())));

            lines.Add(addComent("8. Las compras tienen que ser positivas o 0"));
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(Compras[m, a] >= 0);
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    lines.Add(addassert(addge(Compras(i + 1, j + 1), "0")));

            lines.Add(addComent("9. Los aceites disponibles tienen que ser positivas o 0"));
            // constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(AceiteDisponible[m,a]>=0);
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    lines.Add(addassert(addge(AceiteDisponible(i + 1, j + 1), "0")));

            lines.Add(addComent("10. La producción tiene que ser positiva o 0"));
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)(Produces[m, a] >= 0);
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    lines.Add(addassert(addge(Produces(i + 1, j + 1), "0")));

            lines.Add(addComent("11. Cada mes solo se podrán usar unos determinados aceites"));
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales )
            //(LimitesAceites[m, a] = 1->Produces[m, a] = 0);
            for (int i = 0; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    if (LimitesAceites[i, j] == 1)
                        lines.Add(addassert(addeq(Produces(i + 1, j + 1), "0")));
                }
            }
            //% constraint forall(m in 1..numMeses)(
            // % sum(a in 1..numAceitesTotales)(bool2int(Produces[m, a] > 0)) <= K
            // %);
            for (int i = 0; i < numMeses; i++)
            {
                List<int> ceros = new List<int>();
                for (int j = 0; j < numAceitesTotales; j++)
                    ceros.Add(0);
                lines.Add(addassert(addle(addSumBool2IntVar(ceros, Produces() + (i + 1).ToString() + "_", ">", numAceitesTotales), K.ToString())));
            }

            lines.Add(addComent("12. Si se usa un aceite hay que usar al menos X toneladas"));
            //constraint forall(m in 1..numMeses, a in 1..numAceitesTotales)
            //(Produces[m, a] == 0 \/ Produces[m, a] >= toneladasMinimas[a]);
            for (int i = 0; i < numMeses; i++)
                for (int j = 0; j < numAceitesTotales; j++)
                    lines.Add(addassert(
                        addor(
                            addeq(Produces(i + 1, j + 1), "0"),
                            addge(Produces(i + 1, j + 1), toneladasMinimas[j].ToString())
                            )
                        )
                    );


            lines.Add(addComent("13. Si usamos VEG1 o VEG2 hay que usar ANV3"));
            // constraint forall(m in 1..numMeses)
            // ((Produces[m, 1] + Produces[m, 2]) > 0->Produces[m, 5] > 0);
            for (int i = 0; i < numMeses; i++)
                lines.Add(addassert(addimply(addgt(addplus(Produces(i + 1, 1), Produces(i + 1, 2)), "0"), addgt(Produces(i + 1, 5), "0"))));


            
            lines.Add(addComent("Maximizar las ganancias"));

            lines.Add(addassert(addeq("Beneficio", beneficioAux)));

            lines.Add(addMaximize(beneficioAux));

            lines.Add(checksat());

            for (int i = 0; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(getvalue(Compras(i + 1, j + 1)));
                }
            }
            for (int i = 0; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(getvalue(Produces(i + 1, j + 1)));
                }
            }
            for (int i = 0; i < numMeses; i++)
            {
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    lines.Add(getvalue(AceiteDisponible(i + 1, j + 1)));
                }
            }
            lines.Add(getvalue("Beneficio"));
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
            durezas = new int[numAceitesTotales];
            buff = file.ReadLine();
            split = buff.Split(" ");
            for (int i = 0; i < numAceitesTotales; i++)
            {
                durezas[i] = int.Parse(split[i]);
            }
            toneladas = new int[numAceitesTotales];
            buff = file.ReadLine();
            split = buff.Split(" ");
            for (int i = 0; i < numAceitesTotales; i++)
            {
                toneladas[i] = int.Parse(split[i]);
            }
            MinB = int.Parse(file.ReadLine());
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

            LimitesAceites = new int[numMeses, numAceitesTotales];
            for (int i = 0; i < numMeses; i++)
            {
                buff = file.ReadLine();
                split = buff.Split(' ');
                for (int j = 0; j < numAceitesTotales; j++)
                {
                    LimitesAceites[i, j] = int.Parse(split[j]);
                }
            }

            toneladasMinimas = new int[numAceitesTotales];
            buff = file.ReadLine();
            split = buff.Split(' ');
            for (int i = 0; i < numAceitesTotales; i++)
            {
                toneladasMinimas[i] = int.Parse(split[i]);
            }
            K = int.Parse(file.ReadLine());
            file.Close();
        }
    }
}


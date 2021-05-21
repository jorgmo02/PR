using System;
using System.Collections.Generic;
using System.IO;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static string inputFilePath = "..\\..\\escuchandoMusicaInput1.txt";   // sustituir

        static int min;
        static int n;
        static int t1;
        static int t2;
        static int[] duraciones;
        static int[] puntuaciones;

        static void EscuchandoMusica(string[] args)
        {
            readEscuchandoMusica();

            List<string> lines = new List<string> {
                ProduceModels(),
                SetLogic("QF_LIA")
            };

            // declaracion variables
            for (int i = 1; i <= n; i++)
                lines.Add(intvar(Cancion(i)));

            // constraint de dominio
            for (int i = 1; i <= n; i++) {
                lines.Add(addassert(addimply(addnot(addor(addeq(Cancion(i), "1"), addeq(Cancion(i), "0"))), addeq(Cancion(i), "2"))));
            }

            //% constraint la cancion acaba justo al llegar al destino
            //constraint sum(i in 1..n where canciones[i] = 1) (duraciones[i]) = t1;
            //constraint sum(i in 1..n where canciones[i] = 2) (duraciones[i]) = t2;
            lines.Add(addassert(addeq(SumatorioDuracion("1", n-1), t1.ToString())));
            lines.Add(addassert(addeq(SumatorioDuracion("2", n-1), t2.ToString())));

            // constraint >= minimo puntuacion
            lines.Add(addassert(addle(min.ToString(), SumatorioPuntuacion("0", n - 1))));

            lines.Add(checksat());

            for(int i = 1; i <= n; i++) {
                lines.Add(getvalue(Cancion(i)));
            }

            RellenaFichero(lines, "EscuchandoMusica.smt2");
        }
        static string Cancion(int i) {
            return "canciones_" + i.ToString();
        }

        static void readEscuchandoMusica()
        {
            string[] lines = File.ReadAllLines(inputFilePath);
            
            n = Convert.ToInt32(lines[0]);
            t1 = Convert.ToInt32(lines[1]);
            t2 = Convert.ToInt32(lines[2]);

            duraciones = new int[n];
            puntuaciones = new int[n];

            string[] durs = lines[3].Split(" ");
            for (int i = 0; i < durs.Length; i++)
                duraciones[i] = Convert.ToInt32(durs[i]);

            string[] punts = lines[4].Split(" ");
            for (int i = 0; i < punts.Length; i++)
                puntuaciones[i] = Convert.ToInt32(punts[i]);

            min = Convert.ToInt32(lines[5]);
        }
        static string SumatorioPuntuacion(string eq, int count)
        {
            if (count == 0) return "0";
            if (count == 1) return puntuaciones[count].ToString();
            int x = puntuaciones[count];

            return "(+ " +
                addmul(
                    x.ToString(),
                    bool2int(addnot(addeq(Cancion(count), eq))))
                + " " +
                SumatorioPuntuacion(eq, count - 1) + " )";
        }

        static string SumatorioDuracion(string eq, int count)
        {
            if (count == 0) return "0";
            if (count == 1) return duraciones[count].ToString();
            int x = duraciones[count];

            return "(+ " +
                addmul(
                    x.ToString(),
                    bool2int(addnot(addeq(Cancion(count), eq))))
                + " " +
                SumatorioDuracion(eq, count-1) + " )";
        }
    }
}

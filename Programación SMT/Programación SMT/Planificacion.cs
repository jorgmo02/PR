using System;
using System.Collections.Generic;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static void Planificacion(string[] args)
        {
            int T = Convert.ToInt32(Console.ReadLine());
            int D = Convert.ToInt32(Console.ReadLine());
            int Lim = Convert.ToInt32(Console.ReadLine());

            int[] duracion = new int[T];
            for (int i = 0; i < T; i++)
            {
                duracion[i] = Convert.ToInt32(Console.Read());
                if (duracion[i] > D)
                    throw new Exception("Error: la duración de una de las tareas es mayor que D");
                if (duracion[i] > Lim)
                    throw new Exception("Error: la duración de una de las tareas es mayor que el límite de tiempo");
            }

            int[][] dep = new int[T][];
            for (int i = 0; i < T; i++)
            {
                dep[i] = new int[T];
                for (int j = 0; j < T; j++)
                {
                    dep[i][j] = Convert.ToInt32(Console.Read());
                }

                if (dep[i][i] == 1)
                    throw new Exception("Error: una tarea no puede depender de sí misma");
            }

            for (int i = 0; i < T; i++)
                for (int j = 0; j < T; j++)
                    if (dep[i][j] == 1 && dep[j][i] == 1)
                        throw new Exception("Error: dos tareas no pueden depender entre sí");


            List<string> lines = new List<string>();

            lines.Add(ProduceModels());
            lines.Add("QF_LIA");

            // declaracion variables
            for (int i = 1; i <= T; i++)
                lines.Add(intvar(Tarea(i)));

            for (int i = 1; i <= T; i++)
                for (int j = 1; j <= T; j++)
                    lines.Add(intvar(Dep(i, j)));


            //% no se hace una que depende de otra antes que la otra se haya hecho
            //constraint forall(t in 1..T) (
            //  forall(i in 1..T where dep[t, i] != 0) (
            //    asig[i] + duracion[i] <= asig[t]
            //));
            for (int t = 1; t <= T; t++)
            {
                for (int i = 1; i <= T; i++)
                {
                    lines.Add(bool2int(addnot(addeq(Dep(t, i), "0"))));
                }
            }


            //constraint forall(t in 1..T) (
            //  asig[t] + duracion[t] <= Lim
            //);
            for(int t = 1; t <= T; t++)
            {
                //lines.Add(SMT_Translator.addsum(...))
            }


            RellenaFichero(lines, "Planificacion");
        }

        static string Tarea(int i) { return "tarea_" + i.ToString(); }
        static string Dep(int i, int j) { return "dependencias_" + i.ToString() + "_" + j.ToString(); }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static void Main(string[] args)
        {
            SMTProgram p;
            if (args.Length == 1)
                p = new SMTProgram(args[0]);
            else p = new SMTProgram(args[0], args[1]);
            p.ProduccionDeAlimentos();
        }
    }
}

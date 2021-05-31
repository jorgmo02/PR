using System;
using System.Collections.Generic;
using System.Text;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static void Main(string[] args)
        {
            SMTProgram p = new SMTProgram(args[0]);
            p.ProduccionDeAlimentos();
        }
    }
}

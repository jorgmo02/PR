using System.IO;
using System;
using System.Collections.Generic;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        static string filesPath = "D:\\Archivos\\Universidad\\3V\\PR\\SMTFiles\\";   // sustituir
        static void RellenaFichero(List<string> content, string path)
        {
            try {
                File.WriteAllLines(filesPath + path, content);
            }
            catch {
                Console.WriteLine("ERROR: No se pudo rellenar el fichero.");
            }
        }

        static string ProduceModels() { return "(set-option :produce-models true)"; }
        static string SetLogic(string l) { return "(set-logic " + l + ")"; }
        static string intvar(string v) { return "(declare-fun " + v + " () Int)"; }
        static string intvar(int v) { return "(declare-fun " + v.ToString() + " () Int)"; }
        static string bool2int(string b) { return "(ite " + b + " 1 0)"; }
        static string addand(string a1, string a2) { return "(and " + a1 + " " + a2 + " )"; }
        static string addor(string a1, string a2) { return "(or " + a1 + " " + a2 + " )"; }
        static string addnot(string a) { return "(not " + a + " )"; }
        static string addexists(string a)
        {
            if (a.Length == 0) return "false";
            if (a.Length == 1) return a[0].ToString();
            char x = a[0];
            return "(or " + x + " " + addexists(a.Substring(1)) + " )";
        }
        static string addeq(string a1, string a2) { return "(= " + a1 + " " + a2 + " )"; }
        static string addle(string a1, string a2) { return "(<= " + a1 + " " + a2 + " )"; }
        static string addge(string a1, string a2) { return "(>= " + a1 + " " + a2 + " )"; }
        static string addlt(string a1, string a2) { return "(< " + a1 + " " + a2 + " )"; }
        static string addgt(string a1, string a2) { return "(> " + a1 + " " + a2 + " )"; }
        static string addplus(string a1, string a2) { return "(+ " + a1 + " " + a2 + " )"; }
        static string addimply(string a1, string a2) { return "(=> " + a1 + " " + a2 + " )"; }
        static string addmul(string a1, string a2) { return "(* " + a1 + " " + a2 + " )"; }
        static string addassert(string a) { return "(assert " + a + " )"; }
        static string addsum(List<int> a)
        {
            if (a.Count == 0) return "0";
            if (a.Count == 1) return a[0].ToString();
            int x = a[a.Count-1];
            a.RemoveAt(a.Count - 1);

            return "(+ " + x + " " + addsum(a) + " )";
        }
        static string checksat() { return "(check-sat)"; }
        static string getModel() { return "(get-model)"; }
        static string getvalue(string l) { return "(get-value " + l + " )"; }
    }
}

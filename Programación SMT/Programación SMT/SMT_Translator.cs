using System.IO;
using System;
using System.Collections.Generic;

namespace Programacion_SMT
{
    partial class SMTProgram
    {
        public SMTProgram(string path) { inputFilePath = "..\\..\\" + path; }
        static string filesPath = "..\\..\\";   // sustituir
        static void RellenaFichero(List<string> content, string path)
        {
            try
            {
                File.WriteAllLines(filesPath + path, content);
            }
            catch
            {
                Console.WriteLine("ERROR: No se pudo rellenar el fichero.");
            }
        }

        static string GetObjectives() { return "(get-objectives)"; }
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
        static string addminus(string a1, string a2) { return "(- " + a1 + " " + a2 + " )"; }
        static string addimply(string a1, string a2) { return "(=> " + a1 + " " + a2 + " )"; }
        static string addmul(string a1, string a2) { return "(* " + a1 + " " + a2 + " )"; }
        static string addassert(string a) { return "(assert " + a + " )"; }
        static string addMaximize(string a) { return "(maximize " + a + ")"; }
        static string addMinimize(string a) { return "(minimize " + a + ")"; }

        static string addsum(List<int> a)
        {
            if (a.Count == 0) return "0";
            if (a.Count == 1) return a[0].ToString();
            int x = a[a.Count - 1];
            a.RemoveAt(a.Count - 1);

            return "(+ " + x + " " + addsum(a) + " )";
        }


        public static string addSumVarInicio(string variable, int i, int j, int inicioi, int inicioj)
        {
            return _addSumVarInicio(variable, inicioi, inicioj, i - 1, j - 1, j - 1);
        }


        private static string _addSumVarInicio(string variable, int inicioi, int inicioj, int i, int j, int initJ)
        {
            if (i == inicioi && j == inicioj) return variable + (i + 1).ToString() + "_" + (j + 1).ToString();
            if (j == inicioj)
                return "(+ " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + _addSumVarInicio(variable, inicioi, inicioj, i - 1, initJ, initJ) + ")";
            return "(+ " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + _addSumVarInicio(variable, inicioi, inicioj, i, j - 1, initJ) + ")";
        }

        public static string addSumVarInicio(string variable, int i, int inicioi)
        {
            if (i == inicioi) return "0";
            if (i == inicioi + 1) return variable + (i + 1).ToString();
            return "(+ " + variable + (i + 1).ToString() + " " + addSumVarInicio(variable, i - 1, inicioi) + ")";
        }


        public static string addSumVar(string variable, int i, int j)
        {
            return _addSumVar(variable, i - 1, j - 1, j - 1);
        }
        private static string _addSumVar(string variable, int i, int j, int initJ)
        {
            if (i == 0 && j == 0) return variable + (i + 1).ToString() + "_" + (j + 1).ToString();
            if (j == 0)
                return "(+ " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + _addSumVar(variable, i - 1, initJ, initJ) + ")";
            return "(+ " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + _addSumVar(variable, i, j - 1, initJ) + ")";
        }

        public static string addSumVar(string variable, int i)
        {
            if (i == 0) return variable + (i + 1).ToString();
            //if (i == 1) ;
            return "(+ " + variable + (i + 1).ToString() + " " + addSumVar(variable, i - 1) + ")";
        }

        public static string addsumOperacion(List<int> valor, string variable, string operacion, int i, int j, int SizeI, int SizeJ)
        {
            if (valor.Count == 0) return "0";
            if (valor.Count == 1) return "( " + operacion + " " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + valor[0].ToString() + ")";
            return "(+ " + __addsumOperacion(valor, variable, operacion, i, j, SizeI, SizeJ) + ")";
        }

        public static string addSumBool2IntVar(string variable, string comparison, int i)
        {
            if (i == 0) return bool2int(variable + (i + 1).ToString() + " " + comparison);
            return "(+ " + bool2int(variable + (i + 1).ToString() + " " + comparison) + " " + addSumBool2IntVar(variable, comparison, i-1) + ")";
        }

        public static string addSumBool2IntVar(string variable, string comparison, int i, int j, int initJ)
        {
            if (i == 0 && j == 0) return bool2int(variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + comparison);
            int auxI = i, auxJ = j - 1;
            if (j == 0)
            {
                auxI = i - 1;
                auxJ = initJ;
            }
            return "(+ " +
                bool2int(variable + (i + 1).ToString() + "_" + (j + 1).ToString() + " " + comparison) + " " +
                addSumBool2IntVar(variable, comparison, auxI, auxJ, initJ) +
                ")";
        }

        // (+ 0 (+ (* 3 variable) 0)))
        // (+ (* 4 variable) (+ (* 3 variable) 0)))

        public static string __addsumOperacion(List<int> valor, string variable, string operacion, int i, int j, int SizeI, int SizeJ)
        {
            if (valor.Count == 0) return "0";
            if (valor.Count == 1) return valor[0].ToString();
            int x = valor[0];
            valor.RemoveAt(0);
            if (j == SizeJ - 1) return "(" + operacion + " " + x + " " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + ") " + addsumOperacion(valor, variable, operacion, i + 1, 0, SizeI, SizeJ);
            return "(" + operacion + " " + x + " " + variable + (i + 1).ToString() + "_" + (j + 1).ToString() + ") " + addsumOperacion(valor, variable, operacion, i, j + 1, SizeI, SizeJ);
        }
        static string addsumOperacion(List<int> valor, string variable, string operacion, int i)
        {
            if (i == 0) return "0";
            if (i == 1) return valor[0].ToString();
            int x = valor[valor.Count - 1];
            valor.RemoveAt(valor.Count - 1);
            return "(+ " + addsumOperacion(valor, variable, operacion, i - 1) + " (" + operacion + " " + x + " " + variable + (i + 1).ToString() + " )";
        }
        static string checksat() { return "(check-sat)"; }
        static string getModel() { return "(get-model)"; }
        static string getvalue(string l) { return "(get-value (" + l + ") )"; }
        static string addComent(string comment) { return "; " + comment; }
    }
}

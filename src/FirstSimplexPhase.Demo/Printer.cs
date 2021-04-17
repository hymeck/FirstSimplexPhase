using System.Collections.Generic;
using FirstSimplexPhase.Library;
using static System.Console;

namespace FirstSimplexPhase.Demo
{
    public static class Printer
    {
        public static void PrintFeasibleSolution(IEnumerable<double> feasibleSolution) =>
            WriteLine($"[{string.Join("; ", feasibleSolution)}]");

        public static void PrintBasisIndices(IEnumerable<int> basisIndices) =>
            WriteLine($"({string.Join(", ", basisIndices)})");
        
        public static void PrintResult((Status, FirstSimplexResult) output)
        {
            var (status, result) = output;
            if (status == Status.Success)
            {
                WriteLine("Feasiable solution (x):");
                PrintFeasibleSolution(result.FeasibleSolution);
                WriteLine("Basis indices (J_b):");
                PrintBasisIndices(result.BasisIndices);
            }
            else
                WriteLine(status.ToString());
        }
    }
}

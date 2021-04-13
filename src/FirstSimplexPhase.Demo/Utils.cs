using System.Collections.Immutable;
using System.Linq;
using FirstSimplexPhase.Library;
using static System.Console;

namespace FirstSimplexPhase.Demo
{
    public static class Utils
    {
        public static void PrintFeasibleSolution(ImmutableArray<double> feasibleSolution) =>
            WriteLine($"[{string.Join("; ", feasibleSolution.AsEnumerable())}]");

        public static void PrintBasisIndices(ImmutableSortedSet<int> basisIndices) =>
            WriteLine($"({string.Join(", ", basisIndices.AsEnumerable())})");
        
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

using System.Collections.Generic;
using FirstSimplexPhase.Library;
using static FirstSimplexPhase.Demo.Printer;

namespace FirstSimplexPhase.Demo
{
    public static class Executor
    {
        private static void Perform(double[,] conditions, double[] constraints) =>
            PrintResult(SimplexFirstPhaseService.Solve(conditions, constraints));

        private static readonly Dictionary<int, (double[,], double[])> VariantInputs =
            new()
            {
                {
                    12, (new double[,] {{-1, 1, 1, 0}, {4, 5, 0, 1}, {10, 17, 2, 3}}, 
                        new double[] {2, 28, 88})
                },
                {
                    11, (new double[,] {{-1, 3, 1, 0}, {4, 3, 0, 1}, {5, 0, -1, 1}}, 
                        new double[] {9, 24, 15})
                },
                
            };

        public static void PerformVariant(int variant)
        {
            var (conditions, constraints) = VariantInputs[variant];
            Perform(conditions, constraints);
        }
    }
}

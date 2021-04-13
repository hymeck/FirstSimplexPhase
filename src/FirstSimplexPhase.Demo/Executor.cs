using System.Collections.Generic;
using FirstSimplexPhase.Library;
using static FirstSimplexPhase.Demo.Utils;

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
                        new double[] {2, 8, 88})
                },
                {
                    1, (new double[,] {{-1, 1, 1, 0}, {4, 5, 0, 1}, {10, 17, 2, 3}}, 
                        new double[] {2, 8, 88})
                },
                
            };

        public static void PerformVariant(int variant)
        {
            var (conditions, constraints) = VariantInputs[variant];
            Perform(conditions, constraints);
        }
    }
}

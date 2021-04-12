using System;
using FirstSimplexPhase.Library;

namespace FirstSimplexPhase.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var A = new double[,]
            {
                {1, 2 , 3},
                {4, 5, 6}
            };

            var b = new double[] {-1, -2};

            FirstSimplexPhaseService.Solve(A, b);
        }
    }
}

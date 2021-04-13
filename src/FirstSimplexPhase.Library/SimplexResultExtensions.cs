using System.Collections.Generic;
using System.Linq;
using MainSimplexPhase.Core;
// ReSharper disable SimplifyLinqExpressionUseAll

namespace FirstSimplexPhase.Library
{
    public static class SimplexResultExtensions
    {
        public static bool HasSolutionChanged(this SimplexResult simplexResult, IEnumerable<double> previousSolution) => 
            !simplexResult.Solution.SequenceEqual(previousSolution);

        public static bool IsCompatible(this SimplexResult simplexResult, int lastElementCount) => 
            !simplexResult.Solution
                .TakeLast(lastElementCount)
                .Any(x => x == 0);
    }
}

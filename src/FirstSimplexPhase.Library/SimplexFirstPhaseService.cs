using System.Diagnostics;
using System.Linq;
using MainSimplexPhase.Core;

namespace FirstSimplexPhase.Library
{
    public static class SimplexFirstPhaseService
    {
        public static (Status status, FirstSimplexResult result) Solve(double[,] conditions, double[] constraints)
        {
            // ReSharper disable InconsistentNaming
            var A = LinearAlgebraUtils.BuildMatrixFrom2DArray(conditions);
            var b = LinearAlgebraUtils.BuildVectorFromArray(constraints);

            // if i-th b's element less than zero, multiply it and i-th A's row by -1
            Core.MultiplyByMinusOne(ref A, ref b);

            // form initial data for auxiliary problem solving
            var (
                    initialConditions, 
                    initialObjectiveFunction, 
                    initialSolution, 
                    initialBasisIndices) =
                InitialValuesUtils.GetInitialValues(A, b);

            var mainPhase = new SimplexMainPhaseService(
                initialConditions.ToArray(),
                initialObjectiveFunction.ToArray(),
                initialSolution.ToArray(),
                initialBasisIndices);
            var result = mainPhase.Maximize();

            Debug.WriteLine($"{nameof(result.Solution)}:\n{string.Join(", ", result.Solution)}\n");
            Debug.WriteLine($"{nameof(result.BasisIndices)}:\n{string.Join(", ", result.BasisIndices)}\n");

            if (!result.HasSolutionChanged(initialSolution)) // lib-specific comparison to determine whether solving has failed
                return (Status.Fail, FirstSimplexResult.Empty);

            if (!result.IsCompatible(A.RowCount))
                return (Status.Incompatible, FirstSimplexResult.Empty);
            
            var correctedIndices = Core.GetCorrectedBasisIndices(result, A.ColumnCount, A.RowCount, initialConditions);
            
            return (Status.Success, FirstSimplexResult.Create(result.Solution, correctedIndices));
        }
    }
}

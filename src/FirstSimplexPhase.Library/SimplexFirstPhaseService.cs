using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MainSimplexPhase.Core;
using MathNet.Numerics.LinearAlgebra;
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ParameterTypeCanBeEnumerable.Local
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable InconsistentNaming

namespace FirstSimplexPhase.Library
{
    public static class SimplexFirstPhaseService
    {
        private static void MinusOne(ref Matrix<double> matrix, Vector<double> v)
        {
            for (var i = 0; i < v.Count; i++)
                if (v[i] < 0)
                    matrix.SetRow(i, -matrix.Row(i));
        }

        private static Matrix<double> CreateIdentityMatrix(int size) =>
            Matrix<double>.Build.DiagonalIdentity(size);

        private static Matrix<double> AppendIdentity(Matrix<double> matrix, Matrix<double> identity) =>
            matrix.Append(identity);

        private static double[] CreateInitialObjectiveFunction(int columnCount, int rowCount)
        {
            var zeros = Enumerable.Repeat<double>(element: 0, count: columnCount);
            var minusOnes = Enumerable.Repeat<double>(element: -1, count: rowCount);
            var total = zeros.Concat(minusOnes);
            return total.ToArray();
        }

        private static Vector<double> CreateInitialSolutionVector(Vector<double> constraints, int zeroCount)
        {
            var zeros = Enumerable.Repeat<double>(element: 0, count: zeroCount);
            var total = zeros.Concat(constraints);
            return Vector<double>.Build.DenseOfEnumerable(total);
        }

        private static ISet<int> CreateInitialBasisIndices(int from, int count) =>
            Enumerable.Range(from, count).ToHashSet();

        public static (Status status, FirstSimplexResult result) Solve(double[,] conditions, double[] constraints)
        {
            var A = Matrix<double>.Build.DenseOfArray(conditions);
            var b = Vector<double>.Build.DenseOfArray(constraints);

            // if i-th b's element less than zero, multiply i-th A's row by -1
            MinusOne(ref A, b);
            
            // build extended A matrix with E matrix
            var initialConditions = AppendIdentity(A, CreateIdentityMatrix(A.RowCount));
            Debug.WriteLine($"{nameof(initialConditions)}:\n{initialConditions.ToMatrixString()}\n");
            
            // build c vector that consists of ColumnCount zeros and RowCount minus-ones
            var initialObjectiveFunction = CreateInitialObjectiveFunction(A.ColumnCount, A.RowCount);
            Debug.WriteLine($"{nameof(initialObjectiveFunction)}:\n{string.Join(", ", initialObjectiveFunction.AsEnumerable())}\n");

            // build x vector by appending b vector to ColumnCount zeros
            var initialSolution = CreateInitialSolutionVector(b, A.ColumnCount);
            Debug.WriteLine($"{nameof(initialSolution)}:\n{initialSolution.ToVectorString()}\n");
            
            // build J_b set that contains indices of last RowCount elements of initial sulution vector
            var initialBasisIndices = CreateInitialBasisIndices(A.ColumnCount, A.RowCount);
            Debug.WriteLine($"{nameof(initialBasisIndices)}:\n{string.Join(", ", initialBasisIndices)}\n");
            
            var mainPhase = new SimplexMainPhaseService(
                initialConditions.ToArray(),
                initialObjectiveFunction.ToArray(),
                initialSolution.ToArray(),
                initialBasisIndices);
            var result = mainPhase.Maximize();
            Debug.WriteLine($"solution:\n{string.Join(", ", result.Solution)}\n");
            Debug.WriteLine($"indices:\n{string.Join(", ", result.BasisIndices)}\n");

            if (!result.HasSolutionChanged(initialSolution)) // lib-specific comparison to determine whether calculating has failed
                return (Status.Fail, FirstSimplexResult.Empty);

            if (!result.IsCompatible(A.RowCount))
                return (Status.Incompatible, FirstSimplexResult.Empty);

            var basisIndices = new HashSet<int>(result.BasisIndices);
            
            // todo: mutate basis indices

            return (Status.Success, FirstSimplexResult.Create(result.Solution, basisIndices));
        }
    }
}

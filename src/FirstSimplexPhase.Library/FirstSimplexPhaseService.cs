using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MainSimplexPhase.Core;
using MathNet.Numerics.LinearAlgebra;

namespace FirstSimplexPhase.Library
{
    public static class FirstSimplexPhaseService
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

        private static Vector<double> CreateObjectiveFunctionVector(int columnCount, int rowCount)
        {
            var zeros = Enumerable.Repeat<double>(element: 0, count: columnCount); // n of 0's
            var minusOnes = Enumerable.Repeat<double>(element: -1, count: rowCount); // m of -1's
            var total = zeros.Concat(minusOnes); // total n + m array
            return Vector<double>.Build.DenseOfEnumerable(total);
        }

        private static Matrix<double> CreateInitialConditionsMatrix(int columnCount, int rowCount)
        {
            var identity = CreateIdentityMatrix(rowCount);
            var m = Matrix<double>.Build.Dense(rowCount, columnCount); // zero matrix
            return AppendIdentity(m, identity);
        }

        private static Vector<double> CreateInitialSolutionVector(Vector<double> previous, int zeroCount)
        {
            var zeros = Enumerable.Repeat<double>(element: 0, count: zeroCount); // n of 0's
            var total = zeros.Concat(previous); // zeros + previous vector
            return Vector<double>.Build.DenseOfEnumerable(total);
        }

        private static ISet<int> CreateInitialBasisIndices(int from, int count) =>
            Enumerable.Range(from, count).ToHashSet();

        // public static (Vector<double> X, ImmutableSortedSet<int> BasisIndices) Solve(Matrix<double> A, Vector<double> b)
        public static (Status status, FirstSimplexResult result) Solve(double[,] conditions, double[] constraints)
        {
            // ReSharper disable once InconsistentNaming
            var A = Matrix<double>.Build.DenseOfArray(conditions);
            var b = Vector<double>.Build.DenseOfArray(constraints);

            MinusOne(ref A, b); // if i-th b's element less than zero, multiply i-th A's row by -1

            var extended = AppendIdentity(A, CreateIdentityMatrix(A.RowCount)); // A|E
            Debug.WriteLine($"extended:\n{extended.ToMatrixString()}\n");

            var objectiveFunction = CreateObjectiveFunctionVector(A.ColumnCount, A.RowCount); // new c
            Debug.WriteLine($"objectiveFunction:\n{objectiveFunction.ToVectorString()}\n");

            var initialConditions = CreateInitialConditionsMatrix(A.ColumnCount, A.RowCount); // new A 
            Debug.WriteLine($"initialConditions:\n{initialConditions.ToMatrixString()}\n");

            var initialSolution = CreateInitialSolutionVector(b, A.ColumnCount); // new x
            Debug.WriteLine($"initialSolution:\n{initialSolution.ToVectorString()}\n");

            var initialBasisIndices = CreateInitialBasisIndices(A.ColumnCount, A.RowCount); // new J_b
            Debug.WriteLine($"initialBasisIndices:\n{string.Join(", ", initialBasisIndices)}\n");

            var mainPhase = new SimplexMainPhaseService(
                initialConditions.ToArray(),
                objectiveFunction.ToArray(),
                initialSolution.ToArray(),
                initialBasisIndices);
            var result = mainPhase.Maximize();

            if (!result.HasSolutionChanged(initialSolution))
                return (Status.Fail, FirstSimplexResult.Failure);

            if (!result.IsCompatible()) // solution is incompatible
                return (Status.Incompatible, FirstSimplexResult.Failure);

            var basisIndices = new HashSet<int>(result.BasisIndices);
            
            // todo: mutate basis indices

            return (Status.Success, FirstSimplexResult.Create(result.Solution, basisIndices));
        }
    }
}

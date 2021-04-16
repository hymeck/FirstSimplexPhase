using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using static FirstSimplexPhase.Library.LinearAlgebraUtils; // GetZeros, GetMinusOnes, AppendIdentity, GetIdentityMatrix

namespace FirstSimplexPhase.Library
{
    internal static class InitialValuesUtils
    {
        public static double[] GetInitialObjectiveFunction(int columnCount, int rowCount)
        {
            var zeros = GetZeros(columnCount);
            var minusOnes = GetMinusOnes(rowCount);
            var total = zeros.Concat(minusOnes);
            return total.ToArray();
        }
        
        public static Vector<double> GetInitialSolutionVector(Vector<double> constraints, int zeroCount)
        {
            var zeros = GetZeros(zeroCount);
            var total = zeros.Concat(constraints);
            return Vector<double>.Build.DenseOfEnumerable(total);
        }
        
        public static ISet<int> GetInitialBasisIndices(int from, int count) => Enumerable.Range(from, count).ToHashSet();
        
        public static (
            Matrix<double> initialConditions, 
            double[] initialObjectiveFunction, 
            Vector<double> initialSolution, 
            ISet<int> initialBasisIndices
            ) 
            GetInitialValues(Matrix<double> conditions,
                Vector<double> constraints)
        {
            // build extended A matrix with E matrix
            var initialConditions = AppendIdentity(conditions, GetIdentityMatrix(conditions.RowCount));
            Debug.WriteLine($"{nameof(initialConditions)}:\n{initialConditions.ToMatrixString()}\n");

            // build c vector that consists of ColumnCount zeros and RowCount minus-ones
            var initialObjectiveFunction = GetInitialObjectiveFunction(conditions.ColumnCount, conditions.RowCount);
            Debug.WriteLine(
                $"{nameof(initialObjectiveFunction)}:\n{string.Join(", ", initialObjectiveFunction.AsEnumerable())}\n");

            // build x vector by appending b vector to ColumnCount zeros
            var initialSolution = GetInitialSolutionVector(constraints, conditions.ColumnCount);
            Debug.WriteLine($"{nameof(initialSolution)}:\n{initialSolution.ToVectorString()}\n");

            // build J_b set that contains indices of last RowCount elements of initial sulution vector
            var initialBasisIndices = GetInitialBasisIndices(conditions.ColumnCount, conditions.RowCount);
            Debug.WriteLine($"{nameof(initialBasisIndices)}:\n{string.Join(", ", initialBasisIndices)}\n");

            return (initialConditions, initialObjectiveFunction, initialSolution, initialBasisIndices);
        }
    }
}

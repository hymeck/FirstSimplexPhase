using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
// ReSharper disable ArgumentsStyleNamedExpression

namespace FirstSimplexPhase.Library
{
    internal static class LinearAlgebraUtils
    {
        public static Matrix<double> GetIdentityMatrix(int size) =>
            Matrix<double>.Build.DiagonalIdentity(size);

        public static Matrix<double> AppendIdentity(Matrix<double> matrix, Matrix<double> identity) =>
            matrix.Append(identity);
        
        public static IEnumerable<double> GetNumberSequence(int number, int count) => 
            Enumerable.Repeat<double>(element: number, count: count);

        public static IEnumerable<double> GetZeros(int count) => GetNumberSequence(0, count);
        public static IEnumerable<double> GetMinusOnes(int count) => GetNumberSequence(-1, count);
        
        public static Matrix<double> CreateMatrixFromColumns(IEnumerable<Vector<double>> vectors) => 
            Matrix<double>.Build.DenseOfColumns(vectors);
        
        public static Matrix<double> CreateMatrixFromRows(IEnumerable<Vector<double>> vectors) => 
            Matrix<double>.Build.DenseOfRows(vectors);
        

        public static Matrix<double> BuildMatrixFrom2DArray(double[,] matrix) =>
            Matrix<double>.Build.DenseOfArray(matrix);
        
        public static Vector<double> BuildVectorFromArray(double[] vector) =>
            Vector<double>.Build.DenseOfArray(vector);
    }
}

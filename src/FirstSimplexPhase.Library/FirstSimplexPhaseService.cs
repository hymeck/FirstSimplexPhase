using MainSimplexPhase.Core;
using MathNet.Numerics.LinearAlgebra;

namespace FirstSimplexPhase.Library
{
    public class FirstSimplexPhaseService
    {
        public static void MinusOne(ref Matrix<double> matrix, Vector<double> v)
        {
            for (var i = 0; i < v.Count; i++)
                if (v[i] < 0)
                    matrix.SetRow(i, -matrix.Row(i));
        }

        public static Matrix<double> CreateIdentityMatrix(int size)
        {
            return Matrix<double>.Build.Diagonal(size, size);
        }

        public static Matrix<double> AppendIdentity(Matrix<double> matrix, Matrix<double> identity)
        {
            return matrix.Append(identity);
        }
        
        // public static (Vector<double> X, ImmutableSortedSet<int> BasisIndices) Solve(Matrix<double> A, Vector<double> b)
        public static void Solve(double[,] A, double[] b)
        {
            var _A = Matrix<double>.Build.DenseOfArray(A);
            var _b = Vector<double>.Build.DenseOfArray(b);
            
            MinusOne(ref _A, _b);
            
            var extendedA = AppendIdentity(_A, CreateIdentityMatrix(_A.RowCount));
            
            var s = new SimplexMainPhaseService()
        }
    }
}

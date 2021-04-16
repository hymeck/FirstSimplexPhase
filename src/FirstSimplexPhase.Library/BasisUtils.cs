using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace FirstSimplexPhase.Library
{
    internal static class BasisUtils
    {
        public static IEnumerable<int> GetNonBasisValues(IEnumerable<int> indices, IEnumerable<int> basisIndices, int columnCount) =>
            indices
                .Except(basisIndices)
                .Where(i => i < columnCount);

        public static Matrix<double> GetBasisMatrix(Matrix<double> source, IEnumerable<int> basisIndices)
        {
            var columns = basisIndices.Select(source.Column);
            return LinearAlgebraUtils.CreateMatrixFromVectors(columns);
        }
    }
}

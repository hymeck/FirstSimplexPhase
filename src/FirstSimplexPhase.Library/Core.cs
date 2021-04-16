using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MainSimplexPhase.Core;
using MathNet.Numerics.LinearAlgebra;
using static FirstSimplexPhase.Library.LinearAlgebraUtils;

namespace FirstSimplexPhase.Library
{
    internal static class Core
    {
        public static void MultiplyByMinusOne(ref Matrix<double> matrix, ref Vector<double> v)
        {
            for (var i = 0; i < v.Count; i++)
                if (v[i] < 0)
                {
                    matrix.SetRow(i, -matrix.Row(i));
                    v[i] *= -1;
                }
        }
        
        public static Matrix<double> GetProductMatrix(Matrix<double> initialConditions, IEnumerable<int> freeVariables, Matrix<double> inversedBasisMatrix)
        {
            var products = freeVariables
                .Select(initialConditions.Column)
                .Select(inversedBasisMatrix.Multiply);
            
            return CreateMatrixFromVectors(products);
        }
        
        public static bool ReplaceBasisIndices(Matrix<double> products, int index, ref List<int> basisIndicesList, int basisItem, IList<int> freeVariables)
        {
            var hasChanged = true;
            for (var i = 0; i < products.RowCount; i++)
            {
                if (products[i, index] == 0) 
                    continue;
                
                hasChanged = false;
                basisIndicesList[basisIndicesList.IndexOf(basisItem)] = freeVariables[i];
            }

            return hasChanged;
        }
        
        public static void CorrectBasis(ref HashSet<int> basisIndices, int columnCount, int rowCount, Matrix<double> initialConditions)
        {
            var basisIndicesList = basisIndices.ToList();

            var indices = Enumerable.Range(0, columnCount + rowCount).ToArray();
            foreach (var (index, basisItem) in basisIndicesList.Select((basisItem, index) => (index, basisItem)))
            {
                if (basisItem <= columnCount)
                    continue;

                var freeVariables = BasisUtils.GetNonBasisValues(indices, basisIndices, columnCount).ToList();
                Debug.WriteLine($"{nameof(freeVariables)}\n{string.Join(", ", freeVariables)}\n");

                var basisMatrix = BasisUtils.GetBasisMatrix(initialConditions, basisIndicesList);
                Debug.WriteLine($"{nameof(basisMatrix)}\n{basisMatrix.ToMatrixString()}\n");

                var inversedBasisMatrix = basisMatrix.Inverse();
                Debug.WriteLine($"{nameof(inversedBasisMatrix)}\n{inversedBasisMatrix.ToMatrixString()}\n");
                
                var products = GetProductMatrix(initialConditions, freeVariables, inversedBasisMatrix);

                var case2 = ReplaceBasisIndices(products, index, ref basisIndicesList, basisItem, freeVariables);
                if (case2)
                    basisIndicesList.Remove(basisItem);
            }
            
            basisIndices.IntersectWith(basisIndicesList);
        }
        
        public static ISet<int> GetCorrectedBasisIndices(SimplexResult result, int columnCount, int rowCount, Matrix<double> initialConditions)
        {
            var basisIndices = new HashSet<int>(result.BasisIndices);
            CorrectBasis(ref basisIndices, columnCount, rowCount, initialConditions);
            
            return basisIndices;
        }
    }
}

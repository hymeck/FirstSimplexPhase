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
            
            return CreateMatrixFromRows(products);
        }
        
        public static bool ReplaceBasisIndices(Matrix<double> products, int index, ref List<int> basisIndicesList, int basisElement, IList<int> freeVariables)
        {
            var hasChanged = true;
            for (var i = 0; i < products.RowCount; i++)
            {
                if (products[i, index] == 0) 
                    continue;
                
                hasChanged = false;
                basisIndicesList[basisIndicesList.IndexOf(basisElement)] = freeVariables[i];
            }

            return hasChanged;
        }
        
        public static void CorrectBasis(ref HashSet<int> basisIndices, int columnCount, int rowCount, Matrix<double> initialConditions)
        {
            var basisIndicesList = basisIndices.ToList();
            var mutableBasisIndices = new List<int>(basisIndicesList); // because we can't mutate collection while iterating it

            var indices = Enumerable.Range(0, columnCount + rowCount).ToArray();
            foreach (var (index, basisElement) in basisIndicesList.Select((basisItem, index) => (index, basisItem)))
            {
                if (basisElement <= columnCount)
                    continue;

                var basisMatrix = BasisUtils.GetBasisMatrix(initialConditions, basisIndicesList);
                Debug.WriteLine($"{nameof(basisMatrix)}\n{basisMatrix.ToMatrixString()}\n");
                
                var inversedBasisMatrix = basisMatrix.Inverse();
                Debug.WriteLine($"{nameof(inversedBasisMatrix)}\n{inversedBasisMatrix.ToMatrixString()}\n");

                var freeVariables = BasisUtils.GetNonBasisValues(indices, basisIndices, columnCount).ToList();
                Debug.WriteLine($"{nameof(freeVariables)}\n{string.Join(", ", freeVariables)}\n");
                
                var products = GetProductMatrix(initialConditions, freeVariables, inversedBasisMatrix);
                Debug.WriteLine($"{nameof(products)}\n{products.ToMatrixString()}\n");

                var hasChanged = ReplaceBasisIndices(products, index, ref mutableBasisIndices, basisElement, freeVariables);
                
                if (hasChanged)
                    mutableBasisIndices.Remove(basisElement);
            }
            
            basisIndices.IntersectWith(mutableBasisIndices);
        }
        
        public static IEnumerable<int> GetCorrectedBasisIndices(SimplexResult result, int columnCount, int rowCount, Matrix<double> initialConditions)
        {
            var basisIndices = new HashSet<int>(result.BasisIndices);
            CorrectBasis(ref basisIndices, columnCount, rowCount, initialConditions);
            
            return basisIndices;
        }
    }
}

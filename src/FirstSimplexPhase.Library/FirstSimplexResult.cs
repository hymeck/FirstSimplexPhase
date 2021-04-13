﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace FirstSimplexPhase.Library
{
    public sealed class FirstSimplexResult
    {
        public static readonly FirstSimplexResult Failure = new(ImmutableArray<double>.Empty, ImmutableSortedSet<int>.Empty);

        public ImmutableArray<double> FeasibleSolution { get; init; }
        public ImmutableSortedSet<int> BasisIndices { get; init; }

        private FirstSimplexResult(ImmutableArray<double> feasibleSolution, ImmutableSortedSet<int> basisIndices)
        {
            FeasibleSolution = feasibleSolution;
            BasisIndices = basisIndices;
        }

        public bool IsEmpty => FeasibleSolution.IsEmpty || BasisIndices.IsEmpty;

        public static FirstSimplexResult Create(ImmutableArray<double> feasibleSolution, ImmutableSortedSet<int> basisIndices) =>
            new(feasibleSolution, basisIndices);

        public static FirstSimplexResult Create(IEnumerable<double> feasibleSolution, IEnumerable<int> basisIndices) =>
            Create(feasibleSolution.ToImmutableArray(), basisIndices.ToImmutableSortedSet());
    }
}

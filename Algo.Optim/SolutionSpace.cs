using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionSpace
    {
        readonly Random _random;
        SolutionInstance _bestEver;

        protected SolutionSpace()
        {
            _random = new Random();
        }

        protected SolutionSpace( int randomSeed )
        {
            _random = new Random( randomSeed );
        }

        protected void Initialize( IReadOnlyList<int> d )
        {
            Dimensions = d;
        }

        public IReadOnlyList<int> Dimensions { get; private set; }

        public double Cardinality => Dimensions.Aggregate( 1.0, ( acc, val ) => acc * val );

        public SolutionInstance BestEver => _bestEver;

        public SolutionInstance CreateRandomInstance()
        {
            var r = new int[Dimensions.Count];
            for( int i = 0; i < r.Length; ++i )
            {
                r[i] = _random.Next( Dimensions[i] );
            }
            return DoCreateInstance( r );
        }

        public SolutionInstance ComputeBestRandom( int count )
        {
            return Enumerable.Range( 0, count ).Select( _ => CreateRandomInstance() ).Best();
        }

        public SolutionInstance ComputeBestMonteCarlo( int count )
        {
            return Enumerable.Range( 0, count )
                    .Select( _ => CreateRandomInstance().FindBestAround() )
                    .Best();
        }

        internal SolutionInstance DoCreateInstance( IReadOnlyList<int> coords )
        {
            var s = CreateInstance( coords );
            if( _bestEver == null || _bestEver.Cost > s.Cost ) _bestEver = s;
            return s;
        }

        protected abstract SolutionInstance CreateInstance( IReadOnlyList<int> coords );
    }
}

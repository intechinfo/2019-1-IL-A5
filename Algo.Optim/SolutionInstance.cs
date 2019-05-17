using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public abstract class SolutionInstance
    {
        double _cost;

        protected SolutionInstance( SolutionSpace space, IReadOnlyList<int> coords )
        {
            _cost = -1.0;
            Space = space;
            Coords = coords;
            Debug.Assert( CheckCoords() );
        }

        public bool CheckCoords()
        {
            if( Coords.Count == Space.Dimensions.Count )
            {
                for( int i = 0; i < Coords.Count; i++ )
                {
                    if( Coords[i] < 0 || Coords[i] >= Space.Dimensions[i] )
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public IReadOnlyList<int> Coords { get; }

        public SolutionSpace Space { get; }

        public double Cost
        {
            get
            {
                if( _cost >= 0.0 ) return _cost;
                return _cost = ComputeCost();
            }
        }

        public IEnumerable<SolutionInstance> Neighbors
        {
            get
            {
                for( int i = 0; i < Coords.Count; ++i )
                {
                    if( Coords[i] > 0 )
                    {
                        var clone = Coords.ToArray();
                        --clone[i];
                        yield return Space.DoCreateInstance( clone );
                    }
                    if( Coords[i] < Space.Dimensions[i]-1 )
                    {
                        var clone = Coords.ToArray();
                        ++clone[i];
                        yield return Space.DoCreateInstance( clone );
                    }
                }
            }
        }

        public SolutionInstance FindBestAround()
        {
            var b = Neighbors.Best();
            return b.Cost > Cost ? this : b.FindBestAround();
        }

        protected abstract double ComputeCost();
    }
}

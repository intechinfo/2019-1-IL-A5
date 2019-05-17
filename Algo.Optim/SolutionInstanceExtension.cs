using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public static class SolutionInstanceExtension
    {
        public static SolutionInstance Best( this IEnumerable<SolutionInstance> candidates )
        {
            SolutionInstance best = null;
            foreach( var s in candidates )
            {
                if( best == null || s.Cost < best.Cost ) best = s;
            }
            return best;
        }

    }
}

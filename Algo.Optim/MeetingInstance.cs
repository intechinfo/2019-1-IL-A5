using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algo.Optim
{
    public class MeetingInstance : SolutionInstance
    {
        internal MeetingInstance( Meeting m, IReadOnlyList<int> coords )
            : base( m, coords )
        {
        }

        public new Meeting Space => (Meeting)base.Space;

        protected override double ComputeCost()
        {
            return 0.0;
        }
    }
}

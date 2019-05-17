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
            var arrivals = Space.Guests.Select( g => g.ArrivalFlights[Coords[g.Index]] ).ToArray();
            var departures = Space.Guests.Select( g => g.DepartureFlights[Coords[g.Index+9]] ).ToArray();

            double cost = arrivals.Select( f => f.Price ).Sum() + departures.Select( f => f.Price ).Sum();

            var latestArrival = arrivals.Select( f => f.ArrivalTime ).Max();
            var earlierDeparture = arrivals.Select( f => f.DepartureTime ).Min();

            cost += arrivals.Select( (f,idx) => (latestArrival - f.ArrivalTime).TotalMinutes * Space.Guests[idx].PricePerMinute )
                            .Sum();
            cost += departures.Select( (f,idx) => (f.DepartureTime - earlierDeparture).TotalMinutes * Space.Guests[idx].PricePerMinute )
                            .Sum();

            if( earlierDeparture.Hour > 18 ) cost += 150;

            return cost;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algo.Optim
{
    public class Guest
    {
        public string Name { get; set; }

        public Airport Location { get; set; }

        public int Index { get; set; }

        public List<SimpleFlight> ArrivalFlights { get; set; }

        public List<SimpleFlight> DepartureFlights { get; set; }

        public double PricePerMinute { get; set; } = 5;

    }

    public class Meeting : SolutionSpace
    {
        readonly FlightDatabase _db;

        public Meeting( FlightDatabase db )
        {
            _db = db;

            Location = Airport.FindByCode( "LHR" );
            Guests = new List<Guest>()
            {
                new Guest(){ Name="Helmut", Location= Airport.FindByCode("BER") },
                new Guest(){ Name="Bernard", Location= Airport.FindByCode("CDG") },
                new Guest(){ Name="Marius", Location= Airport.FindByCode("MRS") },
                new Guest(){ Name="Hubert", Location= Airport.FindByCode("LYS") },
                new Guest(){ Name="Tom", Location= Airport.FindByCode("MAN") },
                new Guest(){ Name="Maria", Location= Airport.FindByCode("BIO") },
                new Guest(){ Name="Bob", Location= Airport.FindByCode("JFK") },
                new Guest(){ Name="Ahmed", Location= Airport.FindByCode("TUN") },
                new Guest(){ Name="Luigi", Location= Airport.FindByCode("MXP") }
            };
            MaxArrivalTime = new DateTime( 2010, 7, 27, 17, 0, 0 );
            MinDepartureTime = new DateTime( 2010, 8, 3, 15, 0, 0 );

            int i = 0;
            foreach( var g in Guests )
            {
                g.Index = i++;
                var aFlights = _db.GetFlights( MaxArrivalTime.Date, g.Location, Location )
                                .Concat( _db.GetFlights( MaxArrivalTime.Date.AddDays( -1 ), g.Location, Location ) )
                                .Where( f => f.ArrivalTime < MaxArrivalTime
                                             && f.ArrivalTime > MaxArrivalTime.AddHours( -6 ) );
                g.ArrivalFlights = aFlights.ToList();
                var dFlights = _db.GetFlights( MinDepartureTime.Date, Location, g.Location )
                                  .Concat( _db.GetFlights( MinDepartureTime.Date.AddDays( 1 ), Location, g.Location ) )
                                  .Where( f => f.DepartureTime > MinDepartureTime
                                               && f.DepartureTime < MinDepartureTime.AddHours( 6 ) );
                g.DepartureFlights = dFlights.ToList();
            }

            var dimensions = new int[18];
            for( int i = 0; i < 18; i++ )
            {
                if( i < 9 )
                {
                    dimensions[i] = Guests[i].ArrivalFlights.Count;
                }
                else
                {
                    dimensions[i] = Guests[i-9].DepartureFlights.Count;
                }
            }
            Initialize( dimensions );
        }

        public Airport Location { get; }

        public IReadOnlyList<Guest> Guests { get; }

        public DateTime MaxArrivalTime { get; }

        public DateTime MinDepartureTime { get; }

        protected override SolutionInstance CreateInstance( IReadOnlyList<int> coords )
        {
            return new MeetingInstance( this, coords );
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    public class RecoContext
    {
        public IReadOnlyList<User> Users { get; private set; }
        public IReadOnlyList<Movie> Movies { get; private set; }
        public int RatingCount { get; private set; }

        public double Distance( User u1, User u2 )
        {
            bool atLeastOne = false;
            int sum2 = 0;
            foreach( var movieR1 in u1.Ratings )
            {
                if( u2.Ratings.TryGetValue( movieR1.Key, out var r2 ) )
                {
                    atLeastOne = true;
                    sum2 += (movieR1.Value - r2) ^ 2;
                }
            }
            return atLeastOne ? Math.Sqrt( sum2 ) : double.PositiveInfinity;
        }

        public double Similarity( User u1, User u2 ) => 1.0 / (1.0 + Distance( u1, u2 ));

        public double SimilarityPearson( User u1, User u2 )
        {
            var commonMovies = u1.Ratings.Keys.Intersect( u2.Ratings.Keys );
            return SimilarityPearson( commonMovies.Select( m => (u1.Ratings[m], u2.Ratings[m]) ) );
        }

        public double SimilarityPearson( IEnumerable<(int x, int y)> values )
        {
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXY = 0.0;
            double sumX2 = 0.0;
            double sumY2 = 0.0;
            int N = 0;
            foreach( var t in values )
            {
                sumXY += t.x * t.y;
                sumX += t.x;
                sumY += t.y;
                sumX2 += t.x * t.x;
                sumY2 += t.y * t.y;
                ++N;
            }
            #region  Edge case....
            if( N == 0 ) return 0.0;
            if( N == 1 )
            {
                var onlyOne = values.Single();
                double d = Math.Abs( onlyOne.x - onlyOne.y );
                return 1 / (1 + d);
            }
            #endregion

            double numerator = sumXY - (sumX * sumY / N);

            double denominatorX = sumX2 - (sumX * sumX / N);
            double denominatorY = sumY2 - (sumY * sumY / N);
            var result = numerator / Math.Sqrt( denominatorX * denominatorY );

            #region Edge case
            if( double.IsNaN( result ) )
            {
                double sumSquare = values.Select( v => v.x - v.y ).Select( v => v * v ).Sum();
                result = 1.0 / (1 + Math.Sqrt( sumSquare ));
            }
            #endregion

            return result;
        }

        public bool LoadFrom( string folder )
        {
            string p = Path.Combine( folder, "users.dat" );
            if( !File.Exists( p ) ) return false;
            Users = User.ReadUsers( p );
            p = Path.Combine( folder, "movies.dat" );
            if( !File.Exists( p ) ) return false;
            Movies = Movie.ReadMovies( p );
            p = Path.Combine( folder, "ratings.dat" );
            if( !File.Exists( p ) ) return false;
            RatingCount = User.ReadRatings( Users, Movies, p );
            return true;
        }

        public class RecoOptions
        {
            public int MaxCount { get; set; } = 20;

            public int NbSimilarUser { get; set; } = 500;
        }

       public IEnumerable<(Movie movie, double weight)> GetRecommandation( User u, RecoOptions options = null )
        {
            options = options ?? new RecoOptions();
            var similarUsers = GetSimilarUsers( u, options.NbSimilarUser );

            var allMovies = similarUsers
                            .SelectMany( su => su.u.Ratings.Keys
                                                .Where( m => !u.Ratings.ContainsKey( m ) )
                                                .Select( m => (Movie:m, Weight:su.similarity * su.u.Ratings[m] ) ) );
            var continued = allMovies.GroupBy( mw => mw.Movie )
                                     .Select( g => (movie: g.Key, w: g.Sum( mw => mw.Weight )) )
                                     .OrderByDescending( mw => mw.w )
                                     .Take( options.MaxCount );
            return continued;
        }

        public IReadOnlyList<(User u, double similarity)> GetSimilarUsers( User u, int nbSimilarUser )
        {
            (User u, double similarity)[] simialrs = new (User u, double similarity)[ Users.Count - 1 ];
            int i = 0;
            foreach( var other in Users )
            {
                if( u == other ) continue;
                var s = SimilarityPearson( u, other );
                simialrs[i++] = (other, s);
            }
            return simialrs
                    .OrderByDescending( t =>  Math.Abs(t.similarity) )
                    .Take( nbSimilarUser )
                    .ToArray();
        }


    }
}

using System;
using System.Threading;
using Netflix;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace NetflixPrize
{
	class MainClass
	{
		private static string ReviewPath = "/home/shareff/Dev/Db/reviews.sqlite3";
		private static string MoviePath = "/home/shareff/Dev/Db/toto.db";
		private static string MeanPath = "/home/shareff/Dev/Db/mean.sqlite3";

		private static int _count;

		public static void Main (string[] args)
		{
			new CommonMovie ().GetCommonMovies ();
		}

		#region MeanCalculator 

		private static void MeanCalculator()
		{
			var calculator = new MovieMeanCalculator (ReviewPath, MoviePath, MeanPath);
			
			calculator.MeanCalculated += (Action<MovieMean>)(mean => 
			{
				Console.WriteLine ("{0}({1}) : {2} ({3} mean calculated)", mean.Title, mean.Id, mean.Mean, Interlocked.Increment (ref _count));
			});
			
			calculator.CalculateForAll ();
		}

		#endregion

		#region GetCommonMovie

		class CommonMovie
		{

			private ReviewDatabaseLayer<Review> _reviewConnection;
			private MovieDatabaseLayer _movieConnection;

			public void GetCommonMovies()
			{
				_reviewConnection = new ReviewDatabaseLayer<Review> (ReviewPath);
				_movieConnection = new MovieDatabaseLayer (MoviePath);

				while (true) 
				{
//					var id1 = GetUser (1);
//					var id2 = GetUser (2);

					var id1 = 387418;
					var id2 = 1008749;

//					var sw = new Stopwatch ();
//					sw.Start ();

					//var movies = _reviewConnection.GetCommonMovies (id1, id2).ToArray ();

					var movies = _reviewConnection.GetCommonMovies (id1, id2);

					//sw.Stop ();

					var moviesWithTitle = movies.Select (r => new {Title = _movieConnection.GetMovie (r.Item1.MovieId).Title, Id = r.Item1.MovieId});

//					Console.WriteLine ("Found {0} common movies in {1}ms", movies.Count (), sw.ElapsedMilliseconds);
					foreach (var movie in moviesWithTitle) 
					{
						Console.WriteLine (" - {0} ({1})", movie.Title, movie.Id);
					}

					Console.ReadLine ();
				}
			}

			private int GetUser (int user)
			{
				while (true) 
				{
					Console.Write ("User {0} : ", user);
					var user1 = Console.ReadLine ();
					int id;
					if (int.TryParse (user1, out id)) 
					{
						return id;
					} 
					else 
					{
						Console.WriteLine ("Couldn't get id");
					}
				}
			}
		}

		#endregion
	}
}

using System;
using Netflix;
using SQLite;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetflixPrize
{
	public class MovieMeanCalculator
	{
		private readonly ReviewDatabaseLayer<Review> _reviewConnection;
		private readonly MovieDatabaseLayer _movieConnection;
		private readonly MovieMeanConnection _meanConnection;

		public MovieMeanCalculator(string reviewPath, string moviePath, string meanPath)
		{
			_reviewConnection = new ReviewDatabaseLayer<Review> (reviewPath);
			_movieConnection = new MovieDatabaseLayer (moviePath);
			_meanConnection = new MovieMeanConnection (meanPath);
		}

		public IEnumerable<MovieMean> CalculateForAll()
		{
			var sw = new Stopwatch ();
			sw.Start ();

			var movies = _movieConnection.GetAllMovies ();
			foreach (var movie in movies) 
			{
				var reviews = _reviewConnection.GetReviewsByMovieId (movie.Id).ToArray ();
				if (reviews.Any ()) 
				{
					var mean = ((float)(reviews.Sum (r => r.Note))) / reviews.Count ();

					var movieMean = new MovieMean { Title = movie.Title, Id = movie.Id, Mean = mean };
					_meanConnection.Save (movieMean);

					yield return movieMean;
				}
			}

			sw.Stop ();
			Console.WriteLine ("Calculated {0} means in {1}", movies.Count (), sw.Elapsed);
		}

		public float Calculate(int movieId)
		{
			var movies = _reviewConnection.GetReviewsByMovieId (movieId).ToArray ();
			var mean = ((float)(movies.Sum (r => r.Note))) / movies.Count();

			_meanConnection.Save (new MovieMean { Title = _movieConnection.GetMovie(movieId).Title, Id = movieId, Mean = mean });

			return mean;
		}
	}

	public class MovieMean
	{
		public string Title {get;set;}
		public int Id {get;set;}
		public float Mean {get;set;}
	}

	public class MovieMeanConnection : SQLiteConnection
	{
		public MovieMeanConnection(string dbPath) : base(dbPath)
		{
			CreateTable<MovieMean> ();
		}

		public MovieMean Get(int id)
		{
			return Table<MovieMean> ().Where (m => m.Id == id).FirstOrDefault();
		}

		public void Save(MovieMean mean)
		{
			Insert (mean);
		}
	}
}


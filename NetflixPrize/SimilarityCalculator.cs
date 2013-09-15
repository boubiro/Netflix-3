using System;
using Netflix;
using System.Linq;

namespace NetflixPrize
{
	public static class SimilarityCalculator
	{		
		public static float CalculateForUser(int user1, int user2, string dbPath)
		{
			dbPath = "/home/shareff/Dev/Db/reviews.sqlite3";
			var db = new ReviewDatabaseLayer<Review>(dbPath);
			
			var user1Reviews = db.GetReviewsByUserId(user1).ToList();
			var user2Reviews = db.GetReviewsByUserId(user2).ToList();
			
			var meanUser1 = user1Reviews.Sum(r => r.Note) / user1Reviews.Count;
			var meanUser2 = user2Reviews.Sum(r => r.Note) / user1Reviews.Count;
			
			var intersect = user1Reviews.Intersect(user2Reviews).Select(r => r.MovieId).ToList();
			
			var filteredUser1Reviews = user1Reviews.Where(r => intersect.Contains(r.MovieId)).OrderBy(r => r.MovieId).ToArray();
			var filteredUser2Reviews = user2Reviews.Where(r => intersect.Contains(r.MovieId)).OrderBy(r => r.MovieId).ToArray();
			
			return Calculate(intersect.Count, filteredUser1Reviews, filteredUser2Reviews, meanUser1, meanUser2);	
		}
		
		public static float CalculateForMovie(int movie1, int movie2, string dbPath)
		{
			var db = new ReviewDatabaseLayer<Review>(dbPath);
			
			var movie1Reviews = db.GetReviewsByMovieId(movie1).ToList();
			var movie2Reviews = db.GetReviewsByMovieId(movie2).ToList();
			
			var intersect = movie1Reviews.Intersect(movie2Reviews).Select(r => r.UserId).ToList();
			
			var filteredMovie1Reviews = movie1Reviews.Where(r => intersect.Contains(r.UserId)).OrderBy(r => r.UserId).ToArray();
			var filteredMovie2Reviews = movie2Reviews.Where(r => intersect.Contains(r.UserId)).OrderBy(r => r.UserId).ToArray();
			
			int count = intersect.Count;
			
			return Calculate(count, filteredMovie1Reviews, filteredMovie2Reviews, 0, 1);
		}
		
		private static float Calculate(int totalCount, Review[] uno, Review[] dos, int meanUno, int meanDos)
		{
			int sum = 0;
			for (var i = 0; i < totalCount; i++)
			{
				var diffUno = uno[i].Note - meanUno;
				var diffDos = dos[i].Note - meanDos;
				
//				sum += Math.Abs(uno[i].Note - dos[i].Note); 
				sum += Math.Abs(diffDos - diffUno);
			}
			
			var sim = 1 - (float)sum / (8 * totalCount);
			
			return sim;	
		}


		public static float VarianceForMovie(int movie, string dbPath)
		{
			var db = new ReviewDatabaseLayer<Review>(dbPath);

			var movieReviews = db.GetReviewsByMovieId(movie).ToList();

			var meanMovie = movieReviews.Sum(r => r.Note) / movieReviews.Count;

			var varMovie = movieReviews.Sum (r => Math.Pow ((r.Note - meanMovie), 2)) / movieReviews.Count;


			return varMovie;
		}

	}
}


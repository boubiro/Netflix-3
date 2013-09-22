using System;
using Netflix;
using System.Linq;

namespace NetflixPrize
{
	public abstract class Calculator
	{
		protected ReviewDatabaseLayer<Review> _reviewsConnection;
		
		public Calculator(string dbPath)
		{
			_reviewsConnection = new ReviewDatabaseLayer<Review> (dbPath);
		}	
	}

	public class SimilarityCalculator : Calculator
	{	
		public SimilarityCalculator(string dbPath) : base(dbPath)
		{
		}

		public float CalculateForUser(int user1, int user2)
		{			
			var user1Reviews = _reviewsConnection.GetReviewsByUserId(user1).ToList();
			var user2Reviews = _reviewsConnection.GetReviewsByUserId(user2).ToList();
			
			var meanUser1 = user1Reviews.Sum(r => r.Note) / user1Reviews.Count;
			var meanUser2 = user2Reviews.Sum(r => r.Note) / user1Reviews.Count;
			
			var intersect = user1Reviews.Intersect(user2Reviews).Select(r => r.MovieId).ToList();
			
			var filteredUser1Reviews = user1Reviews.Where(r => intersect.Contains(r.MovieId)).OrderBy(r => r.MovieId).ToArray();
			var filteredUser2Reviews = user2Reviews.Where(r => intersect.Contains(r.MovieId)).OrderBy(r => r.MovieId).ToArray();
			
			return Calculate(intersect.Count, filteredUser1Reviews, filteredUser2Reviews, meanUser1, meanUser2);	
		}
		
		public float CalculateForMovie(int movie1, int movie2)
		{	
			var movie1Reviews = _reviewsConnection.GetReviewsByMovieId(movie1).ToList();
			var movie2Reviews = _reviewsConnection.GetReviewsByMovieId(movie2).ToList();
			
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
	}
	
	public sealed class VarianceCalculator : Calculator
	{
		private readonly VarianceConnection _varConnection;

		public VarianceCalculator(string dbPath) : base(dbPath)
		{
			//_varConnection = new VarianceConnection (variancePath);
		}

		public Variance VarianceForMovie(int movie)
		{	
			double sum = 0;

			var movieReviews = _reviewsConnection.GetReviewsByMovieId(movie).ToList();
			var meanMovie = (float)movieReviews.Sum(r => r.Note) / movieReviews.Count;

			for (var i = 0; i< movieReviews.Count; i++) 
			{
				var diff = movieReviews[i].Note - meanMovie;
				sum += Math.Pow (diff, 2);
			}

			var varMovie = (float)sum / movieReviews.Count;

			return new Variance { Id = movie, Var = varMovie };
		}
	}
}


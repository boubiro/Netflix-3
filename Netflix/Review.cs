using System;
using SQLite;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Linq;

namespace Netflix
{
	public interface IReview 
	{
		int MovieId { get; set; }
		int UserId { get; set; }
		DateTime Date { get; set; }
		int Note { get; set; }
	}

	[Table("Review")]
	public class Review : IReview
	{
		[Indexed]
		public int MovieId { get; set; }
		[Indexed]
		public int UserId { get; set; }

		public DateTime Date { get; set; }
				
		public int Note { get; set; }
		
		public override bool Equals (object obj)
		{
			return Equals(obj as Review);
		}
		
		private bool Equals(Review r)
		{
			if (r == null)
				return false;
			
			return r.MovieId == this.MovieId;
		}
	}

	[Table("Review")]
	public class NonIndexedReview : IReview
	{	
		public int MovieId { get; set; }
	
		public int UserId { get; set; }

		public DateTime Date { get; set; }

		public int Note { get; set; }
	}

//
//	public class ReviewDatabaseLayerAsync : SQLiteAsyncConnection
//	{
//		private const string DefaultPath = "reviews.db";
//
//		public ReviewDatabaseLayerAsync (string path = DefaultPath) : base(path)
//		{
//			CreateTableAsync<Review>();
//		}
//
//		public IEnumerable<Review> GetReviewsByUserId(int userId)
//		{
//			return Table<Review>().Where(review => review.UserId == userId).ToListAsync().Result;
//		}
//
//		public IEnumerable<Review> GetReviewsByMovieId(int movie)
//		{
//			return Table<Review>().Where(review => review.MovieId == movie).ToListAsync().Result;
//		}
//
//		public void CleanTable()
//		{
//			DropTableAsync<Review>().ContinueWith(task => CreateTableAsync<Review>());
//		}
//
//		public void Save (IEnumerable<Review> reviews)
//		{
//			var conn = GetConnection ();
//			using (conn.Lock ()) 
//			{
//				conn.InsertAll (reviews);
//			}
//
//			//InsertAllAsync(reviews);
//		}
//
//		public void Save (Review review)
//		{
//			InsertAsync(review);
//		}
//	}

	public class ReviewDatabaseLayer<T> : SQLiteConnection
		where T : IReview, new()
	{
		private const string DefaultPath = "reviews.db";

		private const string IntersectMoviesForUsersQuery = "select MovieId from Review where UserId=? and MovieId in (select MovieId from Review where UserId=?)";
		private const string IntersectUsersForMoviesQuery = "select UserId from Review where MovieId=? and UserId in (select UserId from Review where MovieId=?)";

		public ReviewDatabaseLayer (string path = DefaultPath) : base(path)
		{
			CreateTable<T>();
		}

		public IEnumerable<T> GetReviewsByUserId(int userId)
		{
			return Table<T>().Where(review => review.UserId == userId);
		}

		public IEnumerable<T> GetReviewsByMovieId(int movie)
		{
			return Table<T>().Where(review => review.MovieId == movie);
		}

		public void CleanTable()
		{
			DeleteAll<T>();
		}

		public void Save (IEnumerable<T> reviews)
		{
			InsertAll(reviews);
		}

		public void Save (T review)
		{
			Insert(review);
		}

		#region Intersection

		public IEnumerable<int> GetCommonMovieIds(int user1, int user2)
		{
			return Query<int> (IntersectMoviesForUsersQuery, user1, user2);
		}

		public IEnumerable<int> GetCommonUserIds(int movie1, int movie2)
		{
			return Query<int> (IntersectUsersForMoviesQuery, movie1, movie2);
		}

		public IEnumerable<Tuple<T, T>> GetCommonUsers(int movie1, int movie2)
		{
			var reviews1 = GetReviewsByMovieId (movie1);
			var reviews2 = GetReviewsByMovieId(movie2);

			return GetCommonReviews (reviews1, reviews2, (r, r2) => r.UserId == r2.UserId);
		}

		public IEnumerable<Tuple<T, T>> GetCommonMovies(int user1, int user2)
		{
			var reviews1 = GetReviewsByUserId (user1).ToArray ();
			var reviews2 = GetReviewsByUserId (user2).ToArray ();

			return GetCommonReviews (reviews1, reviews2, (r, r2) => r.MovieId == r2.MovieId);
		}

		private IEnumerable<Tuple<T, T>> GetCommonReviews(IEnumerable<T> reviews1, IEnumerable<T> reviews2, Func<T, T, bool> compare)
		{
			foreach (var r in reviews1) 
			{
				var user2Review = reviews2.FirstOrDefault (r2 => compare(r, r2));
				if (user2Review != null) 
				{
					yield return new Tuple<T, T> (r, user2Review);
				}
			}
		}

		#endregion
	}
}


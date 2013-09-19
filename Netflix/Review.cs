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

		public IEnumerable<int> GetCommonMovies(int user1, int user2)
		{
			return Query<int> (IntersectMoviesForUsersQuery, user1, user2);
		}

		public IEnumerable<int> GetCommonUsers(int movie1, int movie2)
		{
			return Query<int> (IntersectUsersForMoviesQuery, movie1, movie2);
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
	}
}


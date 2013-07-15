using System;
using SQLite;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace Netflix
{
	public class Review
	{
		//[Indexed]
		public int MovieId { get; set; }
		//[Indexed]
		public int UserId { get; set; }

		public DateTime Date { get; set; }

		public int Note { get; set; }
	}

	public class ReviewDatabaseLayerAsync : SQLiteAsyncConnection
	{
		private const string DefaultPath = "reviews.db";

		public ReviewDatabaseLayerAsync (string path = DefaultPath) : base(path)
		{
			CreateTableAsync<Review>();
		}

		public IEnumerable<Review> GetReviewsByUserId(int userId)
		{
			return Table<Review>().Where(review => review.UserId == userId).ToListAsync().Result;
		}

		public IEnumerable<Review> GetReviewsByMovieId(int movie)
		{
			return Table<Review>().Where(review => review.MovieId == movie).ToListAsync().Result;
		}

		public void CleanTable()
		{
			DropTableAsync<Review>().ContinueWith(task => CreateTableAsync<Review>());
		}

		public void Save (IEnumerable<Review> reviews)
		{
			var conn = GetConnection ();
			using (conn.Lock ()) 
			{
				conn.InsertAll (reviews);
			}

			//InsertAllAsync(reviews);
		}

		public void Save (Review review)
		{
			InsertAsync(review);
		}
	}

	public class ReviewDatabaseLayer : SQLiteConnection
	{
		private const string DefaultPath = "reviews.db";

		public ReviewDatabaseLayer (string path = DefaultPath) : base(path)
		{
			CreateTable<Review>();
		}

		public IEnumerable<Review> GetReviewsByUserId(int userId)
		{
			return Table<Review>().Where(review => review.UserId == userId);
		}

		public IEnumerable<Review> GetReviewsByMovieId(int movie)
		{
			return Table<Review>().Where(review => review.MovieId == movie);
		}

		public void CleanTable()
		{
			DeleteAll<Review>();
		}

		public void Save (IEnumerable<Review> reviews)
		{
			InsertAll(reviews);
		}

		public void Save (Review review)
		{
			Insert(review);
		}
	}

	public class ReviewDatabaseLayerMono
	{
		public ReviewDatabaseLayerMono ()
		{
			//var conn = new SqliteConnection("/home/shareff/Dev/Mono/Netflix"

		}
	}
}


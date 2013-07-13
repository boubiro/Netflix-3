using System;
using SQLite;
using System.Collections.Generic;

namespace Netflix
{
	public class Movie
	{
		[Indexed]
		public int Id { get; set; }

		public string Title { get; set; }

		public int Date { get; set; }
	}

	public class MovieDatabaseLayer : SQLiteConnection
	{
		private const string DefaultPath = "movies.db";

		public MovieDatabaseLayer (string path = DefaultPath) : base(path)
		{
			CreateTable<Movie>();
		}

		public void Save (Movie movie)
		{
			Insert(movie);
		}

		public void Save (IEnumerable<Movie> movies)
		{
			InsertAll(movies);
		}

		public Movie GetMovie(int movieId)
		{
			return	(from s in Table<Movie> ()
					where s.Id == movieId
					select s).FirstOrDefault ();
		}

		public IEnumerable<Movie> GetAllMovies()
		{
			return Table<Movie>();
		}

		public void CleanTable()
		{
			DeleteAll<Movie>();
		}
	}
}


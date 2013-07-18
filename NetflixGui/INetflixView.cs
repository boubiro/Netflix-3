using System;

namespace NetflixGui
{
	public interface INetflixView
	{
		event EventHandler ImportMovies;
		event EventHandler CancelMoviesImportation;

		event EventHandler ImportReviews;
		event EventHandler CancelReviewsImportation;

		event EventHandler CreateScripts;

		event Action<int, int> ReviewQuery;

		/// <summary>
		/// Source file to import movies.
		/// </summary>
		string MovieSource { get; }

		/// <summary>
		/// Target database file.
		/// </summary>
		string MovieTarget { get; }

		/// <summary>
		/// Source directory to import reviews.
		/// </summary>
		string ReviewSource { get; }

		/// <summary>
		/// Target database file.
		/// </summary>
		string ReviewTarget { get; }

		/// <summary>
		/// When importing reviews : splits all review files into little groups.
		/// </summary>
		/// <value>
		/// The group of file size (default 200).
		/// </value>
		int ChunkSize { get; }

		/// <summary>
		/// Review filename contains the MovieId, you can specify the starting file.
		/// </summary>
		/// <value>
		/// Starting MovieId.
		/// </value>
		int StartFile { get; }

		void MoviesImported ();		
		void MovieProgress (int progress, string message);

		void ReviewsImported ();		
		void ReviewProgress (int progress, string message);

		void DisplayError(string message);

		void ClearReview();
		void SetReview (int movieId, int userId, DateTime date, int note);
	}
}


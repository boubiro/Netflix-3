using System;

namespace NetflixGui
{
	public interface INetflixView
	{
		event EventHandler ImportMovies;
		event EventHandler ImportReviews;
		event EventHandler CreateScripts;

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
	}
}


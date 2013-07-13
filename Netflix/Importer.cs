using System;
using System.IO;
using Netflix;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;

namespace Netflix
{
	public class Importer
	{
		private const string DefaultReviewDirectory = "/home/shareff/Dev/Data/training_set";
		private const string DefaultMoviePath = "/home/shareff/Dev/Data/movie_titles.txt";

		#region Movies

		public static void ImportMovies (string path = DefaultMoviePath)
		{
			if (!string.IsNullOrEmpty(path))
			{
				if (File.Exists (path)) 
				{
					// On cleare tout avant d'importer
					var movieDb = new MovieDatabaseLayer();
					movieDb.CleanTable();

					var movies = new List<Movie>();
					using(var reader = File.OpenText(path))
					{
						string line;

						// on choppe tous les films un par un on stocke dans une liste temporaire
						while((line = reader.ReadLine()) != null)
						{
							var splits = line.Split(',');
							var movie = new Movie
							{
								Id = int.Parse(splits[0]),
								Title = splits[2]
							};

							int date;
							// y'a des Date NULL des fois, du coup on "try parse"
							if (int.TryParse(splits[1], out date))
							{
								movie.Date = date;
							}

							movies.Add(movie);
						}
					}

					// on save tout le bloc
					movieDb.Save(movies);
				}
				else
				{
					Logger.Error (string.Format("File {0} does not exist", path));
				}
			}
			else 
			{
				Logger.Error (string.Format("Je crois, c'est pas bon", path));
			}
		}

		#endregion

		#region Reviews

		private static long _reviewCounter;

		public static void ImportReviews(string directory = DefaultReviewDirectory)
		{
			if (!string.IsNullOrEmpty(directory))
			{
				if (Directory.Exists (directory)) 
				{
					// on crée l'objet qui gère la base
					var reviewDb = new ReviewDatabaseLayer();

					// on choppe tous les fichiers du dossier
					var files = Directory.GetFiles(directory);

					// On découpe la liste de fichier...
					var splits = files.Split(200);

					// On fait le boulot en parallèle, mais pour chaque groupe de fichiers
					foreach(var groupOfFiles in splits)
					{
					    var bag = new ConcurrentBag<Review>();
						Parallel.ForEach(groupOfFiles, currentFile =>
						{
							GetReview(currentFile, bag);
						});

						var watch = new Stopwatch();
						watch.Start();

						Logger.Info(string.Format("Saving {0} reviews", bag.Count));

						reviewDb.Save(bag);

						watch.Stop();
						Logger.Info(string.Format("Saved in {0} ms", watch.Elapsed));
					}

//					var counter = 0;
//					foreach(var file in files)
//					{
//						if (counter++ > 500)
//							return;
//
//						GetReview(file, reviewDb);
//					}
				}
				else
				{
					Logger.Error (string.Format("Directory {0} does not exist", directory));
				}
			}
			else 
			{
				Logger.Error ("Je crois, c'est pas bon");
			}
		}

		private static void GetReview (string path, ReviewDatabaseLayer reviewDb)
		{
			using (var reader = File.OpenText(path)) 
			{
				var line = reader.ReadLine();
				var movieId = int.Parse(line.Substring(0, line.Length - 1));

				GetReview(movieId, reader, reviewDb);
			}

			var count = Interlocked.Increment(ref _reviewCounter);

			Logger.Info(string.Format("{0} review done : {1}", count, path));
		}

		private static void GetReview (int movieId, StreamReader reader, ReviewDatabaseLayer reviewDb)
		{
			var watch = new Stopwatch ();
			watch.Start ();

			var counter = 0;

			var reviews = new List<Review> ();

			string line;
			while ((line = reader.ReadLine()) != null) 
			{
				var splits = line.Split (',');
				var review = new Review
				{
					MovieId = movieId,
					UserId = int.Parse(splits[0]),
					Note = int.Parse(splits[1]),
					Date = DateTime.Parse(splits[2])
				};

				reviews.Add (review);

				counter++;
			}
		}

		private static void GetReview (string path, ConcurrentBag<Review> bag)
		{
			using (var reader = File.OpenText(path)) 
			{
				var line = reader.ReadLine();
				var movieId = int.Parse(line.Substring(0, line.Length - 1));

				GetReview(movieId, reader, bag);
			}

			var count = Interlocked.Increment(ref _reviewCounter);

			Logger.Info(string.Format("{0} review done : {1}", count, path));
		}

		private static void GetReview (int movieId, StreamReader reader, ConcurrentBag<Review> bag)
		{
			var watch = new Stopwatch();
			watch.Start();

			var counter = 0;

			string line;
			while ((line = reader.ReadLine()) != null) 
			{
				var splits = line.Split(',');
				var review = new Review
				{
					MovieId = movieId,
					UserId = int.Parse(splits[0]),
					Note = int.Parse(splits[1]),
					Date = DateTime.Parse(splits[2])
				};

				bag.Add(review);

				counter++;
			}

			watch.Stop();
			Logger.Info(string.Format("Movie {0} imported, {1} reviews added in {2} ms ({3} in bag)", movieId, counter, watch.Elapsed.TotalMilliseconds, bag.Count));
		}

		#endregion

		#region Transform Reviews

		public static void TransformReviews (string directory = DefaultReviewDirectory)
		{
			if (!string.IsNullOrEmpty(directory))
			{
				if (Directory.Exists (directory)) 
				{
					// on choppe tous les fichiers du dossier
					var allFiles = Directory.GetFiles(directory);
					var files = new List<string>();

					foreach(var f in allFiles)
					{
						if (CheckFile(f))
						{
							files.Add(f);
						}
					}

					allFiles = null;

					// On découpe la liste de fichier...
					var splits = files.Split(50);

					// On fait le boulot en parallèle, mais pour chaque groupe de fichiers
					foreach(var groupOfFiles in splits)
					{
						var watch = new Stopwatch();
						watch.Start();

					    var bag = new ConcurrentBag<Review>();
						Parallel.ForEach(groupOfFiles, currentFile =>
						{
							GetReview(currentFile, bag);
						});

						CreateScript(bag);

						watch.Stop();
						Logger.Info(string.Format("Script create in {0} ms for {1} files", watch.Elapsed, _reviewCounter));

						if (_reviewCounter >= 100)
							return;
					}
				}
				else
				{
					Logger.Error (string.Format("Directory {0} does not exist", directory));
				}
			}
			else 
			{
				Logger.Error ("Je crois, c'est pas bon");
			}
		}

//		CREATE INDEX "Review_MovieId" on "Review"("MovieId");
//CREATE INDEX "Review_UserId" on "Review"("UserId");


		private static void CreateScript (ConcurrentBag<Review> bag)
		{
			var builder = new StringBuilder();
			
		    builder.AppendLine("begin transaction;");
			foreach (var review in bag) 
			{
				builder.Append("insert into Review (MovieId, UserId, Date, Note) values (").
					Append(review.MovieId).Append(", ").
					Append(review.UserId).Append(", ").
						Append("'").Append(review.Date.ToString ("yyyy-MM-dd HH:mm:ss")).Append("', ").
						Append(review.Note).AppendLine(");");			
			}

			builder.AppendLine("commit;");

			File.WriteAllText("script_" + _reviewCounter, builder.ToString());
		}

		private static bool CheckFile (string file)
		{
			var splits = Path.GetFileName(file).Split ('_', '.');
			int fileId;

			if (splits.Length > 1 && int.TryParse (splits [1], out fileId)) 
			{
				return fileId > 8800;
			}

			return false;
		}

		#endregion 
	}
}


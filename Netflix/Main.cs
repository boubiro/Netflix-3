using System;

namespace Netflix
{
	class NetflixReview
	{
		public static void Main (string[] args)
		{
			for (var i = 0; i < args.Length; i++) 
			{
				switch(args[i])
				{
					case "importMovies":
						if (i < args.Length - 1)
						{
							ImportMovies(args[++i]);
						}
						else
						{
							ImportMovies();
						}
					break;
					case "importReviews":
						if (i < args.Length - 1)
						{
							ImportReviews(args[++i]);
						}
						else
						{
							ImportReviews();
						}
					break;

				case "transform":
					if (i < args.Length - 1)
					{
						TransformReviews(args[++i]);
					}
					else
					{
						TransformReviews();
					}
					break;
					default:
					Usage ();
					break;
				}
			}
		}

		private static void Usage ()
		{
			Logger.Info("NetflixReview importMovies [path] / importReviews [directory]");
		}

		private static void ImportMovies()
		{
			Importer.ImportMovies();
		}

		private static void ImportMovies(string path)
		{
			Importer.ImportMovies(path);
		}

		private static void ImportReviews()
		{
			Importer.ImportReviews();
		}

		private static void ImportReviews(string path)
		{
			Importer.ImportReviews(path);
		}

		private static void TransformReviews(string path)
		{
			Importer.TransformReviews(path);
		}

		
		private static void TransformReviews()
		{
			Importer.TransformReviews();
		}
	}
}

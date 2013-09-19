using System;

namespace NetflixPrize
{
	class MainClass
	{
		private static string ReviewPath = "/home/shareff/Dev/Db/reviews.sqlite3";
		private static string MoviePath = "/home/shareff/Dev/Db/toto.db";
		private static string MeanPath = "/home/shareff/Dev/Db/mean.sqlite3";

		public static void Main (string[] args)
		{
			var calculator = new MovieMeanCalculator (ReviewPath, MoviePath, MeanPath);

			foreach (var mean in calculator.CalculateForAll()) 
			{
				Console.WriteLine ("{0}({1}) : {2}", mean.Title, mean.Id, mean.Mean);
			}
		}
	}
}

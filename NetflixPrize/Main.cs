using System;
using System.Threading;

namespace NetflixPrize
{
	class MainClass
	{
		private static string ReviewPath = "/home/shareff/Dev/Db/reviews.sqlite3";
		private static string MoviePath = "/home/shareff/Dev/Db/toto.db";
		private static string MeanPath = "/home/shareff/Dev/Db/mean.sqlite3";

		private static int _count;

		public static void Main (string[] args)
		{
			var calculator = new MovieMeanCalculator (ReviewPath, MoviePath, MeanPath);

			calculator.MeanCalculated += (Action<MovieMean>)(mean => 
			{
				Console.WriteLine ("{0}({1}) : {2} ({3} mean calculated)", mean.Title, mean.Id, mean.Mean, Interlocked.Increment(ref _count));
			});

			calculator.CalculateForAll ();
		}
	}
}

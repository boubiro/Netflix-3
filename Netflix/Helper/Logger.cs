using System;

namespace Netflix
{
	public static class Logger
	{
		public static void Error(string message)
		{
			WriteLine("Error", message);
		}

		public static void Info(string message)
		{
			WriteLine("Info", message);
		}


		private static void WriteLine(string header, string message)
		{
			Console.WriteLine(string.Format("{0} : {1}", header, message));
		}

		private static void Write(string header, string message)
		{
			Console.Write(string.Format("{0} : {1}", header, message));
		}
	}
}


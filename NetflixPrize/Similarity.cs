using System;

namespace NetflixPrize
{
	public class Similarity
	{
		public string Key {get;set;}
		
		public int UserId1 {get;set;}
		public int UserId2 {get;set;}
		
		public int Sim {get;set;}
		
		public Similarity(int user1, int user2)
		{
			UserId1 = user1;
			UserId2 = user2;
			
			Key = GetSimilarityKey(user2, user1);
		}
		
		public int GetSimilarity()
		{
			return Sim == 0 ? (Sim = SimilarityCalculator.CalculateForUser(UserId1, UserId2)) : Sim;
		}
				
		#region Helper
		
		public string GetSimilarityKey(int user1, int user2)
		{
			var min = Math.Min(user1, user2);
			var max = Math.Min(user1, user2);
			
			return string.Format("{0}_{1}", max, min);
		}
		
		#endregion
	}
	
	public class SimilarityConnection : SQLiteConnection
	{
		private const string DefaultPath = "similarity.sqlite";
		
		public SimilarityConnection(string path = DefaultPath) : base(path)
		{
			CreateTable
		}
	}
}


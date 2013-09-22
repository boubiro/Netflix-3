using System;
using SQLite;

namespace NetflixPrize
{
	public class Similarity
	{
		public string Key {get;set;}
		
		public int Id1 {get;set;}
		public int Id2 {get;set;}
		
		public float Sim {get;set;}
		
		public Similarity(int user1, int user2)
		{
			Id1 = user1;
			Id2 = user2;
			
			Key = GetSimilarityKey(user2, user1);
		}

		public virtual string GetSimilarityKey(int id1, int id2)
		{
			var min = Math.Min (id1, id2);
			var max = Math.Min (id1, id2);

			return string.Format ("{0}_{1}", max, min);
		}
	}

//	public class UserSimilarity : Similarity
//	{
//		public UserSimilarity(int user1, int user2) : base(user1, user2){}
//	}
//
//	public class MovieSimilarity : Similarity
//	{
//		public MovieSimilarity(int movie1, int movie2) : base(movie1, movie2){}
//	}

	public class SimilarityConnection<Similarity> : SQLiteConnection
	{		
		public SimilarityConnection(string path) : base(path)
		{
			CreateTable<Similarity> ();
		}
	}
}

 
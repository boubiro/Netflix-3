using System;
using SQLite;

namespace NetflixPrize
{
	public class Variance
	{
		[Indexed]
		public int Id {get;set;}
		public float Var {get;set;}
	}

	public class VarianceConnection : SQLiteConnection
	{
		public VarianceConnection(string path) : base(path)
		{
			CreateTable<Variance> ();
		}

		public void Save(Variance variance)
		{
			Insert (variance);
		}

		public Variance Get(int id)
		{
			return Table<Variance> ().Where (v => v.Id == id).FirstOrDefault();
		}
	}
}


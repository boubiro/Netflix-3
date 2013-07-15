using System;
using Gtk;
using NetflixGui;

public partial class MainWindow: Gtk.Window, INetflixView
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	#region INetflixView implementation

	public event EventHandler ImportMovies;

	public event EventHandler ImportReviews;

	public event EventHandler CreateScripts;

	public string MovieSource { get; set; }

	public string MovieTarget { get; set; }

	public string ReviewSource { get; set; }

	public string ReviewTarget { get; set; }

	public int ChunkSize { get; set; }

	public int StartFile {get;set;}

	#endregion


}

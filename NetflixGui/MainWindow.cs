using System;
using Gtk;
using NetflixGui;

public partial class MainWindow: Gtk.Window, INetflixView
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		new NetflixPresenter(this);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	#region INetflixView implementation

	public event EventHandler ImportMovies;
	public event EventHandler CancelMoviesImportation;

	public event EventHandler ImportReviews;
	public event EventHandler CancelReviewsImportation;

	public event EventHandler CreateScripts;

	#region Properties

	public string MovieSource 
	{
		get 
		{
			return filechooserMovieSource.Filename; 
		}
	}

	public string MovieTarget 
	{
		get 
		{
			return movieTargetEntry.Text; 
		} 
	}

	public string ReviewSource 
	{
		get 
		{
			return filechooserReviewSource.CurrentFolder; 
		} 
	}

	public string ReviewTarget
	{ 
		get
		{
			return reviewTargetEntry.Text; 
		} 
	}

	public int ChunkSize 
	{
		get
		{
			int chunk;
			if (int.TryParse (chunkSizeEntry.Text, out chunk)) 
			{
				return chunk;
			}

			return 200;
		}
	}

	public int StartFile 
	{ 
		get 
		{
			int start;
			if (int.TryParse(startFileEntry1.Text, out start))
			{
				return start;
			}

			return 0;
		} 
	}

	#endregion

	public void MoviesImported ()
	{
	}

	public void MovieProgress (int progress, string message)
	{
		//movieProgressbar.Adjustment = progress;
	}

	public void ReviewsImported ()
	{
	}

	public void ReviewProgress (int progress, string message)
	{
	}

	public void DisplayError (string message)
	{
	}

	#endregion

	#region Movie callbacks
	
	private void OnMovieImportButtonClicked (object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty (MovieSource) && !string.IsNullOrEmpty (MovieTarget)) 
		{
			if (ImportMovies != null) 
			{
				ImportMovies (this, EventArgs.Empty);
			}
		}
	}

	private void OnMovieCancelButtonClicked (object sender, EventArgs e)
	{
		if (CancelMoviesImportation != null) 
		{
			CancelMoviesImportation(this, EventArgs.Empty);
		}
	}

	private void OnMovieTargetBrowseButtonClicked (object sender, EventArgs e)
	{
		OpenFileChooser(filename => movieTargetEntry.Text = filename);
	}

	#endregion

	#region Review callbacks

	private void OnReviewImportButtonClicked (object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty (MovieTarget) && !string.IsNullOrEmpty (MovieTarget)) 
		{
			if (CancelReviewsImportation != null) 
			{
				CancelReviewsImportation (this, EventArgs.Empty);
			}
		}
	}

	private void OnReviewCancelButtonClicked (object sender, EventArgs e)
	{
		if (ImportReviews != null) 
		{
			ImportReviews(this, EventArgs.Empty);
		}
	}

	private void OnReviewTargetBrowseButtonClicked (object sender, EventArgs e)
	{
		OpenFileChooser(filename => reviewTargetEntry.Text = filename);
	}

	#endregion

	private void OpenFileChooser(Action<string> action)
	{
		var fileChooser = new FileChooserDialog ("Sélectionner un fichier",
		                            this,
		                            FileChooserAction.Save,
		                            "Cancel", ResponseType.Cancel,
		                            "Choose", ResponseType.Accept);

		if (fileChooser.Run () == (int)ResponseType.Accept) 
		{			
			if (!string.IsNullOrEmpty (fileChooser.Filename)) 
			{
				action(fileChooser.Filename);
			}
		}

		fileChooser.Destroy();
	}
}

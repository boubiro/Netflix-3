using System;
using Gtk;
using NetflixGui;

public partial class MainWindow: Gtk.Window, INetflixView
{	
	private ListStore _model;

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();

		InitTreeview();

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

	public event Action<int, int> ReviewQuery;

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
			return filechooserReviewSource.Filename; 
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
		ReviewProgress(100, "Movies imported");
	}

	public void MovieProgress (int progress, string message)
	{
		Application.Invoke ((sender, arg) =>
		{
			movieProgressbar.Adjustment.Value = progress;
			movieStatusbar.Pop (1);
			movieStatusbar.Push (1, message);
		}
		);
	}

	public void ReviewsImported ()
	{
		ReviewProgress(100, "Reviews imported");
	}

	public void ReviewProgress (int progress, string message)
	{
		Application.Invoke ((sender, arg) =>
		{
			reviewProgressbar.Adjustment.Value = progress;
			reviewStatusbar.Pop (1);
			reviewStatusbar.Push (1, message);
		}
		);
	}

	public void DisplayError (string message)
	{
		var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, message);
	}

	private void AddColumn (string title, int place)
	{
		var column = new Gtk.TreeViewColumn ();
		var cell = new Gtk.CellRendererText ();
 		column.Title = title;
		column.PackStart (cell, true); 

		queryResultListview.AppendColumn(column);

		column.AddAttribute(cell, "text", place);
	}

	private void InitTreeview ()
	{
		// Prepare columns
		AddColumn("MovieId", 0);
		AddColumn("UserId", 1);
		AddColumn("Date", 2);
		AddColumn("Note", 3);

		// Model
		_model = new ListStore (typeof (string), typeof (string), typeof (string), typeof (string));
		queryResultListview.Model = _model;
 
	}

	public void SetReview (int movieId, int userId, DateTime date, int note)
	{
		Application.Invoke ((sender, args) =>
		{
			_model.AppendValues (movieId.ToString(), userId.ToString(), date.ToShortDateString (), note.ToString()); 
		});
	}

	public void ClearReview ()
	{
		_model.Clear ();
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
		if (!string.IsNullOrEmpty (ReviewTarget) && !string.IsNullOrEmpty (ReviewSource)) 
		{
			if (ImportReviews != null) 
			{
				ImportReviews (this, EventArgs.Empty);
			}
		}
	}

	private void OnReviewCancelButtonClicked (object sender, EventArgs e)
	{
		if (CancelReviewsImportation != null) 
		{
			CancelReviewsImportation(this, EventArgs.Empty);
		}
	}

	private void OnReviewTargetBrowseButtonClicked (object sender, EventArgs e)
	{
		OpenFileChooser(filename => reviewTargetEntry.Text = filename);
	}

	private void OnReviewTargetForQueryBrowseButtonClicked (object sender, EventArgs e)
	{
		OpenFileChooser(filename => reviewTargetForQueryEntry.Text = filename);
	}

	private void OnSelectButtonClicked (object sender, EventArgs e)
	{
		var dbFile = reviewTargetForQueryEntry.Text;
		if (!string.IsNullOrEmpty (dbFile)) 
		{
			if (ReviewQuery != null)
			{
				// j'ai pas fait pour l'user id encore...
				ReviewQuery(movieIdEntry.ValueAsInt, 0);
			}
		}

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

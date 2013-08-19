using System;
using Gtk;
using Netflix;
using NetflixPrize;

public partial class MainWindow: Gtk.Window
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

	protected void OnCalculateButtonClicked (object sender, EventArgs e)
	{
		float sim;
		if (userRadioButton.Active) 
		{
			var userSim = new UserSimilarity (int.Parse(id1Entry.Text), int.Parse(id2Entry.Text));
			sim = userSim.GetSimilarity (reviewTargetForQueryEntry.Text);

			// Save 
		}
		else 
		{
			
			var movieSim = new MovieSimilarity (int.Parse(id1Entry.Text), int.Parse(id2Entry.Text));
			sim = movieSim.GetSimilarity (reviewTargetForQueryEntry.Text);

			// Save 
		}

		resultTextbox.Text = sim.ToString ();;
	}
}

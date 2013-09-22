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
			var userSim = new Similarity (int.Parse(id1Entry.Text), int.Parse(id2Entry.Text));
			var calculator = new SimilarityCalculator (reviewTargetForQueryEntry.Text);
			sim = userSim.Sim = calculator.CalculateForUser (userSim.Id1, userSim.Id2);

			// Save 
		}
		else 
		{
			
			var movieSim = new Similarity (int.Parse(id1Entry.Text), int.Parse(id2Entry.Text));
			var calculator = new SimilarityCalculator (reviewTargetForQueryEntry.Text);
			sim = movieSim.Sim = calculator.CalculateForMovie (movieSim.Id1, movieSim.Id2);

			// Save 
		}

		resultTextbox.Text = sim.ToString ();;
	}
}

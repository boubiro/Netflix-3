using System;
using System.ComponentModel;
using Netflix;

namespace NetflixGui
{
	public class NetflixPresenter
	{
		private readonly INetflixView _view;

		private MovieImporter _movieImporter;
		private ReviewImporter _reviewImporter;

		public NetflixPresenter (INetflixView view)
		{
			_view = view;

			_view.ImportMovies += OnImportMovies;
			_view.CancelMoviesImportation += OnCancelMoviesImportation;

			_view.ImportReviews += OnImportReviews;
			_view.CancelReviewsImportation += OnCancelReviewsImportation;

			AppDomain.CurrentDomain.UnhandledException += (sender, e) => view.DisplayError(e.ExceptionObject.ToString());

		}

		#region Movies

		private void OnImportMovies (object sender, EventArgs e)
		{
			_movieImporter = new MovieImporter
				{
					Source = _view.MovieSource,
					Target = _view.MovieTarget
				};

			Run (_movieImporter, () => _view.MoviesImported(), (progress, message) => _view.MovieProgress(progress, message));
		}

		private void OnCancelMoviesImportation (object sender, EventArgs e)
		{
			_movieImporter.Cancel();
		}

		#endregion

		#region Reviews

		private void OnImportReviews (object sender, EventArgs e)
		{
			_reviewImporter = new ReviewImporter
			{
				Source = _view.ReviewSource,
				Target = _view.ReviewTarget,
				ChunkSize = _view.ChunkSize,
				StartFile = _view.StartFile
			};

			Run (_reviewImporter, () => _view.ReviewsImported(), (progress, message) => _view.ReviewProgress(progress, message));
		}

		private void OnCancelReviewsImportation (object sender, EventArgs e)
		{
			_reviewImporter.Cancel();
		}

		#endregion

		private void Run (AbstractImporter importer, Action whenFinished, Action<int, string> reportProgress)
		{
			var worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;

			importer.ReportProgress += (progress, message) =>
			{
				worker.ReportProgress(progress, message);
			};

			worker.ProgressChanged += (sender, e) => reportProgress(e.ProgressPercentage, e.UserState.ToString());

			worker.DoWork += (sender, e) => importer.Import();

			worker.RunWorkerCompleted += (sender, e) => 
			{
				if (e.Error != null)
				{
					_view.DisplayError(e.Error.Message);
				}
				else
				{
					if (whenFinished != null)
					{
						whenFinished();
					}
				}
			};

			worker.RunWorkerAsync();
		}
	}
}


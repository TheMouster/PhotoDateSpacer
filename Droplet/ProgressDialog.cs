using System;
using System.ComponentModel;
using System.Windows.Forms;
using ExifLibrary;
using System.Collections.Generic;

namespace PhotoDateSpacer
{
	public partial class ProgressDialog : Form
	{
		public ProgressDialog()
		{
			InitializeComponent();
		}

		public ProgressDialog(String[] filePaths) : this()
		{
			backgroundWorker.RunWorkerAsync(filePaths);
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			//Acquire the filePaths
			String[] filePaths = (String[])e.Argument;

			//Sort the files by name. Assumption is that the names are alpha-ordered.
			Array.Sort<String>(filePaths);

			//Convert the array to a list to make it easier to manipulate the contents.
			List<String> filePathsList = new List<String>(filePaths);

			//Get the first and last images' DateTimeDigitized values
			const Int32 FIRST = 0;
			Int32 LAST = filePathsList.Count - 1;

			ExifFile firstImage = ExifFile.Read(filePathsList[FIRST]);
			ExifFile lastImage = ExifFile.Read(filePathsList[LAST]);
			DateTime firstFileDigitisation, lastFileDigitisation, newFileDigitisation;
			firstFileDigitisation = (DateTime)firstImage.Properties[ExifTag.DateTimeDigitized].Value;
			lastFileDigitisation = (DateTime)lastImage.Properties[ExifTag.DateTimeDigitized].Value;
			newFileDigitisation = new DateTime(firstFileDigitisation.Ticks);

			//Determine the timespan between each image for the list.
			Int32 millisecondsBetweenFirstLast = Convert.ToInt32((lastFileDigitisation - firstFileDigitisation).TotalMilliseconds);
			Int32 millisecondsBetweenEachFile = millisecondsBetweenFirstLast / (filePathsList.Count - 1);

			//Remove the last and first images from the list, as were not going to process them.
			//These two files need to have the start & end date/times set.
			filePathsList.RemoveAt(filePathsList.Count - 1);
			filePathsList.RemoveAt(0);

			int numberProcessed = 1, progressPercentage = 0;
			
			foreach(var path in filePathsList)
			{
				if (backgroundWorker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}

				newFileDigitisation = newFileDigitisation.AddMilliseconds(millisecondsBetweenEachFile);

				PhotoProcessor.ProcessImage(path, newFileDigitisation);
				progressPercentage = (numberProcessed++ * 100) / filePathsList.Count;
				backgroundWorker.ReportProgress(progressPercentage);
			}
		}

		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.ValueFast(e.ProgressPercentage);
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close(); //Close the form. We're done.
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			backgroundWorker.CancelAsync();
		}
	}

	public static class ProgressBarExtensions
	{
		public static void ValueFast(this ProgressBar progressBar, int value)
		{
			if (value < progressBar.Maximum)    // prevent ArgumentException error on value = 100
			{
				progressBar.Value = value + 1;    // set the value +1
			}

			progressBar.Value = value;    // set the actual value

		}
	}
}

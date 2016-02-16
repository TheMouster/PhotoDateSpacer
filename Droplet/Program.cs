using System;
using System.Windows.Forms;

namespace PhotoDateSpacer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(String[] filePaths)
		{
			if (filePaths.Length < 3)
			{
				DisplayInstructions();
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ProgressDialog(filePaths));
		}

		/// <summary>
		/// Displays the instructions.
		/// </summary>
		private static void DisplayInstructions()
		{
			MessageBox.Show("Drop images with embedded EXIF tags on me. Where possible I'll update the images' DateTime, DateTimeOriginal, ThumbnailDateTime & File Creation Date to a date between the first and last images in the selection.");
		}
	}
}

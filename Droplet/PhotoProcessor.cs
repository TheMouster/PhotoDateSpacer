using System;
using System.IO;
using ExifLibrary;
using System.Collections.Generic;

namespace PhotoDateSpacer
{
	public static class PhotoProcessor
	{
		public static void ProcessImage(String filePath, DateTime newDateTime)
		{
			ExifFile image = ExifFile.Read(filePath);

			//Set the EXIF date/time values in the image and save it.
			try
			{
				image.Properties[ExifTag.DateTimeDigitized].Value = newDateTime;
			}
			catch (KeyNotFoundException) //No DateTimeDigitized? Add one.
			{
				var dateTime = new ExifDateTime(ExifTag.DateTimeDigitized, newDateTime);
				image.Properties.Add(ExifTag.DateTimeDigitized, dateTime);
			}

			try
			{
				image.Properties[ExifTag.DateTime].Value = newDateTime;
			}
			catch(KeyNotFoundException) //No DateTime? Add one.
			{
				var dateTime = new ExifDateTime(ExifTag.DateTime, newDateTime);
				image.Properties.Add(ExifTag.DateTime, dateTime);
			}

			try
			{
				image.Properties[ExifTag.DateTimeOriginal].Value = newDateTime;
			}
			catch (KeyNotFoundException) //No DateTimeOriginal? Add one.
			{
				var dateTimeOriginal = new ExifDateTime(ExifTag.DateTimeOriginal, newDateTime);
				image.Properties.Add(ExifTag.DateTimeOriginal, dateTimeOriginal);
			}

			try
			{
				image.Properties[ExifTag.ThumbnailDateTime].Value = newDateTime;
			}
			catch (KeyNotFoundException) //No ThumbnailDateTime? Add one.
			{
				var thumbnailDate = new ExifDateTime(ExifTag.ThumbnailDateTime, newDateTime);
				image.Properties.Add(ExifTag.ThumbnailDateTime, thumbnailDate);
			}

			image.Save(filePath);
			
			//Update the images' file creation time.
			File.SetCreationTime(filePath, newDateTime);
		}
	}
}

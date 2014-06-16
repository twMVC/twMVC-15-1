// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ImageHandlerBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Kuick.Data;

namespace Kuick.Web
{
	/// <summary>
	/// QueryStringParameter :
	/// w   = width in pixel
	/// h   = height in pixel
	/// cut = cut image to fit size <seealso cref="ResizeWay"/>
	/// q   = jpeg quility (30 to 100)
	/// </summary>
	public abstract class ImageHandlerBase : HandlerBase
	{
		[RequestParameter("w", "Image width in pixel", RequestType.Query)]
		[ValidationString(BoundaryString.IsMatch, true, @"^\d+$")]
		public string Width { get; set; }

		[RequestParameter("h", "Image height in pixel", RequestType.Query)]
		[ValidationString(BoundaryString.IsMatch, true, @"^\d+$")]
		public string Height { get; set; }

		[RequestParameter("cut", "cut image to fit size", RequestType.Query)]
		[ValidationString(BoundaryString.IsMatch, true, @"^\d$")]
		public string Cut { get; set; }

		[RequestParameter("q", "jpeg quility (30 to 100)", RequestType.Query)]
		[ValidationString(BoundaryString.IsMatch, true, @"^\d{2,3}$")]
		public string Quility { get; set; }

		/// <summary>
		/// The file set to be never expire and give the LastModified date time 
		/// in header by default.
		/// you can override it with your own cache algorithm.
		/// </summary>
		/// <param name="file">the image file(full path) to be process</param>
		/// <returns>cached or not</returns>
		protected virtual bool ProcessCache(string file)
		{
			Response.Cache.SetExpires(DateTime.MaxValue);
			DateTime fileTime = File.GetLastWriteTime(file);
			Response.Cache.SetLastModified(fileTime);

			DateTime since = DateTime.Now;
			if(DateTime.TryParse(Request.Headers["If-Modified-Since"], out since)) {
				if(since.ToString("yyyyMMddHHmm").Equals(fileTime.ToString("yyyyMMddHHmm"))) {
					Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// If GetFullPathFileName() is not an exist file, use this image instead.
		/// return null or string.Empty means not to use default image.
		/// </summary>
		protected virtual string DefaultImageFullPathFileName
		{
			get
			{
				return null;
			}
		}

		public override void OnProcessRequest(HttpContext ctx)
		{
			string file = GetFullPathFileName();
			string ext = Path.GetExtension(file);

			Logger.Track(
				"ImageHandlerBase.OnProcessRequest",
				new Any("file", file)
			);

			try {
				if(!File.Exists(file)) {
					Logger.Track(
						"ImageHandlerBase.OnProcessRequest",
						"file does not exists.",
						new Any("Request.RawUrl", ctx.Request.RawUrl),
						new Any("File", file)
					);
					file = DefaultImageFullPathFileName;
					if(String.IsNullOrEmpty(file) || !File.Exists(file)) {
						throw new HttpException(
							(int)System.Net.HttpStatusCode.NotFound,
							string.Format(
								"NOT FOUND. FileName is [{0}]",
								file
							)
						);
					}
				}
			} catch(HttpException) {
				throw;
			} catch(Exception ex) {
				Logger.Track(
					"ImageHandlerBase.OnProcessRequest",
					"Exception occured when execute File.Exists check.",
					new Any("Message", ex.Message),
					new Any("File", file)
				);
				throw;
			}

			bool isPng = Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase);
			Response.ContentType = isPng ? "image/png" : "image/jpeg";

			if(ProcessCache(file)) { return; }

			int width = Formator.AirBagToInt(Width, 0);
			int height = Formator.AirBagToInt(Height, 0);
			ResizeWay cut = (ResizeWay)Formator.AirBagToInt(Cut, 0);
			long quality = Formator.AirBagToLong(Quility, 100L);
			if(quality > 100L) { quality = 100L; }
			if(quality < 30L) { quality = 30L; }

			using(Bitmap bmp = ProcessImageFile(file, width, height, cut)) {
				EncoderParameters eps = new EncoderParameters(1);
				eps.Param[0] = new EncoderParameter(Encoder.Quality, quality);

				if(isPng) {
					using(MemoryStream ms = new MemoryStream()) {
						bmp.Save(ms, ImageFormat.Png);
						byte[] buffer = ms.ToArray();
						Response.BinaryWrite(buffer);
						Response.Flush();
					}
				} else {
					bmp.Save(Response.OutputStream, GetEncoderInfo(Response.ContentType), eps);
				}
			}
		}
		protected abstract string GetFullPathFileName();

		private static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for(j = encoders.Length - 1; j > -1; j--) {
				if(encoders[j].MimeType.Equals(mimeType)) { return encoders[j]; }
			}
			return null;
		}

		protected Bitmap ProcessImageFile(
			string file,
			int width,
			int height,
			ResizeWay resizeWay)
		{
			if(!Carrier.LoadImage(file)) {
				throw new FieldAccessException(String.Format(
					"File({0}) not found or unauthorized Access",
					file
				));
			}

			if(width <= 0 && height <= 0) {
				// original size
				return new Bitmap(Carrier.ShadowImage);
			} else if(height <= 0) {
				// fit width
				if(Carrier.ResizeShadow(
					((float)width / Carrier.ShadowImage.Width),
					InterpolationMode.HighQualityBicubic)) {
					return new Bitmap(Carrier.ShadowImage);
				}
			} else if(width <= 0) {
				// fit height
				if(Carrier.ResizeShadow(
					((float)height / Carrier.ShadowImage.Height),
					InterpolationMode.HighQualityBicubic)) {
					return new Bitmap(Carrier.ShadowImage);
				}
			} else {
				// fit width AND height
				Carrier.ResizeShadow(
					width,
					height,
					resizeWay,
					InterpolationMode.HighQualityBicubic
				);
				if(!Carrier.IsEmpty) { return new Bitmap(Carrier.ShadowImage); }
			}

			throw new NotImplementedException();
		}

		protected abstract IImageCarrier Carrier { get; }
	}
}

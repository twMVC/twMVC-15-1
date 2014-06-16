// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IImageCarrier.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Kuick.Web
{
	public interface IImageCarrier : IDisposable
	{
		#region Property
		Image ShadowImage { get; }
		string BasePath { get; set; }
		bool IsEmpty { get; }
		#endregion

		#region LoadImage
		bool LoadImage(string filename);
		bool LoadImage(string filename, int width, int height);
		bool LoadImage(Stream stream);
		bool LoadImage(Image image);
		#endregion

		#region Resize Shadow
		bool ResizeShadow(int width, int height, ResizeWay way, InterpolationMode mode);
		bool ResizeShadow(int width, int height, InterpolationMode mode);
		bool ResizeShadow(float percent, InterpolationMode mode);
		#endregion

		#region Get Thumbnail Image
		Image GetThumbnail(int width, int height, ThumbnailWay way);
		Image GetThumbnail(int width, int height);
		Image GetThumbnail(float percent);
		#endregion

		#region Cut Shadow
		bool CutShadow(int width, int height);
		bool CutShadow(Rectangle rectangle);
		#endregion

		#region Merge Shadow
		bool MergeShadow(string filename, int x, int y);
		bool MergeShadow(string filename, Matrix matrix, int x, int y, int scaleRate);
		bool MergeShadow(string filename, Matrix matrix, float x, float y, float scale);
		bool MergeShadow(Stream stream, int x, int y);
		bool MergeShadow(Stream stream, Matrix matrix, int x, int y, int scaleRate);
		bool MergeShadow(Stream stream, Matrix matrix, float x, float y, float scale);
		bool MergeShadow(Image sourceImage, int x, int y);
		bool MergeShadow(Image sourceImage, Matrix matrix, int x, int y, int scaleRate);
		bool MergeShadow(Image sourceImage, Matrix matrix, float x, float y, float scale);
		bool MergeShadowBorder(string borderPath);
		bool MergeShadowBorder(string borderPath, int width, int height);
		#endregion
	}
}

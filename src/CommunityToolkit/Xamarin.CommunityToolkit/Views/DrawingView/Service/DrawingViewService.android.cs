﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using Point = Xamarin.Forms.Point;

namespace Xamarin.CommunityToolkit.UI.Views
{
	static class DrawingViewService
	{
		public static Stream GetImageStream(IList<Point> points,
			Size imageSize,
			float lineWidth,
			Color strokeColor,
			Color backgroundColor)
		{
			if (points == null || points.Count < 2)
			{
				return Stream.Null;
			}

			var image = GetImageInternal(points, lineWidth, strokeColor, backgroundColor);
			if (image == null)
			{
				return Stream.Null;
			}

			var resizedImage = MaxResizeImage(image, (float)imageSize.Width, (float)imageSize.Height);
			using (resizedImage)
			{
				var stream = new MemoryStream();
				var compressResult = resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

				resizedImage.Recycle();

				if (!compressResult)
				{
					return null;
				}

				stream.Position = 0;
				return stream;
			}
		}

		static Bitmap GetImageInternal(IList<Point> points,
			float lineWidth,
			Color strokeColor,
			Color backgroundColor)
		{
			var minPointX = points.Min(p => p.X);
			var minPointY = points.Min(p => p.Y);
			var drawingWidth = points.Max(p => p.X) - minPointX;
			var drawingHeight = points.Max(p => p.Y) - minPointY;
			const int minSize = 1;
			if (drawingWidth < minSize || drawingHeight < minSize)
			{
				return null;
			}

			var image = Bitmap.CreateBitmap((int)drawingWidth, (int)drawingHeight, Bitmap.Config.Argb8888);
			using (var canvas = new Canvas(image))
			{
				// background
				canvas.DrawColor(backgroundColor.ToAndroid());

				// strokes
				using (var paint = new Paint())
				{
					paint.Color = strokeColor.ToAndroid();
					paint.StrokeWidth = lineWidth;
					paint.StrokeJoin = Paint.Join.Round;
					paint.StrokeCap = Paint.Cap.Round;
					paint.AntiAlias = true;
					paint.SetStyle(Paint.Style.Stroke);

					var pointsCount = points.Count;
					for (var i = 0; i < pointsCount - 1; i++)
					{
						var p1 = points.ElementAt(i);
						var p2 = points.ElementAt(i + 1);
						canvas.DrawLine((float)(p1.X - minPointX), (float)(p1.Y - minPointY), (float)(p2.X - minPointX),
										(float)(p2.Y - minPointY), paint);
					}
				}
			}

			return image;
		}

		static Bitmap MaxResizeImage(Bitmap sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = new Size(sourceImage.Width, sourceImage.Height);
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1)
			{
				return sourceImage;
			}

			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			return Bitmap.CreateScaledBitmap(sourceImage, (int)width, (int)height, false);
		}
	}
}
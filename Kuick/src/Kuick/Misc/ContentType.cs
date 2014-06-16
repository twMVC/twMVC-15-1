// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ContentType.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick
{
	public class ContentType
	{
		public const string FORM = "application/x-www-form-urlencoded";
		public const string DOC = "application/msword";
		public const string EXE = "application/octet-stream";
		public const string DLL = "application/octet-stream";
		public const string PDF = "application/pdf";
		public const string AI = "application/postscript";
		public const string EPS = "application/postscript";
		public const string PS = "application/postscript";
		public const string XLS = "application/vnd.ms-excel";
		public const string PPT = "application/vnd.ms-powerpoint";
		public const string JS = "application/x-javascript";
		public const string SWF = "application/x-shockwave-flash";
		public const string XHTML = "application/xhtml+xml";
		public const string XHT = "application/xhtml+xml";
		public const string ZIP = "application/zip";
		public const string MID = "audio/midi";
		public const string MIDI = "audio/midi";
		public const string MPGA = "audio/mpeg";
		public const string MP2 = "audio/mpeg";
		public const string MP3 = "audio/mpeg";
		public const string M3U = "audio/x-mpegurl";
		public const string RAM = "audio/x-pn-realaudio";
		public const string RM = "audio/x-pn-realaudio";
		public const string RA = "audio/x-realaudio";
		public const string WAV = "audio/x-wav";
		public const string BMP = "image/bmp";
		public const string GIF = "image/gif";
		public const string JPEG = "image/jpeg";
		public const string JPG = "image/jpeg";
		public const string JPE = "image/jpeg";
		public const string PNG = "image/png";
		public const string TIFF = "image/tiff";
		public const string TIF = "image/tiff";
		public const string CSS = "text/css";
		public const string HTML = "text/html";
		public const string HTM = "text/html";
		public const string TXT = "text/plain";
		public const string RTX = "text/richtext";
		public const string WML = "text/vnd.wap.wml";
		public const string WMLS = "text/vnd.wap.wmlscript";
		public const string XSL = "text/xml";
		public const string XML = "text/xml";
		public const string MPEG = "video/mpeg";
		public const string MPG = "video/mpeg";
		public const string MPE = "video/mpeg";
		public const string QT = "video/quicktime";
		public const string MOV = "video/quicktime";
		public const string AVI = "video/x-msvideo";
		public const string JSON = "application/json";

		public static string GetByFileExt(string fileExt)
		{
			if(Checker.IsNull(fileExt)) { return string.Empty; }
			if(!fileExt.StartsWith(".")) { fileExt = "." + fileExt; }

			switch(fileExt) {
				case Constants.File.Extension.AI:
					return ContentType.AI;
				case Constants.File.Extension.AVI:
					return ContentType.AVI;
				case Constants.File.Extension.BMP:
					return ContentType.BMP;
				case Constants.File.Extension.CSS:
					return ContentType.CSS;
				case Constants.File.Extension.DLL:
					return ContentType.DLL;
				case Constants.File.Extension.DOC:
					return ContentType.DOC;
				case Constants.File.Extension.EPS:
					return ContentType.EPS;
				case Constants.File.Extension.EXE:
					return ContentType.EXE;
				case Constants.File.Extension.GIF:
					return ContentType.GIF;
				case Constants.File.Extension.HTM:
					return ContentType.HTM;
				case Constants.File.Extension.HTML:
					return ContentType.HTML;
				case Constants.File.Extension.JPE:
					return ContentType.JPE;
				case Constants.File.Extension.JPEG:
					return ContentType.JPEG;
				case Constants.File.Extension.JPG:
					return ContentType.JPG;
				case Constants.File.Extension.JS:
					return ContentType.JS;
				case Constants.File.Extension.M3U:
					return ContentType.M3U;
				case Constants.File.Extension.MID:
					return ContentType.MID;
				case Constants.File.Extension.MIDI:
					return ContentType.MIDI;
				case Constants.File.Extension.MOV:
					return ContentType.MOV;
				case Constants.File.Extension.MP2:
					return ContentType.MP2;
				case Constants.File.Extension.MP3:
					return ContentType.MP3;
				case Constants.File.Extension.MPE:
					return ContentType.MPE;
				case Constants.File.Extension.MPEG:
					return ContentType.MPEG;
				case Constants.File.Extension.MPG:
					return ContentType.MPG;
				case Constants.File.Extension.MPGA:
					return ContentType.MPGA;
				case Constants.File.Extension.PDF:
					return ContentType.PDF;
				case Constants.File.Extension.PNG:
					return ContentType.PNG;
				case Constants.File.Extension.PPT:
					return ContentType.PPT;
				case Constants.File.Extension.PS:
					return ContentType.PS;
				case Constants.File.Extension.QT:
					return ContentType.QT;
				case Constants.File.Extension.RA:
					return ContentType.RA;
				case Constants.File.Extension.RAM:
					return ContentType.RAM;
				case Constants.File.Extension.RM:
					return ContentType.RM;
				case Constants.File.Extension.RTX:
					return ContentType.RTX;
				case Constants.File.Extension.SWF:
					return ContentType.SWF;
				case Constants.File.Extension.TIF:
					return ContentType.TIF;
				case Constants.File.Extension.TIFF:
					return ContentType.TIFF;
				case Constants.File.Extension.TXT:
					return ContentType.TXT;
				case Constants.File.Extension.WAV:
					return ContentType.WAV;
				case Constants.File.Extension.WML:
					return ContentType.WML;
				case Constants.File.Extension.WMLS:
					return ContentType.WMLS;
				case Constants.File.Extension.XHT:
					return ContentType.XHT;
				case Constants.File.Extension.XHTML:
					return ContentType.XHTML;
				case Constants.File.Extension.XLS:
					return ContentType.XLS;
				case Constants.File.Extension.XML:
					return ContentType.XML;
				case Constants.File.Extension.XSL:
					return ContentType.XSL;
				case Constants.File.Extension.ZIP:
					return ContentType.ZIP;
				default:
					return string.Empty;
			}
		}
	}
}

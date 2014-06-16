// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebPageCollector.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-10 - Creation


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace Kuick.Web
{
	public class WebPageCollector
	{
		public WebPageCollector()
		{
		}

		private Action<DirectoryInfo, Type, string> _Collector;
		public Action<DirectoryInfo, Type, string> Collector
		{
			get
			{
				if(null == _Collector) {
					return (dir, pageType, relativeUrl) => { };
				}
				return _Collector;
			}
			set
			{
				_Collector = value;
			}
		}

		public void Collect()
		{
			Collect("~/");
		}

		private void Collect(string url)
		{
			string path = HttpContext.Current.Server.MapPath(url);
			DirectoryInfo dir = new DirectoryInfo(path);
			string[] files = Directory.GetFiles(path, "*.aspx");
			foreach(string file in files) {
				string relativeUrl = string.Concat(url, Path.GetFileName(file));
				Type type = BuildManager.GetCompiledType(relativeUrl);
				Collector(dir, type.BaseType, relativeUrl);
			}

			string[] folders = Directory.GetDirectories(path);
			foreach(string subFolder in folders) {
				string folderName = subFolder.Substring(
					subFolder.LastIndexOf("\\") + 1
				);
				if(folderName.In(
					"bin", "css", "js", "img", "image", "images", 
					"log", "setting", "CKEditor")) {
					continue;
				}

				Collect(string.Concat(url, folderName, "/"));
			}
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Disker.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.IO;

namespace Kuick
{
	public class Disker
	{
		/// <summary>
		/// Create a directory.
		/// </summary>
		/// <param name="path"></param>
		public static void CreateFolder(string path)
		{
			if(!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		}

		/// <summary>
		/// Copy file or directory.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="overwrite"></param>
		public static void Copy(string from, string to, bool overwrite = true)
		{
			if(File.Exists(from)) {
				if(Directory.Exists(to)) {
					to = Path.Combine(to, Path.GetFileName(from));
				} else {
					CreateFolder(Path.GetDirectoryName(to));
					if(String.IsNullOrEmpty(Path.GetFileName(to))) {
						to = Path.Combine(to, Path.GetFileName(from));
					}
					var fi = new FileInfo(to);
					if(fi.Exists) { fi.IsReadOnly = false; }
				}
				File.Copy(from, to, overwrite);
			} else if(Directory.Exists(from)) {
				if(!from.EndsWith(Path.DirectorySeparatorChar.ToString())) {
					from += Path.DirectorySeparatorChar;
				}
				if(to.StartsWith(from)) {
					throw new ArgumentException("Can't copy to self.");
				}
				if(!Directory.Exists(to)) { Directory.CreateDirectory(to); }

				new List<string>(Directory.GetFiles(from))
					.ForEach(x => {
						var file = Path.GetFileName(x);
						Disker.Copy(x, Path.Combine(to, file), overwrite);
					});
				new List<string>(Directory.GetDirectories(from))
					.ForEach(x => {
						Disker.Copy(x, Path.Combine(to, x.Replace(from, "")), overwrite);
					});
			}
		}

		/// <summary>
		/// Delete file or directory.
		/// </summary>
		/// <param name="pathFile">directory or file path</param>
		public static void Delete(string pathFile, bool deleteReadOnly = true)
		{
			FileInfo fi = new FileInfo(pathFile);
			if(fi.Exists) {
				if(fi.IsReadOnly) {
					if(!deleteReadOnly) { return; }
					fi.IsReadOnly = false;
				}
				File.Delete(pathFile);
			} else if(Directory.Exists(pathFile)) {

				new List<string>(Directory.GetFiles(pathFile))
					.ForEach(x => Disker.Delete(x));

				new List<string>(Directory.GetDirectories(pathFile))
					.ForEach(x => Disker.Delete(x));

				Directory.Delete(pathFile);
			}
		}

		/// <summary>
		/// Get temp file name (full path)
		/// </summary>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static string GetTempFileName(string extension = null)
		{
			var s = Path.GetTempFileName();
			if(null != extension) {
				if(!extension.StartsWith(".")) { extension = "." + extension; }
				var f = Path.ChangeExtension(s, extension);
				File.Move(s, f);
				return f;
			}
			return s;
		}
	}
}

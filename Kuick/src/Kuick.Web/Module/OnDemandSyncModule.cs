// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OnDemandSyncModule.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.IO;
using System.Web;
using System.Collections.Generic;

namespace Kuick.Web
{
	public class OnDemandSyncModule : ModuleBase
	{
		/// <summary>
		/// Class name
		/// </summary>
		public static readonly string _Group = typeof(OnDemandSyncModule).Name;
		private static object _Locking = new object();
		//private Dictionary<string, string[]> Mappings = new Dictionary<string, string[]>();

		public override void Init(HttpApplication app)
		{
			if(null == app) { return; }
			app.BeginRequest += new EventHandler(app_BeginRequest);
			base.Init(app);
		}

		private void app_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = sender as HttpApplication;
			if(null == app) { return; }
			Sync(app.Context, app.Request.AppRelativeCurrentExecutionFilePath);
		}

		private static string ToFull(HttpServerUtility server, string file)
		{
			return file.StartsWith("~") || file.StartsWith("/")
				? null == server 
					? file 
					: server.MapPath(file)
				: file;
		}

		/// <summary>
		/// Sync file
		/// </summary>
		/// <param name="file">File to sync</param>
		public static void Sync(HttpContext context, string file)
		{
			try {
				var server = context.Server;
				string physicalFilePath = ToFull(server, file);

				Logger.Track(
					_Group,
					"OnDemandSyncModule Sync...",
					new Any("virtual file path", file),
					new Any("physical file path", physicalFilePath)
				);

				if(File.Exists(physicalFilePath)) {
					Logger.Track(_Group, "File already exists.");
					return;
				}

				string ext = Path.GetExtension(physicalFilePath);
				if(String.IsNullOrEmpty(ext)) {
					Logger.Track(_Group, "File Extension is Null Or Empty.");
					return;
				}

				List<string> extList = WebCurrent.Application.GetAllString(_Group, "Extension");
				if(!WebChecker.IsNull(extList) && !extList.Contains(ext)) {
					Logger.Track(_Group, "skip sync");
					return;
				}

				List<string> settingsList = WebCurrent.Application.GetAllString(_Group, "Source");
				foreach(string setting in settingsList) {
					List<string> sourceList = new List<string>(setting.Split('|'));
					if(WebChecker.IsNull(sourceList)) { continue; }

					var targetFilePath = ToFull(server, sourceList[0]);
					Logger.Track(
						_Group,
						"OnDemandSyncModule Sync...",
						new Any("config file path", sourceList[0]),
						new Any("target file path", targetFilePath)
					);
					if(!physicalFilePath.StartsWith(
						targetFilePath,
						StringComparison.OrdinalIgnoreCase)) {
						Logger.Track(
							_Group,
							"not match",
							new Any("From", targetFilePath),
							new Any("To", physicalFilePath)
						);
						continue;
					}

					sourceList.RemoveAt(0);

					var p = physicalFilePath.Substring(targetFilePath.Length);

					foreach(var path in sourceList) {
						var f = Path.Combine(path, p);
						Logger.Track(
							_Group,
							"OnDemandSyncModule Sync...",
							new Any("target file path name", f)
						);
						if(!File.Exists(f)) {
							Logger.Track(
								_Group,
								"file does not exists in target path",
								new Any("From", f),
								new Any("To", physicalFilePath)
							);
							continue;
						}

						string folder = Path.GetDirectoryName(physicalFilePath);

						try {
							if(!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
							File.Copy(f, physicalFilePath);
							Logger.Track(
								_Group,
								"Sync file success",
								new Any("From", f),
								new Any("To", physicalFilePath)
							);
						} catch(Exception ex) {
							Logger.Error(
								_Group,
								"Sync file error",
								ex,
								new Any("From", f),
								new Any("To", physicalFilePath)
							);
						}
						return;
					}
				}
			} catch(Exception exception) {
				Logger.Error(
					_Group,
					"OnDemandSyncModule Sync...",
					exception.ToAny()
				);
				throw;
			}
		}
	}
}

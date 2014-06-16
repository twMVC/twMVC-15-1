// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Start.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Text;

namespace Kuick
{
	public class Start : IStart
	{
		private StreamWriter _Listener;

		#region IStart
		#region Start
		public void DoPreStart(object sender, EventArgs e)
		{
			Current.Setting.FolderPath = Heartbeat.CreateFolder(Current.Setting.Folder);
			Current.Log.FolderPath = Heartbeat.CreateFolder(Current.Log.Folder);

			// Logger
			Logger.Initialize(ref _Listener);

			Logger.Message(
				"Kernel",
				new Any("Mode", Current.Mode),
				new Any("AppId", Current.AppID),
				new Any("IsFrontEnd", Current.IsFrontEnd),
				new Any("Language", Current.Language),
				new Any("UserName", Current.UserName),
				new Any("OperatingSystemBits", Current.OperatingSystemBits),
				new Any("ProcessorBits", Current.ProcessorBits),
				new Any("OSVersion", Current.OSVersion),
				new Any("Setting.Enable", Current.Setting.Enable),
				new Any("Setting.Folder", Current.Setting.Folder),
				new Any("Setting.FolderPath", Current.Setting.FolderPath),
				new Any("Log.Enable", Current.Log.Enable),
				new Any("Log.Folder", Current.Log.Folder),
				new Any("Log.FolderPath", Current.Log.FolderPath),
				new Any("Log.RefreshSize", Current.Log.RefreshSize),
				new Any("Log.RefreshFrequency", Current.Log.RefreshFrequency)
			);

			Logger.Track(
				"Application",
				Current.Application.ConvertAll<Any>(x =>
					new Any(string.Concat(x.Group, ".", x.Name), x.Value)
				).ToArray()
			);
			Logger.Track(
				"Database",
				Current.Database.ConvertAll<Any>(x =>
					new Any(
						string.Concat(
							x.Vender,
							".",
							x.Name
						),
						x.ConnectionString
					)
				).ToArray()
			);

			// EventCache
			foreach(Assembly assembly in Reflector.Assemblies) {
				var types = Reflector.GatherByInterface<IEvent>(assembly);
				foreach(Type type in types) {
					if(type.IsAbstract) { continue; }
					var one = Reflector.CreateInstance(type) as IEvent;
					if(null == one) { continue; }
					EventCache.Add(one);
				}
			}

			// ErrorCache
			foreach(Assembly assembly in Reflector.Assemblies) {
				var types = Reflector.GatherByInterface<IError>(assembly);
				foreach(Type type in types) {
					if(type.IsAbstract) { continue; }
					var one = Reflector.CreateInstance(type) as IError;
					if(null == one) { continue; }
					ErrorCache.Add(one);
				}
			}
		}

		public void DoBuiltinStart(object sender, EventArgs e)
		{
			foreach(Assembly asm in Reflector.Assemblies) {
				var types = Reflector.GatherByInterface<IBuiltin>(asm);
				foreach(Type type in types) {
					var builtIn = Reflector.CreateInstance(type) as IBuiltin;
					if(null == builtIn || builtIn.IsNull) { continue; }
					builtIn.Initiate();
					Builtins.Add(builtIn);
				}
			}
		}

		public void DoPreDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoPostDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoPostStart(object sender, EventArgs e)
		{
			foreach(Assembly asm in Reflector.Assemblies) {
				var types = Reflector.GatherByInterface<IInitializer>(asm);
				foreach(Type type in types) {
					var initializer = Reflector.CreateInstance(type) as IInitializer;
					if(null == initializer) { continue; }
					initializer.Initiate();
				}
			}
		}
		#endregion

		#region Terminate
		public void DoBuiltinTerminate(object sender, EventArgs e)
		{
			var defaultServices = Builtins.AllDefault;
			while(defaultServices.MoveNext()) {
				defaultServices.Current.Terminate();
			}
			var customServices = Builtins.AllCustom;
			while(customServices.MoveNext()) {
				customServices.Current.Terminate();
			}
		}

		public void DoPostTerminate(object sender, EventArgs e)
		{
			Logger.Message("Logger Terminating.");
			Logger.StopLogger(ref _Listener);
		}
		#endregion
		#endregion
	}
}

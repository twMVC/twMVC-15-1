// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace Kuicker
{
	public class RunTime
	{
		private static object _Lock = new object();


		public static string CalleeFullName()
		{
			return CalleeFullName(1);
		}
		public static string CalleeFullName(int deep)
		{
			var stackTrace = new StackTrace();
			var method = stackTrace.GetFrame(deep).GetMethod();
			var className = method.ReflectedType.Name;
			var methodName = method.Name;

			var suffix = string.Empty;
			switch(method.MemberType) {
				case System.Reflection.MemberTypes.Constructor:
					suffix = "() -- Constructor";
					break;
				case System.Reflection.MemberTypes.Method:
					suffix = "() -- Method";
					break;
				case System.Reflection.MemberTypes.Property:
					suffix = " -- Property";
					break;
				case System.Reflection.MemberTypes.All:
				case System.Reflection.MemberTypes.Custom:
				case System.Reflection.MemberTypes.Event:
				case System.Reflection.MemberTypes.Field:
				case System.Reflection.MemberTypes.NestedType:
				case System.Reflection.MemberTypes.TypeInfo:
				default:
					break;
			}

			return string.Concat(className, ".", methodName, suffix);
		}

		#region property
		public static string UserDomainName
		{
			get
			{
				return Environment.UserDomainName;
			}
		}

		public static string UserName
		{
			get
			{
				return Environment.UserName;
			}
		}

		public static string OperatingSystemBits
		{
			get
			{
				return Environment.Is64BitOperatingSystem ? "x64" : "x86";
			}
		}

		public static string ProcessorBits
		{
			get
			{
				return Environment.Is64BitProcess ? "x64" : "x86";
			}
		}

		public static string OSVersion
		{
			get
			{
				return Environment.OSVersion.VersionString;
			}
		}

		public static bool IsWebApp
		{
			get
			{
				return HostingEnvironment.IsHosted;
			}
		}

		private static string _BinFolder;
		public static string BinFolder
		{
			get
			{
				if(null == _BinFolder) {
					lock(_Lock) {
						if(null == _BinFolder) {
							_BinFolder = IsWebApp
								? Path.Combine(
									AppDomain.CurrentDomain.BaseDirectory,
									Constants.Folder.Bin
								)
								: AppDomain.CurrentDomain.BaseDirectory;
						}
					}
				}
				return _BinFolder;
			}
		}
		#endregion

		private static Int64 _NextIndex = 0;
		public static Int64 NextIndex()
		{
			lock(_Lock) {
				return _NextIndex++;
			}
		}

		public static string ToPhysicalPath(string path)
		{
			string physicalPath = path;
			try {
				if(!Path.IsPathRooted(path)) {
					if(IsWebApp) {
						physicalPath = HostingEnvironment.MapPath(path);
					} else {
						physicalPath = Path.Combine(
							AppDomain.CurrentDomain.BaseDirectory,
							path
						);
					}
				}
				return physicalPath;
			} catch(Exception ex) {
				//Logger.Error(
				//	CalleeFullName(),
				//	"Build physicalPath",
				//	ex.ToAnys(
				//		new Any("IsWebApp", IsWebApp),
				//		new Any("path", path),
				//		new Any("physicalPath", physicalPath)
				//	)
				//);
				throw;
			}
		}

		public static string CreateFolder(string path)
		{
			var physicalPath = ToPhysicalPath(path);

			try {
				if(!Directory.Exists(physicalPath)) {
					var info = Directory.CreateDirectory(physicalPath);
					if(null == info) {
						throw new Exception(String.Format(
							"Create folder failure ({0}).", physicalPath
						));
					}
				}
				return physicalPath;
			} catch(Exception ex) {
				//Logger.Error(
				//	CalleeFullName(), 
				//	"Create Directory",
				//	ex.ToAnys(
				//		new Any("path", path),
				//		new Any("physicalPath", physicalPath)
				//	)
				//);
				throw;
			}
		}
	}
}

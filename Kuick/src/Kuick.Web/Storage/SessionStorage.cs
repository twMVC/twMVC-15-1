// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SessionStorage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-11-20 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class SessionStorage : IStorage
	{
		private static object _Lock = new object();

		#region Singleton
		private static SessionStorage _Singleton;
		public static SessionStorage Singleton 
		{
			get
			{
				if(null == _Singleton) {
					lock(_Lock) {
						if(null == _Singleton) {
							_Singleton = new SessionStorage();
						}
					}
				}
				return _Singleton;
			}
		}
		#endregion

		#region IStorage
		public bool Exists(string name)
		{
			EnvironmentInspection();
			return HttpContext.Current.Session[Naming(name)] != null;
		}

		public string Read(string name)
		{
			EnvironmentInspection();
			return Formator.AirBag(
				HttpContext.Current.Session[Naming(name)] as string
			);
		}

		public void Write(string name, string value)
		{
			EnvironmentInspection();
			HttpContext.Current.Session[Naming(name)] = value;
		}

		public void Write(string name, object value)
		{
			EnvironmentInspection();
			HttpContext.Current.Session[Naming(name)] = value;
		}

		public void Clear(string name)
		{
			EnvironmentInspection();
			HttpContext.Current.Session.Remove(Naming(name));
		}

		public void EnvironmentInspection()
		{
			if(null == HttpContext.Current || null == HttpContext.Current.Session) {
				throw new PlatformNotSupportedException(
					"Web Session object is null reference!"
				);
			}
		}
		#endregion

		#region static
		public static Func<string, string> NameInterceptor { private get; set; }
		private static string Naming(string name)
		{
			return null == NameInterceptor ? name : NameInterceptor(name);
		}

		public static T Read<T>(string name)
		{
			try {
				return (T)HttpContext.Current.Session[Naming(name)];
			} catch {
				return default(T);
			}
		}

		public static void Write<T>(string name, T value)
		{
			try {
				HttpContext.Current.Session[Naming(name)] = value;
			} catch {
				return;
			}
		}
		#endregion
	}


	public class SessionStorage<T> 
		: SessionStorage where T : class, new()
	{
		public SessionStorage() { }

		private static string Name { get { return typeof(T).FullName; } }
		public bool Exists { get { return base.Exists(Name); } }

		public static T Read()
		{
			T one = Read<T>(Name);
			if(null == one) { one = new T(); }
			return one;
		}

		public void Write()
		{
			Write(Name, this);
		}

		public static void Clear()
		{
			SessionStorage.Singleton.Clear(Name);
		}
	}
}

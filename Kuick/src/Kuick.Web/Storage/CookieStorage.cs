// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CookieStorage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-11-20 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class CookieStorage : IStorage
	{
		#region IStorage
		public bool Exists(string name)
		{
			EnvironmentInspection();
			return CookieManager.Exists(name);
		}

		public string Read(string name)
		{
			EnvironmentInspection();
			return Formator.AirBag(CookieManager.Get(name));
		}

		public void Write(string name, string value)
		{
			EnvironmentInspection();
			CookieManager.Set(name, value);
		}

		public void Clear(string name)
		{
			EnvironmentInspection();
			CookieManager.Clear(name);
		}

		public void EnvironmentInspection()
		{
			if(null == HttpContext.Current || null == HttpContext.Current.Request) {
				throw new PlatformNotSupportedException(
					"Web Request object is null reference!"
				);
			}
		}
		#endregion
	}
}

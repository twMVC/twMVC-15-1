// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// AuthenticateService.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-13 - Creation


using System;
using Kuick.Web;
using System.Web;
using System.Web.SessionState;

namespace Kuick.Builtin.Security
{
	public class AuthenticateService : BuiltinBase, IAuthenticate
	{
		#region IAuthenticate
		public bool Signin(string userID, string password)
		{
			UserEntity user = UserEntity.Get(userID);
			if(null == user) { return false; }

			// User
			SessionStorage.Write<string>(Constants.Authentication.UserID, user.UserID);
			SessionStorage.Write<UserEntity>(Constants.Authentication.User, user);

			// Device
			DeviceEntity device = DeviceEntity.GetCurrentDevice();
			if(null != device) {
				SessionStorage.Write<string>(Constants.Client.DeviceID, device.DeviceID);
				SessionStorage.Write<DeviceEntity>(Constants.Client.Device, device);
			}

			return user.Signin(password).Success;
		}

		public bool Signout()
		{
			if(!Current.IsWebApplication){return true;}

			HttpSessionState session = HttpContext.Current.Session;
			HttpResponse response = HttpContext.Current.Response;
			if(null == session || null == response) { return true; }

			session.Abandon();
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
			response.Cache.SetNoStore();
			response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
			response.AppendHeader("Pragma", "no-cache");

			return true;
		}

		public IUser GetUser(string userID)
		{
			return UserEntity.Get(userID);
		}

		public IUser GetCurrentUser()
		{
			return SessionStorage.Read<UserEntity>(Constants.Authentication.User);
		}
		#endregion

		#region IBuiltin
		public override bool Default
		{
			get 
			{
				return true; 
			}
		}
		#endregion
	}
}

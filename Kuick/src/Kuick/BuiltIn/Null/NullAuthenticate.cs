// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullAuthenticate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10 - Creation


using System;

namespace Kuick
{
	public class NullAuthenticate : NullBuiltin, IAuthenticate
	{
		#region IAuthenticate
		public bool Signin(string userID, string password)
		{
			return true;
		}

		public bool Signout()
		{
			return true;
		}

		public IUser GetUser(string userID)
		{
			return null;
		}

		public IUser GetCurrentUser()
		{
			return null;
		}
		#endregion
	}
}

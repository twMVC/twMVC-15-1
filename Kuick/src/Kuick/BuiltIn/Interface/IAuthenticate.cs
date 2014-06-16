// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IAuthenticate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10


using System;
using System.Collections.Generic;

namespace Kuick
{
	public interface IAuthenticate : IBuiltin
	{
		bool Signin(string userID, string password);
		bool Signout();
		IUser GetUser(string userID);
		IUser GetCurrentUser();
	}
}

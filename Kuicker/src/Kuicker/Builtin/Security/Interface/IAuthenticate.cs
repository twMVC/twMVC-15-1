// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface IAuthenticate : IBuiltin
	{
		bool Signin(string userID, string password);
		bool Signout();
		IUser GetUser(string userID);
		IUser GetCurrentUser();
	}
}

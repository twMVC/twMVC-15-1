// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;

namespace Kuicker
{
	public interface IUser
	{
		string UserID { get; set; }
		string Name { get; set; }
		string Email { get; set; }
		string Password { get; set; }
		List<IRole> Roles { get; }

		Result Signin(string password);
	}
}

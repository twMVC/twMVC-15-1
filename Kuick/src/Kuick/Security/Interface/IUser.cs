// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IUser.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
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

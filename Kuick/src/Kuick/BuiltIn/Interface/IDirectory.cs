// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IDirectory.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;

namespace Kuick
{
	/// <summary>
	/// One directory for all applications.
	/// </summary>
	public interface IDirectory : IBuiltin
	{
		// User
		IUser GetUser(string userName);
		List<IUser> Users { get; }

		// Role
		IRole GetRole(string roleID);
		List<IRole> Roles { get; }
	}
}

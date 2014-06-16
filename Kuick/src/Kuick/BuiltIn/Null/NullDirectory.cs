// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullDirectory.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;

namespace Kuick
{
	public class NullDirectory : NullBuiltin, IDirectory
	{
		public List<IUser> Users
		{
			get
			{
				return new List<IUser>();
			}
		}

		public IUser GetUser(string userName)
		{
			return default(IUser);
		}

		public List<IRole> Roles
		{
			get
			{
				return new List<IRole>();
			}
		}

		public IRole GetRole(string roleID)
		{
			return default(IRole);
		}
	}
}

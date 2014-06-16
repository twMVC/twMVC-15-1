// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;

namespace Kuicker
{
	public interface IRole
	{
		string RoleID { get; set; }
		string Title { get; set; }
		List<IUser> Users { get; }
	}
}

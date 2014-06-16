// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IRole.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public interface IRole
	{
		string RoleID { get; set; }
		string Title { get; set; }
		List<IUser> Users { get; }
	}
}

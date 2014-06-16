// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface IAccessControl
	{
		string AccessControlID { get; set; }
		string RoleID { get; set; }
		string ResourceID { get; set; }
		PermissionAction Permission { get; set; }
		bool Permit { get; set; }

		IResource Resource { get; }
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IAccessControl.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation



namespace Kuick
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

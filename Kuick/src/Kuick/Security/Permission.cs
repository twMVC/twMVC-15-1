// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Permission.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation



namespace Kuick
{
	internal class Permission
	{
		private Permission(PermissionAction action)
		{
			this.Action = action;
		}

		public PermissionAction Action { get; private set; }

		public static Permission Set(PermissionAction action)
		{
			return new Permission(action);
		}

		// Verify
		public bool Verify(PermissionAction action)
		{
			return (Action & action) == action;
		}

		// Grant
		public void Grant(PermissionAction action)
		{
			Action = Action | action;
		}

		// Revoke
		public void Revoke(PermissionAction action)
		{
			Action = Action & (PermissionAction.Admin ^ action);
		}
	}
}

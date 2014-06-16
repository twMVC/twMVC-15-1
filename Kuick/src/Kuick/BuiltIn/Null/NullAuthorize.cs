// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullAuthorize.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10 - Creation


using System;

namespace Kuick
{
	public class NullAuthorize : NullBuiltin, IAuthorize
	{
		#region IAuthorize
		public IApp GetCurrentApp()
		{
			return null;
		}

		public bool RoleInApp(string roleID, string appID)
		{
			return true;
		}

		public bool RoleInFeature(string roleID, string featureID)
		{
			return true;
		}

		public bool RoleInFragment(string roleID, string fragmentID)
		{
			return true;
		}

		public void RoleGrantApp(string roleID, string appID)
		{
		}

		public void RoleGrantFeature(string roleID, string featureID)
		{
		}

		public void RoleGrantFragment(string roleID, string fragmentID)
		{
		}

		public void RoleRevokeApp(string roleID, string appID)
		{
		}

		public void RoleRevokeFeature(string roleID, string featureID)
		{
		}

		public void RoleRevokeFragment(string roleID, string fragmentID)
		{
		}
		#endregion
	}
}

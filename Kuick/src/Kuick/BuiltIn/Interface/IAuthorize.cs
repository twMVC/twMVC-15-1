// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IAuthorize.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10


using System;

namespace Kuick
{
	public interface IAuthorize : IBuiltin
	{
		IApp GetCurrentApp();

		// In
		bool RoleInApp(string roleID, string appID);
		bool RoleInFeature(string roleID, string featureID);
		bool RoleInFragment(string roleID, string fragmentID);

		// Grant
		void RoleGrantApp(string roleID, string appID);
		void RoleGrantFeature(string roleID, string featureID);
		void RoleGrantFragment(string roleID, string fragmentID);

		// Revoke
		void RoleRevokeApp(string roleID, string appID);
		void RoleRevokeFeature(string roleID, string featureID);
		void RoleRevokeFragment(string roleID, string fragmentID);
	}
}

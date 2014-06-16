// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface IAuthorize
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

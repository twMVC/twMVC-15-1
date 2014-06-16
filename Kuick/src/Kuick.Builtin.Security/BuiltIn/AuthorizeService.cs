// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// AuthorizeService.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-13 - Creation


using System;
using Kuick.Web;
using System.Web;
using System.Web.SessionState;
using Kuick.Data;

namespace Kuick.Builtin.Security
{
	public class AuthorizeService : BuiltinBase, IAuthorize
	{
		#region IAuthorize
		public IApp GetCurrentApp()
		{
			return AppEntity.Get(Current.AppID);
		}

		public bool RoleInApp(string roleID, string appID)
		{
			return DataCurrent.Mapping.Exists<RoleEntity, AppEntity>(
				roleID, appID
			);
		}

		public bool RoleInFeature(string roleID, string featureID)
		{
			return DataCurrent.Mapping.Exists<RoleEntity, FeatureEntity>(
				roleID, featureID
			);
		}

		public bool RoleInFragment(string roleID, string fragmentID)
		{
			return DataCurrent.Mapping.Exists<RoleEntity, FragmentEntity>(
				roleID, fragmentID
			);
		}

		public void RoleGrantApp(string roleID, string appID)
		{
			DataCurrent.Mapping.Add<RoleEntity, AppEntity>(
				roleID, appID
			);
		}

		public void RoleGrantFeature(string roleID, string featureID)
		{
			DataCurrent.Mapping.Add<RoleEntity, FeatureEntity>(
				roleID, featureID
			);
		}

		public void RoleGrantFragment(string roleID, string fragmentID)
		{
			DataCurrent.Mapping.Add<RoleEntity, FragmentEntity>(
				roleID, fragmentID
			);
		}

		public void RoleRevokeApp(string roleID, string appID)
		{
			DataCurrent.Mapping.Remove<RoleEntity, AppEntity>(
				roleID, appID
			);
		}

		public void RoleRevokeFeature(string roleID, string featureID)
		{
			DataCurrent.Mapping.Remove<RoleEntity, FeatureEntity>(
				roleID, featureID
			);
		}

		public void RoleRevokeFragment(string roleID, string fragmentID)
		{
			DataCurrent.Mapping.Remove<RoleEntity, FragmentEntity>(
				roleID, fragmentID
			);
		}
		#endregion

		#region IBuiltin
		public override bool Default
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}

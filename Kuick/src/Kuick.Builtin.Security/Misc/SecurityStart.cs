// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// SecurityStart.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.Collections.Generic;
using Kuick.Web;

namespace Kuick.Builtin.Security
{
	public class SecurityStart : IStart
	{
		public void DoPreStart(object sender, EventArgs e)
		{
		}

		public void DoBuiltinStart(object sender, EventArgs e)
		{
		}

		public void DoPreDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoPostDatabaseStart(object sender, EventArgs e)
		{
		}

		private static AppEntity _App;
		private static List<FeatureEntity> _Features = new List<FeatureEntity>();
		private static List<FragmentEntity> _Fragments = new List<FragmentEntity>();
		private static object _Lock = new object();
		public void DoPostStart(object sender, EventArgs e)
		{
			#region Kuick.Web: Authorizer.AppAuthorize
			Authorizer.AppAuthorize = () => {
				// App
				if(null == _App) {
					lock(_Lock) {
						if(null == _App) {
							_App = Builtins.Authorize.GetCurrentApp() as AppEntity;
							if(null == _App) { return false; }
						}
					}
				}

				// Skip
				if(!WebCurrent.Web.EnableAuthorize) { return true; }

				// User
				IUser user = Builtins.Authenticate.GetCurrentUser();
				if(null == user) { return false; }

				// Check
				List<IRole> roles = user.Roles;
				foreach(IRole role in roles) {
					if(Builtins.Authorize.RoleInApp(role.RoleID, Current.AppID)) {
						return true;
					}
				}
				return false;
			};
			#endregion

			#region Kuick.Web: Authorizer.PageAuthorize
			Authorizer.FeatureAuthorize = p => {
				if(null == p.PageAuthorize) { return true; }

				// App
				if(null == _App) {
					lock(_Lock) {
						if(null == _App) {
							_App = Builtins.Authorize.GetCurrentApp() as AppEntity;
							if(null == _App) { return false; }
						}
					}
				}

				// Feature
				FeatureEntity feature = _Features.FilterFirst(x=>
					x.AppID == _App.AppID 
					&
					x.Title == Formator.AirBagToString(
						p.PageAuthorize.Title,
						WebTools.GetCurrentAspx()
					)
					&
					x.RelativeUrl == WebTools.GetPathWithoutApplication()
				);
				if(null == feature) {
					lock(_Lock) {
						feature = _App.GetCurrentFeature(p) as FeatureEntity;
						if(null == feature) {
							feature = _App.GetCurrentFeature(p) as FeatureEntity;
							if(null == feature) {
								return false;
							} else {
								_Features.Add(feature);
							}
						} else {
							_Features.Add(feature);
						}
					}
				}

				// Skip
				if(!WebCurrent.Web.EnableAuthorize) { return true; }

				// User
				IUser user = Builtins.Authenticate.GetCurrentUser();

				if(null == user) { return false; }

				// Check
				List<IRole> roles = user.Roles;
				foreach(IRole role in roles) {
					if(Builtins.Authorize.RoleInFeature(role.RoleID, feature.FeatureID)) {
						return true;
					}
				}
				return false;
			};
			#endregion

			#region Kuick.Web: Authorizer.FragmentAuthorize
			Authorizer.FragmentAuthorize = (p, title, description) => {
				if(null == p.PageAuthorize) { return true; }
				if(string.IsNullOrEmpty(title)) { return true; }

				// App
				if(null == _App) {
					return false;
				}

				// Feature
				FeatureEntity feature = _Features.FilterFirst(x =>
					x.AppID == _App.AppID
					&
					x.Title == Formator.AirBagToString(
						p.PageAuthorize.Title,
						WebTools.GetCurrentAspx()
					)
					&
					x.RelativeUrl == WebTools.GetPathWithoutApplication()
				);
				if(null == feature) {
					return false;
				}

				// Fragment
				FragmentEntity fragment = _Fragments.FilterFirst(x =>
					x.AppID == _App.AppID
					&
					x.FeatureID == feature.FeatureID
					&
					x.Title == Formator.AirBagToString(
						p.PageAuthorize.Title,
						WebTools.GetCurrentAspx()
					)
				);
				if(null == fragment) {
					lock(_Lock) {
						fragment = feature
							.GetCurrentFragment(title, description) as FragmentEntity;
						if(null == fragment) {
							fragment = feature
								.GetCurrentFragment(title, description) as FragmentEntity;
							if(null == fragment) {
								return false;
							} else {
								_Fragments.Add(fragment);
							}
						} else {
							_Fragments.Add(fragment);
						}
					}
				}

				// Skip
				if(!WebCurrent.Web.EnableAuthorize) { return true; }

				// User
				IUser user = Builtins.Authenticate.GetCurrentUser();
				if(null == user) { return false; }

				// Check
				List<IRole> roles = user.Roles;
				foreach(IRole role in roles) {
					if(Builtins.Authorize.RoleInFragment(role.RoleID, fragment.FragmentID)) {
						return true;
					}
				}
				return false;
			};
			#endregion
		}

		public void DoBuiltinTerminate(object sender, EventArgs e)
		{
		}

		public void DoPostTerminate(object sender, EventArgs e)
		{
		}
	}
}

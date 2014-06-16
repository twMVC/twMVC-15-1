// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IdentityService.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-12-08 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using Kuick.Data;
using System.Threading;

namespace Kuick.Builtin.Identity
{
	public sealed class IdentityService : BuiltinBase, IIdentity
	{
		#region IBuiltin
		public override bool Default
		{
			get
			{
				return true;
			}
		}
		#endregion

		private static object _Locking = new object();

		public void Setting(
			string identityID, Frequency frequency, int start, int incremental)
		{
			if(DbSettingCache.Count == 0) { return; }

			IdentityEntity identity;
			if(!IdentityEntity.Exists(identityID)) {
				lock(_Locking) {
					if(!IdentityEntity.Exists(identityID)) {
						identity = IdentityEntity.Get(identityID);
						if(null == identity) {
							identity = new IdentityEntity();
							identity.IdentityID = identityID;
							identity.Frequency = frequency;
							identity.Start = start;
							identity.CurrentValue = start - incremental;
							identity.Incremental = incremental;
							DataResult result = identity.Add();
							Logger.MesssageOrError(
								"IdentityService.Setting",
								result,
								identity.ToAny().ToArray()
							);
						}
					}
				}
			}
		}

		public int Booking(string identityID)
		{
			if(DbSettingCache.Count == 0) { return -1; }

			int times = 10;

			lock(_Locking) {
				while(times-- > 0) {
					IdentityEntity identity = IdentityEntity.Get(identityID);
					if(null == identity) {
						Logger.Error(
							"IdentityService.Booking",
							"Can't find this setting.",
							new Any("IdentityID", identityID)
						);
						return -1;
					}

					if(Checker.IsMatch(identity.LastModifiedDate, identity.Frequency)) {
						identity.CurrentValue = 
							identity.CurrentValue + identity.Incremental;
					} else {
						identity.CurrentValue = identity.Start;
					}
					DataResult result = identity.Modify();
					if(result.Success) { return identity.CurrentValue; }

					Thread.Sleep(1000);
				}
			}

			Logger.Error(
				"IdentityService.Booking",
				"Have been tried 10 times still can not get the next increment value of an identity",
				new Any("IdentityID", identityID)
			);
			return -1;
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ErrorCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation


using System;

namespace Kuick
{
	public class ErrorCache : StaticCache<IError>
	{
		public static void Add(IError value)
		{
			if(Heartbeat.Singleton.PreDatabaseStartFinished) { return; }
			if(null == value) { return; }

			lock(_Lock) {
				if(!_Cache.ContainsKey(value.ErrorID)) {
					_Cache.Add(value.ErrorID, value);
				}
			}
		}

		public new static IError Get(string errorID)
		{
			IError error = _Cache[errorID];
			if(null == error) {
				throw new ApplicationException(string.Format(
					"Cannot find the Error of {0} in ErrorCache",
					errorID
				));
			}
			return error;
		}
	}
}

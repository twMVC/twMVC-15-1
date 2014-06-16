// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EventCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation


using System;

namespace Kuick
{
	public class EventCache : StaticCache<IEvent>
	{
		public static void Add(IEvent value)
		{
			if(Heartbeat.Singleton.PreDatabaseStartFinished) { return; }
			if(null == value) { return; }

			lock(_Lock) {
				if(!_Cache.ContainsKey(value.EventID)) {
					_Cache.Add(value.EventID, value);
				}
			}
		}

		public new static IEvent Get(string eventID)
		{
			IEvent _event = _Cache[eventID];
			if(null == _event) {
				throw new ApplicationException(string.Format(
					"Cannot find the Event of {0} in EventCache",
					eventID
				));
			}
			return _event;
		}
	}
}

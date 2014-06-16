// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Platform.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class Platform
	{
		private static Heartbeat _Heartbeat = null;

		public static void Start()
		{
			if(null == _Heartbeat) {
				_Heartbeat = Heartbeat.Singleton;
			}
		}

		public static void Terminate()
		{
			if(null != _Heartbeat) {
				_Heartbeat.Dispose();
				_Heartbeat = null;
			}
		}
	}
}

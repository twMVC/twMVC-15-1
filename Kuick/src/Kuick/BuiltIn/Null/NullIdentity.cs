// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullIdentity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;

namespace Kuick
{
	public class NullIdentity : NullBuiltin, IIdentity
	{
		public void Setting(string identityID, Frequency frequency, int start, int incremental)
		{
		}

		public int Booking(string identityID)
		{
			return -1;
		}
	}
}

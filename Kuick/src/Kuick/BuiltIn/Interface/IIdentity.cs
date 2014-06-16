// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IConfig.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;

namespace Kuick
{
	public interface IIdentity : IBuiltin
	{
		void Setting(string identityID, Frequency frequency, int start, int incremental);
		int Booking(string identityID);
	}
}

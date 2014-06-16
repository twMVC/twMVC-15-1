// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public interface IStart
	{
		// Start
		void DoPreStart(object sender, EventArgs e);
		void DoBuiltinStart(object sender, EventArgs e); // Api, Service
		void DoPreDatabaseStart(object sender, EventArgs e);
		void DoDatabaseStart(object sender, EventArgs e);
		void DoPostDatabaseStart(object sender, EventArgs e);
		void DoPostStart(object sender, EventArgs e);

		// Terminate
		void DoBuiltinTerminate(object sender, EventArgs e);
		void DoPostTerminate(object sender, EventArgs e);
	}
}

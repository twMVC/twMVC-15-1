// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// StartBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2014-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class StartBase : IStart
	{
		public virtual void DoPreStart(object sender, EventArgs e)
		{
		}

		public virtual void DoBuiltinStart(object sender, EventArgs e)
		{
		}

		public virtual void DoPreDatabaseStart(object sender, EventArgs e)
		{
		}

		public virtual void DoDatabaseStart(object sender, EventArgs e)
		{
		}

		public virtual void DoPostDatabaseStart(object sender, EventArgs e)
		{
		}

		public virtual void DoPostStart(object sender, EventArgs e)
		{
		}

		public virtual void DoBuiltinTerminate(object sender, EventArgs e)
		{
		}

		public virtual void DoPostTerminate(object sender, EventArgs e)
		{
		}
	}
}

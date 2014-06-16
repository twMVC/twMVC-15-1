// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KException.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class KException : Exception
	{
		public KException()
		{
		}

		public KException(string message)
			: base(message)
		{
		}

		public KException(string message, Result result)
			: base(message)
		{
			this.Result = result;
		}

		public Result Result { get; private set; }
	}
}

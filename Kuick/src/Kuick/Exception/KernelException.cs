// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KernelException.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class KernelException : KException
	{
		public KernelException()
			: base()
		{
		}

		public KernelException(string message)
			: base(message)
		{
		}

		public KernelException(string message, Result result)
			: base(message, result)
		{
		}
	}
}

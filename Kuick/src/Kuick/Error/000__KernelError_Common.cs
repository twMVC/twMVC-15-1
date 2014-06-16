// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// __KernelError_Common.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation



namespace Kuick
{
	public class __KernelError_Common : __KernelError
	{
		public override string Code
		{
			get { return "000"; }
		}

		protected override string DefaultCause
		{
			get { return "Not yet classified the cause of the error."; }
		}

		protected override string DefaultAction
		{
			get { return "Reaction to the system vendor for further assistance."; }
		}
	}
}

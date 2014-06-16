// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// __KernelError_HeartbeatStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation



namespace Kuick
{
	public class __KernelError_HeartbeatStart : __KernelError
	{
		public override string Code
		{
			get { return "001"; }
		}

		protected override string DefaultCause
		{
			get { return "Kernel start error occurred."; }
		}

		protected override string DefaultAction
		{
			get { return "Please refer to the system log file to understand the cause of the error."; }
		}
	}
}

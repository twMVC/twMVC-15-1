// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// __KernelError_ConfigName.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation



namespace Kuick
{
	public class __KernelError_ConfigName : __KernelError
	{
		public override string Code
		{
			get { return "002"; }
		}

		protected override string DefaultCause
		{
			get { return "The name of the config is not specified or is empty."; }
		}

		protected override string DefaultAction
		{
			get { return "Please give the proper name of the config."; }
		}
	}
}

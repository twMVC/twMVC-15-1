// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// __KernelError.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-22 - Creation



namespace Kuick
{
	public abstract class __KernelError : Error
	{
		protected override Scope Scope { get { return Kuick.Scope.Kernel; } }
	}
}

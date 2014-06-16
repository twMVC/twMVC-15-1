// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// __ServerError.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-06 - Creation



namespace Kuick
{
	public abstract class __ServerError : Error
	{
		protected override Scope Scope { get { return Kuick.Scope.Server; } }
	}
}

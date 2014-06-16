// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IError.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-19 - Creation



namespace Kuick
{
	public interface IError
	{
		//Scope Scope { get; }
		//string Namespace { get; }
		string ErrorID { get; }
		string Code { get; }
		string Cause { get; }
		string Action { get; }

		//string DefaultCause { get; }
		//string DefaultAction { get; }
	}
}

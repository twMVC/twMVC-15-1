// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IBuiltin.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation
// Kevin Jong       2012-03-18 - Rename from IService to IBuiltin


namespace Kuick
{
	public interface IBuiltin : IName, IIsNull
	{
		bool Default { get; }
		void Initiate();
		void Terminate();
	}
}

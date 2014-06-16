// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IStorage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-11-20 - Creation


using System;

namespace Kuick
{
	public interface IStorage
	{
		bool Exists(string name);
		string Read(string name);
		void Write(string name, string value);
		void Clear(string name);

		void EnvironmentInspection();
	}
}

// IBuiltin.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface IBuiltin
	{
		string Name { get; }

		void Start();
		void Suspend();
		void Resume();
		void Stop();


	}
}

// IPlugin.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface IPlugin
	{
		string Name { get; }

		void Start();
		void Suspend();
		void Resume();
		void Stop();
		
		// 
		void Shutdown();
	}
}

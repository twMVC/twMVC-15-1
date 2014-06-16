// ILifeCycle.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface ILifeCycle
	{
		// Start
		void DoBeforeStart();
		void DoBeforeBuiltinStart();
		void DoAfterBuiltinStart();
		void DoBeforePluginStart();
		void DoAfterPluginStart();
		void DoAfterStart();

		// Stop
		void DoBeforeStop();
		void DoAfterStop();
	}

	internal interface IKernelLifeCycle : ILifeCycle
	{
		// Start
		void DoBuiltinStart();
		void DoPluginStart();

		// Stop
		void DoPluginStop();
		void DoBuiltinStop();
	}
}

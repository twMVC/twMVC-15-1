// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Initializer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class Initializer : IInitializer
	{
		protected abstract void OnInit();

		#region IDataInitializer
		public void Initiate()
		{
			if(Heartbeat.Singleton.Stage == KernelStage.Starting) { 
				OnInit();
			}
		}
		#endregion
	}
}

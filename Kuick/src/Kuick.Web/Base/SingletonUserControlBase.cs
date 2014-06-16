// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SingletonUserControlBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick.Web
{
	/// <summary>
	/// The singleton user control in page scope.
	/// </summary>
	public abstract class SingletonUserControlBase : UserControlBase
	{
		public SingletonUserControlBase()
		{
			Init += new EventHandler(SingletonUserControlBase_Init);
		}

		private void SingletonUserControlBase_Init(object sender, EventArgs e)
		{
			Type Key = this.GetType();

			if(!Page.Items.Contains(Key)) {
				Page.Items.Add(Key, true);
			} else {
				throw new Exception("Only one control can be in page.");
			}
		}


	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KernelConfigException.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-31 - Creation


using System;

namespace Kuick
{
	public class KernelConfigException : ConfigException
	{
		public KernelConfigException(string name, string help)
			: base(ConfigScope.Kernel)
		{
			this.Attribute = string.Format("@name='{1}'", name);
			this.Help = help;
		}
	}
}

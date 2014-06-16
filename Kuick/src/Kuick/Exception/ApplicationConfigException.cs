// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ApplicationConfigException.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-31 - Creation


using System;

namespace Kuick
{
	public class ApplicationConfigException : ConfigException
	{
		public ApplicationConfigException(string group, string name, string help)
			: base(ConfigScope.Application)
		{
			this.Attribute = string.Format("@group='{0}' and @name='{1}'", group, name);
			this.Help = help;
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DatabaseConfigException.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-31 - Creation


using System;

namespace Kuick
{
	public class DatabaseConfigException : ConfigException
	{
		public DatabaseConfigException(
			string entityName, string vender, string schema, string help)
			: base(ConfigScope.Database)
		{
			this.Attribute = string.Format(
				"@entityName='{0}' and @vender='{1}' and @schema='{2}'", 
				entityName, 
				vender,
				schema
			);
			this.Help = help;
		}
	}
}

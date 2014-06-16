// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigException.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-31 - Creation


using System;

namespace Kuick
{
	public class ConfigException : KException
	{
		public ConfigException(ConfigScope scope)
			: this(scope, string.Empty, string.Empty)
		{
		}
		public ConfigException(ConfigScope scope, string attribute, string help)
			: base()
		{
			this.Scope = scope;
			this.Attribute = attribute;
			this.Help = help;
		}

		public ConfigScope Scope { get; private set; }
		public string Attribute { get; protected set; }
		public string Help { get; protected set; }

		public override string Message
		{
			get
			{
				return string.Format(
					"{0} config exception: configuration/Kuick/{0}/add[{1}] >> ",
					Scope,
					Attribute,
					Help
				);
			}
		}
	}
}

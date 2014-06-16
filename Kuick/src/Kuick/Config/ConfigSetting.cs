// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigSetting.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class ConfigSetting : GroupNameBase
	{
		#region constructor
		public ConfigSetting()
			: this(string.Empty, string.Empty, string.Empty)
		{
		}

		public ConfigSetting(string group, string name, string value)
		{
			this.Group = group.AirBag(Constants.Default.Group);
			this.Name = name.AirBag(Constants.Default.Name);
			this.Value = value;
		}
		#endregion

		#region property
		public string Value { get; set; }
		#endregion

		#region override
		public override string ToString()
		{
			return Value;
		}
		#endregion

		#region inner constants class
		internal class Xml
		{
			internal const string Tag = "add";

			internal class Attribute
			{
				internal const string Group = "group";
				internal const string Name = "name";
				internal const string Value = "value";
			}
		}
		#endregion
	}
}

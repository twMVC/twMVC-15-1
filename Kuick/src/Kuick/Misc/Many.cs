// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Many.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-04 - Creation


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kuick
{
	public class Many : IName, ICloneable<Many>
	{
		public const string NAME = "Name";
		public const string ANY1 = "ANY1";
		public const string ANY2 = "ANY2";

		#region constructor
		public Many()
		{
		}

		public Many(string name)
			: this(name, null, null)
		{
		}

		public Many(string name, object value1, object value2)
		{
			this.Name = name;
			this.Any1 = new Any(name, value1);
			this.Any2 = new Any(name, value2);
		}
		#endregion

		#region IName
		public string Name { get; set; }

		public bool Equals(string name)
		{
			if(Checker.IsNull(Name) || Checker.IsNull(name)) { return false; }
			return this.Name == name;
		}
		#endregion

		#region IEquatable<IName>
		public bool Equals(IName other)
		{
			if(null == other) { return false; }
			return this.Name == other.Name;
		}
		#endregion

		#region ICloneable<Many>
		public Many Clone()
		{
			Many clone = new Many();
			clone.Name = this.Name;
			clone.Any1 = this.Any1;
			clone.Any2 = this.Any2;
			return clone;
		}
		#endregion

		#region property
		public Any Any1 { get; set; }
		public Any Any2 { get; set; }
		#endregion

		#region method
		public Many SetValue1(object value1)
		{
			Any1.Value = value1;
			return this;
		}

		public Many SetValue2(object value2)
		{
			Any2.Value = value2;
			return this;
		}
		#endregion
	}
}

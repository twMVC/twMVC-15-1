// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PermissionGroupAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;

namespace Kuick
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class PermissionGroupAttribute : Attribute
	{
		public PermissionGroupAttribute(string name)
			: this(name, string.Empty)
		{
		}

		public PermissionGroupAttribute(string name, string description)
		{
			this.Name = name;
			this.Description = description;
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public OperationType OperationType { get { return OperationType.Group; } }
	}
}

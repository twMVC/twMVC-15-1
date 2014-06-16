// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PermissionAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;

namespace Kuick
{
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
		AllowMultiple = false,
		Inherited = false)
	]
	public class PermissionAttribute : Attribute
	{
		public PermissionAttribute()
			: this(PermissionAction.Admin, string.Empty)
		{
		}

		public PermissionAttribute(string description)
			: this(PermissionAction.Admin, description)
		{
		}

		public PermissionAttribute(PermissionAction action)
			: this(action, string.Empty)
		{
		}

		public PermissionAttribute(PermissionAction action, string description)
		{
			this.Action = action;
			this.Description = description;
		}

		public PermissionAction Action { get; set; }
		public string Description { get; set; }
		public OperationType OperationType { get { return OperationType.Page; } }
	}
}

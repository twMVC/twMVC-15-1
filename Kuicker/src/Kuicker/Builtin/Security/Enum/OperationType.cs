// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(LogLevel.Info)]
	public enum OperationType
	{
		/// <summary>
		/// An operation section.
		/// </summary>
		Execute,

		/// <summary>
		/// A web page or a winwdows form.
		/// </summary>
		Page,

		/// <summary>
		/// Refers to a series of related operation.
		/// </summary>
		Group,

		/// <summary>
		/// Refers to a particular Entity only.
		/// </summary>
		Entity
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KernelPosition.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-03-01 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick
{
	[Serializable]
	[Flags]
	[DefaultValue(KernelPosition.Stopped)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum KernelPosition
	{
		Stopped,
		Running,
		Starting,
		Terminating,

		PreStart,
		BuiltinStart,
		PreDatabaseStart,
		DatabaseStart,
		PostDatabaseStart,
		PostStart,

		BuiltinTerminate,
		PostTerminate
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Scope.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[DefaultValue(Scope.Unknown)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum Scope
	{
		[Description("UnCategory")]
		Unknown,

		[Description("Kuick Application Framework")]
		Kernel,

		[Description("Kuick O/R Mapping Framework")]
		Data,

		[Description("Kuick Builtins")]
		Builtins,

		[Description("Kuick Server")]
		Server,

		[Description("Kuick BackEnd")]
		BackEnd,

		[Description("Projects built on Kuick")]
		Project
	}
}

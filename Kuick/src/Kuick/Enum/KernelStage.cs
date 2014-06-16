// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KernelStage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick
{
	[Serializable]
	[Flags]
	[DefaultValue(KernelStage.Stopped)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum KernelStage
	{
		Stopped = 0,
		Starting = 1,
		Running = 2,
		Terminating = 4
	}
}

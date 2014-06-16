// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AuditLevel.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-18 - Creation


using System.Xml.Serialization;

namespace Kuick
{
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum AuditLevel
	{
		None,
		Success,
		Failure,
		SuccessOrFailure
	}
}

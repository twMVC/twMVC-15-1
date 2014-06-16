// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BoundaryString.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(BoundaryString.None)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum BoundaryString
	{
		None,
		In,
		Exclude,
		ContainsAny,
		ContainsAll,
		StartWith,
		EndWith,
		IsMatch,
		LengthRange,
		LengthGreaterThan,
		LengthSmallerThan,
		IdentificationCardNumberTW,
		IdentificationCardNumberCN,
		CreditCardNumber,
		UnifiedSerialNumber,
		IsEmailAddress
	}
}

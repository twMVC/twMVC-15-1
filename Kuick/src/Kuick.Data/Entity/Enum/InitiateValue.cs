// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// InitiateValue.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(InitiateValue.None)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum InitiateValue
	{
		None,
		Empty,
		Uuid,
		UuidAndAutoUpdate,
		UniqueTicks,
		UniqueTicksAndAutoUpdate,
		NullDate,
		MaxDate,

		/// <summary>
		/// yyyy
		/// </summary>
		Date4,

		/// <summary>
		/// yyyyMMdd
		/// </summary>
		Date8s,

		/// <summary>
		/// yyyy-MM-dd
		/// </summary>
		Date8,

		/// <summary>
		/// yyyyMMddhhmmss
		/// </summary>
		Date14s,

		/// <summary>
		/// yyyy-MM-dd hh:mm:ss
		/// </summary>
		Date14,

		/// <summary>
		/// yyyyMMddhhmmssfff
		/// </summary>
		Date17s,

		/// <summary>
		/// yyyy-MM-dd hh:mm:ss.fff
		/// </summary>
		Date17,

		DaysOffset,

		Date14AutoUpdate,
		Date17AutoUpdate,
		IntegerAndAutoIncremental,
		ToUpper,
		ToLower,
		CurrentUser,
		CurrentUserAndAutoUpdate,
		TimsSpanHex
	}
}

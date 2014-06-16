// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// VisualInput.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[DefaultValue(VisualInput.TextBox)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum VisualInput
	{
		TextBox,
		TextArea,
		Password,
		Date,
		Time,
		TimeStamp,
		FileUpload,
		DropDownList,
		CheckBoxList,
		CheckBox,
		RadioButtons,
		HtmlEditor,
		Color
	}
}

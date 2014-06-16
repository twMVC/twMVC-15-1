// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AttributeName.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-03 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Web.UI.Bootstrap
{
	// http://msdn.microsoft.com/zh-tw/library/system.web.ui.htmltextwriterattribute(v=vs.80).aspx
	[DefaultValue(AttributeName.Id)]
	public enum AttributeName
	{
		Non,
		Alt,
		Checked,
		Class,
		Cols,
		Colspan,
		Disabled,
		For,
		Href,
		Id,
		Maxlength,
		Multiple,
		Name,
		Nowrap,
		Placeholder,
		Role,
		Rows,
		Rowspan,
		Selected,
		Size,
		Src,
		Style,
		TabIndex,
		Target,
		Title,
		Type,
		Value,
		Width,

		//
		NgModel,
		BtnCheckbox,
		BtnCheckboxTrue,
		BtnCheckboxFalse,
		BtnRadio,

		//
		AriaLabelledBy,
		AriaHidden,

		//
		DataToggle,
		DataTarget,
		DataSrc,
		DataDismiss,
		DataParent,
		DataLoadingText,
		DataCompleteText,
		DataSlideTo,
		DataSlide,
		DataProvide,

		//
		KuickEntityName,
		KuickKeyValue,
		KuickColumnName,
		KuickOriginalValue,

		//
		Required
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IAngularJSHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-10 - Creation


using System;
using System.Collections.Generic;
using System.Text;

namespace Kuick.Web.UI.Bootstrap
{
	public interface IAngularJSHtmlTag
	{
		string NgModel { get; set; }
		bool BtnCheckbox { get; set; }
		string BtnCheckboxTrue { get; set; }
		string BtnCheckboxFalse { get; set; }
		string BtnRadio { get; set; }
	}
}

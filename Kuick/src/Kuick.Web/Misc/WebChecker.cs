// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebChecker.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;
using System.Web.UI.WebControls;

namespace Kuick.Web
{
	public class WebChecker : Kuick.Checker
	{
		public static bool IsNull(TextBox textbox)
		{
			return null == textbox || WebChecker.IsNull(textbox.Text);
		}
		public static bool IsNull(CheckBox checkBox)
		{
			return null == checkBox || !checkBox.Checked;
		}
		public static bool IsNull(DropDownList dropDownList)
		{
			return null == dropDownList || WebChecker.IsNull(dropDownList.SelectedValue);
		}
		public static bool IsNull(HiddenField hiddenField)
		{
			return null == hiddenField || WebChecker.IsNull(hiddenField.Value);
		}
		public static bool IsNull(Label label)
		{
			return null == label || WebChecker.IsNull(label.Text);
		}
		public static bool IsNullReference(TextBox textbox)
		{
			return null == textbox;
		}
		public static bool IsNullReference(CheckBox checkBox)
		{
			return null == checkBox;
		}
		public static bool IsNullReference(DropDownList dropDownList)
		{
			return null == dropDownList;
		}
		public static bool IsNullReference(HiddenField hiddenField)
		{
			return null == hiddenField;
		}
		public static bool IsNullReference(Label label)
		{
			return null == label;
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ButtonTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class ButtonTag : RichContainerHtmlTag, IDataHtmlTag, IAriaHtmlTag
	{
		public ButtonTag()
			: base()
		{
		}

		[Attr]
		public string Type { get; set; }

		[Attr]
		public bool Disabled { get; set; }

		#region IDataHtmlTag
		[Attr]
		public string DataToggle { get; set; }

		[Attr]
		public string DataTarget { get; set; }

		[Attr]
		public string DataDismiss { get; set; }

		[Attr]
		public string DataParent { get; set; }
		#endregion

		[Attr]
		public string DataLoadingText { get; set; }

		#region IAriaHtmlTag
		[Attr]
		public string AriaLabelledBy { get; set; }

		[Attr]
		public bool AriaHidden { get; set; }
		#endregion

		// AngularJS
		[Attr]
		public string NgModel { get; set; }
		[Attr]
		public bool BtnCheckbox { get; set; }
		[Attr]
		public string BtnCheckboxTrue { get; set; }
		[Attr]
		public string BtnCheckboxFalse { get; set; }
		[Attr]
		public string BtnRadio { get; set; }
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BToggle.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-10 - Creation


using System;
using System.Text;

namespace Kuick.Web.UI.Bootstrap
{
	public class BToggle : ButtonTag
	{
		public BToggle()
			: base()
		{
			this.Type = "button";
			this.Class.Add(Css.Btn, Css.BtnPrimary);
			this.NgModel = "singleModel";
			this.BtnCheckbox = true;
			this.BtnCheckboxTrue = "1";
			this.BtnCheckboxFalse = "0";

			this.Title = "BToggle";
		}

		public override TagName TagName
		{
			get
			{
				return TagName.Button;
			}
		}

		public string Title { get; set; }

		public override void Render(StringBuilder sb)
		{
			base.Render(sb);
			sb.Append(Title);
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BRadio.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-10 - Creation


using System;
using System.Text;
using System.Collections.Generic;

namespace Kuick.Web.UI.Bootstrap
{
	public class BRadio : DivTag
	{
		public BRadio()
			: base()
		{
			base.Class.Add(Css.BtnGroup);
			base.DataToggle = "buttons-checkbox";
			this.Buttons = new List<ButtonTag>();
		}

		public override TagName TagName
		{
			get
			{
				return TagName.Div;
			}
		}

		protected List<ButtonTag> Buttons { get; set; }

		public BRadio AddItem(string name)
		{
			ButtonTag button = new ButtonTag();
			button.Type = "button";
			button.Class.Add(Css.Btn, Css.BtnPrimary);
			button.NgModel = "radioModel";
			button.BtnRadio = "'" + name + "'";
			Buttons.Add(button);
			return this;
		}

		public override void Render(StringBuilder sb)
		{
			base.Render(sb);
			foreach(ButtonTag button in Buttons) {
				button.Render(sb);
				sb.Append(button.BtnRadio.TrimStart("'").TrimEnd("'"));
				button.RenderCloseTag();
			}
			base.RenderCloseTag();
		}
	}
}

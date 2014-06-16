// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// UserControlBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web.UI;
using System.Collections.Generic;

namespace Kuick.Web
{
	public abstract class UserControlBase : UserControl
	{
		public event EventHandler OnValidRequestBegin;
		public event EventHandler OnValidRequestEnd;

		private bool _O = false;

		protected override void OnInit(EventArgs e)
		{
			if(_O) { return; }

			if(OnValidRequestBegin != null) { OnValidRequestBegin(this, EventArgs.Empty); }

			List<RequestParameterAttribute> parameters =
				AttributeHelper.GetRequestParameterAttributes(this.GetType());
			AttributeHelper.ValidateParameter(Page, parameters);
			AttributeHelper.SetValueByParameter(this, parameters);

			if(OnValidRequestEnd != null) { OnValidRequestEnd(this, EventArgs.Empty); }

			_O = true;
			base.OnInit(e);
		}


		public new PageBase Page
		{
			get
			{
				return (PageBase)base.Page;
			}
		}

		public void RegisterCSS(string href)
		{
			Page.RegisterCSS(href);
		}

		public void RunJs(string js)
		{
			Page.RunJs(js, false);
		}

		public void RunJs(string js, bool defer)
		{
			Page.RunJs(js, defer);
		}

		public void RunJs(string js, params object[] args)
		{
			Page.RunJs(js, args);
		}
	}
}

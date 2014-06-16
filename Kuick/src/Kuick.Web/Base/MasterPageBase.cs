// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MasterPageBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web.UI;
using System.Collections.Generic;

namespace Kuick.Web
{
	public abstract class MasterPageBase : MasterPage
	{
		public new PageBase Page
		{
			get
			{
				return (PageBase)base.Page;
			}
		}

		public virtual string SecurityPage
		{
			get
			{
				return ResolveUrl("~/Security.aspx");
			}
		}

		public virtual string AuthorizePage
		{
			get
			{
				return ResolveUrl("~/Authorize.aspx");
			}
		}

		protected override void OnInit(EventArgs e)
		{
			List<RequestParameterAttribute> parameters =
				AttributeHelper.GetRequestParameterAttributes(this.GetType());
			AttributeHelper.ValidateParameter(Page, parameters);
			AttributeHelper.SetValueByParameter(this, parameters);

			base.OnInit(e);
		}
	}
}

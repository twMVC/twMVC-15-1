// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SigninPage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-24 - Creation


using System;
using Kuick.Data;
using System.Web.Security;
using System.Web;

namespace Kuick.Web.UI
{
	public class SigninPage : PageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		public virtual string SignoutUrl { get { return "~/Signout.aspx"; } }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if(Authenticated) { Response.Redirect(SignoutUrl); }
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SignoutPage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-24 - Creation


using System;
using Kuick.Data;
using System.Web.Security;
using System.Web;

namespace Kuick.Web.UI
{
	public class SignoutPage : PageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if(Clear) {
				FormsAuthentication.SignOut();
				Response.Redirect(FormsAuthentication.LoginUrl);
			} else {
				Session.Abandon();
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
				Response.Cache.SetNoStore();
				Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
				Response.AppendHeader("Pragma", "no-cache");
				Response.Redirect(WebTools.GetCurrentAspxWithExt() + "?Clear=true");
			}
		}

		[RequestParameter]
		public bool Clear { get; set; }
	}
}
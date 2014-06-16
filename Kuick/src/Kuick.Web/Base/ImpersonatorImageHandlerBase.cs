// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ImpersonatorImageHandlerBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public abstract class ImpersonatorImageHandlerBase<T>
		: ImageHandlerBase
		where T : ForceImpersonator
	{
		public override void OnProcessRequest(HttpContext ctx)
		{
			if(WebCurrent.Impersonator.Enable) {
				using(ForceImpersonator fi = Reflector.CreateInstance<T>(
					new Type[] { typeof(string), typeof(string), typeof(string) },
					new object[]{
						WebCurrent.Impersonator.UserName,
						WebCurrent.Impersonator.DomainName,
						WebCurrent.Impersonator.Password})) {
					fi.Anys.Add("HttpContext", ctx);
					fi.Dealing += new EventHandler(OnProcessRequestDelegate);
					fi.Deal();
				}
			} else {
				base.OnProcessRequest(ctx);
			}
		}

		private void OnProcessRequestDelegate(object sender, EventArgs e)
		{
			ForceImpersonator fi = sender as ForceImpersonator;
			if(null == fi) { return; }
			HttpContext ctx = fi.Anys.GetValue("HttpContext") as HttpContext;
			base.OnProcessRequest(ctx);
		}
	}
}

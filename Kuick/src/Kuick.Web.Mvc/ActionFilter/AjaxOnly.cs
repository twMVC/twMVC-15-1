// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ListModel.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Kuick.Web.Mvc
{
	public class AjaxOnly : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if(filterContext.HttpContext.Request.IsAjaxRequest()) {
				base.OnActionExecuting(filterContext);
			} else {
				throw new HttpException(404, "Not Found");
			}
		}
	}
}

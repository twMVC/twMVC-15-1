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
	public class ListModel : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var result = filterContext.Result as ViewResultBase;
			if(result.Model == null) {
				throw new HttpException(404, "Not Found");
			} else {
				var list = (dynamic)result.Model;
				if(list.Count == 0) {
					throw new HttpException(404, "Not Found");
				}
				base.OnActionExecuted(filterContext);
			}
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// QueryableModel.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Linq;

namespace Kuick.Web.Mvc
{
	public class QueryableModel : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var result = filterContext.Result as ViewResultBase;
			if(
				null == result
				||
				null == result.Model
				||
				!result.Model.IsDerived<IQueryable>()) {
				throw new HttpException(404, "Not Found");
			}
			base.OnActionExecuted(filterContext);
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// JsonResult.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Kuick.Web.Mvc
{
	public class JsonResult : ActionResult
	{
		public JsonResult()
			: this(null)
		{
		}

		public JsonResult(object data)
		{
			this.Data = data;
		}

		public object Data { get; set; }

		public override void ExecuteResult(ControllerContext context)
		{
			string json = null == Data
				? Constants.Json.Null.Object
				: JsonConvert.SerializeObject(Data);

			HttpResponseBase response = context.HttpContext.Response;
			response.ContentEncoding = Encoding.UTF8;
			response.ContentType = Constants.ContentType.Json;
			response.Write(json);
		}
	}
}
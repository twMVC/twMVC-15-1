// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ApiHandler.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-27 - Creation


using System;
using Kuick.Data;
using System.Web;
using System.Net;

namespace Kuick.Web.UI
{
	public abstract class ApiHandler : ApiHandlerBase
	{
		public override string Title
		{
			get
			{
				return "ApiHandler: " + this.GetType().Name;
			}
		}

		protected HttpStatusCode Status { get; set; }
		protected string Description { get; set; }
		private ILogger ProcessingILogger { get; set; }

		public override void OnProcessRequest(HttpContext ctx)
		{
			if(!SigninCheck()) {
				Response.StatusCode = (int)HttpStatusCode.Forbidden;
				Response.StatusDescription = Error("Need to be signin.");
				return;
			}

			using(ILogger logger = new ILogger(Title, LogLevel.Track)) {
				ProcessingILogger = logger;
				try {
					Status = HttpStatusCode.OK;

					// 1. CheckParameter
					using(IILogger iil = new IILogger("CheckParameter", logger)) {
						CheckParameter();
						if(Status != HttpStatusCode.OK) {
							Response.StatusCode = (int)Status;
							Response.StatusDescription = string.IsNullOrEmpty(Description)
								? string.Empty
								: Description.UrlEncode();
							logger.Add("CheckParameter Error", Description);
							logger.Level = LogLevel.Error;
							return;
						}
					}

					// 2. Deal
					using(IILogger iil = new IILogger("Deal", logger)) {
						Deal();
						if(Status != HttpStatusCode.OK) {
							Response.StatusCode = (int)Status;
							Response.StatusDescription = string.IsNullOrEmpty(Description)
								? string.Empty
								: Description.UrlEncode();
							logger.Add("Deal Error", Description);
							logger.Level = LogLevel.Error;
							return;
						}
					}

					// 3. ResponseHeader
					using(IILogger iil = new IILogger("ResponseHeader", logger)) {
						ResponseHeader();
					}

					// 4. ResponseData
					using(IILogger iil = new IILogger("ResponseData", logger)) {
						ResponseData();
						if(Status != HttpStatusCode.OK) {
							Response.StatusCode = (int)Status;
							Response.StatusDescription = string.IsNullOrEmpty(Description)
								? string.Empty
								: Description.UrlEncode();
							logger.Add("ResponseData Error", Description);
							logger.Level = LogLevel.Error;
							return;
						}
					}
				} catch(Exception ex) {
					Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					Response.StatusDescription = string.IsNullOrEmpty(ex.Message)
						? string.Empty
						: ex.Message.UrlEncode();
					logger.Add("Exception", "Exception Occured as following informations!");
					logger.AddRange(ex.ToAny());
					logger.Level = LogLevel.Error;
					return;
				}
			}
		}

		public virtual bool SigninCheck()
		{
			return true;
		}

		public virtual void CheckParameter()
		{
		}

		public virtual void Deal()
		{
		}

		public virtual void ResponseHeader()
		{
			Response.ContentType = ContentType.JSON;
			Response.Cache.SetExpires(DateTime.Now.AddHours(-1));
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		public virtual void ResponseData()
		{
		}

		public string Error(string message, params Any[] anys)
		{
			if(null == ProcessingILogger) {
				return Logger.Error(
					WebTools.GetRawUrlWithoutApplication(),
					message,
					anys
				);
			} else {
				ProcessingILogger.Add("Error Message", message);
				ProcessingILogger.AddRange(anys);
				ProcessingILogger.Level = LogLevel.Error;
				return message;
			}
		}
	}
}

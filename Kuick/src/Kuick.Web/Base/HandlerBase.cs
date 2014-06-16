// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HandlerBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace Kuick.Web
{
	public abstract class HandlerBase
		: WebBase, IHttpHandler
	{
		private DateTime Now = DateTime.Now;

		// event
		protected event EventHandler<HandlerEventArgs> ProcessRequestBegin;
		protected event EventHandler<HandlerEventArgs> ProcessRequestEnd;

		protected event EventHandler<HandlerEventArgs> OnCheckVerbBegin;
		protected event EventHandler<HandlerEventArgs> OnCheckVerbEnd;

		protected event EventHandler<HandlerEventArgs> OnValidRequestBegin;
		protected event EventHandler<HandlerEventArgs> OnValidRequestEnd;

		/// <summary>
		/// You can do somthing before the output cache response, 
		/// such as log to db, get last modified time ...etc.
		/// WARNING: This event always trigger even if the page is not cached.
		/// </summary>
		protected event EventHandler<HandlerEventArgs> OnBeforeProcessCache;

		public HandlerBase()
		{
		}

		private void RaiseEvent(EventHandler<HandlerEventArgs> e)
		{
			if(!WebChecker.IsNull(e)) {
				e(this, new HandlerEventArgs(Context));
			}
		}

		public abstract void OnProcessRequest(HttpContext ctx);

		#region Properties
		public HttpRequest Request { get; private set; }
		public HttpResponse Response { get; private set; }
		public HttpServerUtility Server { get; private set; }
		public HttpSessionState Session { get; private set; }
		public HttpApplicationState Application { get; private set; }
		public HttpContext Context { get; private set; }
		protected virtual DateTime LastModifiedTime { get { return Now; } }
		#endregion

		#region ProcessRequestBegin event handlers
		private void Compress()
		{
			CompressAttribute ca = AttributeHelper.Get<CompressAttribute>(
				this.GetType().BaseType
			);
			if(null != ca) { CompressionModule.Compress(Response); }
		}
		private void CheckVerb()
		{
			RaiseEvent(OnCheckVerbBegin);
			AttributeHelper.CheckAcceptFilter(this.GetType(), Request);
			RaiseEvent(OnCheckVerbEnd);
		}

		private void ValidRequest()
		{
			RaiseEvent(OnValidRequestBegin);
			List<RequestParameterAttribute> parameters =
				AttributeHelper.GetRequestParameterAttributes(this.GetType());
			AttributeHelper.ValidateParameter(this, parameters);
			AttributeHelper.SetValueByParameter(this, parameters);
			RaiseEvent(OnValidRequestEnd);
		}
		private DateTime _LastModifiedTime;
		private void ProcessCache()
		{
			RaiseEvent(OnBeforeProcessCache);

			CacheDurationAttribute cda = AttributeHelper.Get<CacheDurationAttribute>(
				this.GetType().BaseType
			) ?? new CacheDurationAttribute();
			_LastModifiedTime = LastModifiedTime;

			if(WebCurrent.Mode != KernelMode.Developing && cda.Duration > 0) {
				Response.Cache.SetExpires(cda.ExpireTime(Now));
			}

			if(Now != _LastModifiedTime) {
				if(NotModified(_LastModifiedTime)) { return; }
				Response.Cache.SetLastModified(_LastModifiedTime);
			}
		}

		/// <summary>
		/// Hook for nonsecure url.
		/// If your server is not use default port or use other Urischeme, you can override it.
		/// </summary>
		/// <param name="defaultRedirectUri">the default redriect url for nonsecure url</param>
		/// <returns>The uri to redirect</returns>
		protected virtual UriBuilder NonSecureUrl(UriBuilder defaultRedirectUri)
		{
			return defaultRedirectUri;
		}

		private void Secure()
		{
			SecureAttribute secure = AttributeHelper.Get<SecureAttribute>(
				this.GetType().BaseType
			);
			UriBuilder ub = AttributeHelper.CheckSecureConnect(secure, Request);
			if(ub != null) {
				Response.Redirect(
					(ub.Scheme == Uri.UriSchemeHttps ? ub : NonSecureUrl(ub)).Uri.ToString(),
					true
				);
			}
		}

		#endregion
		#region IHttpHandler
		public void ProcessRequest(HttpContext context)
		{
			this.Context = context;
			this.Request = context.Request;
			this.Response = context.Response;
			this.Server = context.Server;
			this.Session = context.Session;
			this.Application = context.Application;
			base.Reset(context);

			CheckVerb();
			if(WebCurrent.Web.CheckSsl) { Secure(); }
			Compress();
			ValidRequest();
			ProcessCache();

			RaiseEvent(ProcessRequestBegin);

			try {
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				OnProcessRequest(context);
			} catch(Exception ex) {
				Logger.Error(ex);
				throw;
			}

			RaiseEvent(ProcessRequestEnd);
		}

		public virtual bool IsReusable
		{
			get
			{
				return false;
			}
		}
		#endregion


		#region Utility
		/// <summary>
		/// Check If-Modified-Since Http header, default compare format yyyyMMddHHmm
		/// </summary>
		/// <param name="modifiedTime"></param>
		/// <returns></returns>
		protected bool NotModified(DateTime modifiedTime)
		{
			string compareFormat = "yyyyMMddHHmm";
			return NotModified(modifiedTime, compareFormat);
		}
		/// <summary>
		/// Check If-Modified-Since Http header
		/// </summary>
		/// <param name="modifiedTime"></param>
		/// <param name="compareFormat"></param>
		/// <returns></returns>
		protected bool NotModified(DateTime modifiedTime, string compareFormat)
		{
			DateTime since = new DateTime();

			if(!DateTime.TryParse(Request.Headers["If-Modified-Since"], out since)) {
				return false;
			}
			if(!since.ToString(compareFormat).Equals(modifiedTime.ToString(compareFormat))) {
				return false;
			}

			Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
			Response.End();
			return true;
		}
		#endregion
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Collections;

namespace Kuick.Web
{
	public class WebBase : IWeb
	{
		private HttpContext _Context;
		private NameValueCollection _NVs;
		private IParameter _Parameter;


		#region constructor
		public WebBase()
			: this(HttpContext.Current)
		{
		}

		public WebBase(HttpContext context)
		{
			this.Errors = new Anys();
			_Context = context;
		}
		#endregion

		#region internal
		internal void Reset(HttpContext context)
		{
			_Context = context;
			_NVs = null;
			_Parameter = null;
		}
		#endregion

		#region IWeb
		public IParameter Parameter
		{
			get
			{
				return GetParameter();
			}
		}

		public HttpContext GetCurrentContext()
		{
			return _Context;
		}
		public NameValueCollection GetRequestNVs()
		{
			if(null == _NVs) {
				_NVs = new NameValueCollection();
				_NVs.Add(_Context.Request.Form);
				_NVs.Add(_Context.Request.QueryString);
				HttpCookieCollection cookies = _Context.Request.Cookies;
				for(int i = 0; i < cookies.Count; i++) {
					_NVs.Add(cookies[i].Name, cookies[i].Value);
				}
			}
			return _NVs;
		}
		public IParameter GetParameter()
		{
			if(null == _Parameter) {
				_Parameter = new WebParameter(GetRequestNVs());
				_Parameter.Request = _Context.Request;
			}
			return _Parameter;
		}
		public string RequestValue(string name)
		{
			return Parameter.RequestValue(name, string.Empty);
		}
		public string RequestValue(string name, string airBag)
		{
			return Parameter.RequestValue(name, airBag);
		}

		private string _UserAgent;
		public string UserAgent
		{
			get
			{
				if(null == _UserAgent) {
					_UserAgent = _Context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower();
				}
				return _UserAgent;
			}
		}

		public bool IsMobileClient
		{
			get
			{
				string client = UserAgent.ToLower();
				foreach(string agent in WebConstants.UserAgent.Mobiles) {
					if(client.Contains(agent.ToLower())) { return true; }
				}
				return false;
			}
		}

		public bool IsiOSClient
		{
			get
			{
				return IsiPodClient || IsiPhoneClient || IsiPadClient;
			}
		}

		public bool IsiPodClient
		{
			get
			{
				return UserAgent.ToLower().Contains(WebConstants.UserAgent.iPod);
			}
		}

		public bool IsiPhoneClient
		{
			get
			{
				return UserAgent.ToLower().Contains(WebConstants.UserAgent.iPhone);
			}
		}

		public bool IsiPadClient
		{
			get
			{
				return UserAgent.ToLower().Contains(WebConstants.UserAgent.iPad);
			}
		}

		public Anys Errors { get; set; }

		public bool Authenticated
		{
			get
			{
				if(FormsAuthentication.IsEnabled) {
					// Forms Authentication
					HttpContext context = GetCurrentContext();
					if(null == context){return false;}

					return
						null != context.User &&
						null != context.User.Identity &&
						!string.IsNullOrEmpty(context.User.Identity.Name);
				} else {
					// Authentication state saved in session
					return
						SessionStorage.Singleton.Exists(Constants.Authentication.User)
						||
						SessionStorage.Singleton.Exists(Constants.Authentication.UserID);
				}
			}
		}
		#endregion
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ModuleBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Specialized;
using System.Web;

namespace Kuick.Web
{
	public abstract class ModuleBase : IHttpModule, IModule
	{
		private IWeb _WebBase;
		
		#region constructor
		public ModuleBase()
		{
			this.Errors = new Anys();
		}
		#endregion

		#region IHttpModule
		public virtual void Init(HttpApplication context)
		{
			this.Application = context;
		}

		public virtual void Dispose()
		{
		}
		#endregion

		#region IWeb
		public IParameter Parameter
		{
			get
			{
				return this.WebBase.Parameter;
			}
		}

		public HttpContext GetCurrentContext()
		{
			return this.WebBase.GetCurrentContext();
		}

		public NameValueCollection GetRequestNVs()
		{
			return this.WebBase.GetRequestNVs();
		}

		public IParameter GetParameter()
		{
			return this.WebBase.GetParameter();
		}

		public string RequestValue(string name)
		{
			return this.WebBase.RequestValue(name);
		}

		public string RequestValue(string name, string airBag)
		{
			return this.WebBase.RequestValue(name, airBag);
		}

		public string UserAgent
		{
			get
			{
				return WebBase.UserAgent;
			}
		}

		public bool IsMobileClient
		{
			get
			{
				return WebBase.IsMobileClient;
			}
		}

		public bool IsiOSClient
		{
			get
			{
				return WebBase.IsiOSClient;
			}
		}

		public bool IsiPodClient
		{
			get
			{
				return WebBase.IsiPodClient;
			}
		}

		public bool IsiPhoneClient
		{
			get
			{
				return WebBase.IsiPhoneClient;
			}
		}

		public bool IsiPadClient
		{
			get
			{
				return WebBase.IsiPadClient;
			}
		}

		public Anys Errors { get; set; }

		public bool Authenticated
		{
			get 
			{
				return WebBase.Authenticated;
			}
		}
		#endregion

		#region IModule
		public HttpApplication Application { get; set; }

		public IWeb WebBase
		{
			get
			{
				if(null == _WebBase) {
					_WebBase = new WebBase(this.Application.Context);
				}
				return _WebBase;
			}
		}
		#endregion

		#region Sample Event Handler
		private void OnSomeEvent(object sender, EventArgs e)
		{
			this.Application = (HttpApplication)sender;

			// to do..
		}
		#endregion
	}
}

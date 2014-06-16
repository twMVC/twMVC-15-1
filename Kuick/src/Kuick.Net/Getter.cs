// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Getter.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-28 - Creation


using System;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace Kuick.Net
{
	public class Getter
	{
		#region DllImport
		[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool InternetGetCookieEx(
			string pchURL,
			string pchCookieName,
			StringBuilder pchCookieData,
			ref int pcchCookieData,
			int dwFlags,
			object lpReserved
		);
		#endregion

		#region constructor
		public Getter()
		{
			this.ApiUrl = string.Empty;
			this.RedirectUrl = string.Empty;
			this.Method = HttpMethod.Get;
			this.Datas = new Anys();
			this.Message = string.Empty;
		}
		#endregion

		#region property
		public string ApiUrl { get; set; }
		public string RedirectUrl { get; set; }
		public HttpMethod Method { get; set; }
		public Anys Datas { get; private set; }
		// new NetworkCredential("UserName", "Password", "DomainName");
		public ICredentials Credential { get; set; }
		public IWebProxy Proxy { get; set; }
		public ICredentials ProxyCredential { get; set; }

		public bool HasError { get; private set; }
		public string Message { get; private set; }
		#endregion

		#region method
		public string Invoke()
		{
			try {
				StringBuilder sb = new StringBuilder();
				foreach(Any any in Datas) {
					if(sb.Length == 0) { sb.Append(NetConstants.Symbol.Ampersand); }
					sb.AppendFormat("{0}={1}", any.Name.HtmlEncode(), any.ToString().HtmlEncode());
				}
				string parameters = sb.ToString();

				HttpWebRequest rq;
				switch(Method) {
					case HttpMethod.Get:
						rq = (HttpWebRequest)HttpWebRequest.Create(
							ApiUrl +
							(parameters.Length == 0 ? string.Empty : "?" + parameters)
						);
						rq.Method = WebRequestMethods.Http.Get;
						break;
					case HttpMethod.Post:
						byte[] parametersBytes = parameters.ToBytes();
						rq = (HttpWebRequest)HttpWebRequest.Create(ApiUrl);
						rq.Method = WebRequestMethods.Http.Post;
						rq.ContentLength = parametersBytes.Length;
						break;
					case HttpMethod.Head:
					case HttpMethod.Put:
					case HttpMethod.Delete:
					case HttpMethod.Trace:
					case HttpMethod.Options:
					default:
						throw new NotSupportedException(string.Format(
							"HttpMethod = {0}", Method.ToString()
						));
				}

				CookieCollection cookies = GetCookies(ApiUrl);
				CookieContainer cookieContainer = new CookieContainer();
				cookieContainer.Add(cookies); //
				rq.CookieContainer = cookieContainer;
				rq.ContentType = ContentType.FORM;
				rq.AllowAutoRedirect = true; //
				rq.UserAgent = NetConstants.Net.UserAgent;
				if(null != Credential) { rq.Credentials = Credential; }
				if(null != Proxy) { rq.Proxy = Proxy; }
				if(null != ProxyCredential) { rq.Proxy.Credentials = ProxyCredential; }

				using(HttpWebResponse rp = (HttpWebResponse)rq.GetResponse()) {
					//cookies = rp.Cookies;
					using(Stream stream = rp.GetResponseStream()) {
						using(StreamReader streamReader = new StreamReader(stream)) {
							string trophy = streamReader.ReadToEnd();
							ClearError();
							return trophy;
						}
					}
				}
			} catch(Exception ex) {
				Logger.Error(
					"Getter.Invoke",
					ex,
					Datas.ToAny(
						new Any("ApiUrl", ApiUrl),
						new Any("RedirectUrl", RedirectUrl),
						new Any("Method", Method)
					)
				);
				YieldError(ex.Message);
			}

			YieldError("Server Error");
			return string.Empty;
		}
		#endregion

		#region private
		private string GetCookieString(string url)
		{
			int datasize = 256;
			StringBuilder cookieData = new StringBuilder(datasize);
			try {
				if(!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, null)) {
					if(datasize < 0) {
						return null;
					}
					cookieData = new StringBuilder(datasize);
					if(!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, null)) {
						return null;
					}
				}
			} catch(Exception ex) {
				Logger.Error("Kuick.Net.Getter.GetCookieString", ex.ToAny());
			}
			return cookieData.ToString();
		}

		private CookieCollection CookieStringToCollection(string cookie) 
		{
			CookieCollection cc = new CookieCollection();
			foreach(string pair in cookie.Split(' ')) {
				string[] cookies = pair.Split('=');
				cc.Add(new Cookie(cookies[0], cookies[1]));
			}
			return cc;
		}

		private CookieCollection GetCookies(string url)
		{
			string cookieStr = GetCookieString(ApiUrl);
			CookieCollection cc = CookieStringToCollection(cookieStr);
			return cc;

			//CookieCollection cookies = new CookieCollection();
			//HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			//CookieContainer cookieContainer = new CookieContainer();
			//cookieContainer.Add(cookies);
			//request.CookieContainer = cookieContainer;
			//HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			//cookies = response.Cookies;
			//return cookies;
		}

		private void YieldError(string message)
		{
			HasError = true;
			Message = message;
		}

		private void ClearError()
		{
			HasError = false;
			Message = string.Empty;
		}
		#endregion
	}
}

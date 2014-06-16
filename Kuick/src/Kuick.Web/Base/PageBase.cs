// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PageBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Kuick.Data;

namespace Kuick.Web
{
	public abstract class PageBase : Page, IPage
	{
		#region Privite Fields
		private object _Locking = new object();
		private WebBase _WebBase;
		private StringBuilder _RunJs = new StringBuilder();
		private Dictionary<string, string> _Const = new Dictionary<string, string>();
		private PermissionAttribute _Permission;
		private PageAuthorizeAttribute _PageAuthorize;

		private string _cacheKey;
		private string _cacheKeyDateTime;
		private DateTime Now = DateTime.Now;
		private DateTime _LastModifiedTime;

		// default no cache.
		private CacheDurationAttribute _CacheDuration = new CacheDurationAttribute(0);
		#endregion

		/// <summary>
		/// You can do somthing before the output cache response, 
		/// such as log to db, get last modified time ...etc.
		/// WARNING: This event always trigger even if the page is not cached.
		/// </summary>
		public event EventHandler BeforeProcessCache;
		public event EventHandler OnValidRequestBegin;
		public event EventHandler OnValidRequestEnd;

		#region constructors
		public PageBase()
			: base()
		{
			this.Errors = new Anys();

			if(WebCurrent.Web.CheckSsl) {
				this.PreInit += new EventHandler(Secure_PreInit);
			}
			//this.PreInit += new EventHandler(CompressPage_PreInit);
			//this.PreInit += new EventHandler(PagePermit_PreInit);
			this.PreInit += new EventHandler(PageAuthorize_PreInit);
			this.PreInit += new EventHandler(RequestValidation_PreInit);
			this.PreInit += new EventHandler(CachePage_PreInit);
		}
		#endregion


		#region Event Handler
		private void RequestValidation_PreInit(object sender, EventArgs e)
		{
			if(OnValidRequestBegin != null) { OnValidRequestBegin(this, EventArgs.Empty); }

			List<RequestParameterAttribute> parameters =
				AttributeHelper.GetRequestParameterAttributes(this.GetType());
			AttributeHelper.ValidateParameter(this, parameters);
			AttributeHelper.SetValueByParameter(this, parameters);

			if(OnValidRequestEnd != null) { OnValidRequestEnd(this, EventArgs.Empty); }
		}

		private void Secure_PreInit(object sender, EventArgs e)
		{
			SecureAttribute secure = AttributeHelper.Get<SecureAttribute>(
				this.GetType().BaseType
			);
			UriBuilder ub = AttributeHelper.CheckSecureConnect(secure, Request);
			if(ub != null) {
				if(WebCurrent.Mode.EnumIn(KernelMode.Developing, KernelMode.Testing)) { return; }
				if(!WebChecker.IsLoopBackIP(Request.UserHostAddress)) {
					Response.RedirectPermanent(
						(ub.Scheme == Uri.UriSchemeHttps
							? ub
							: NonSecureUrl(ub)
						).Uri.ToString(),
						true
					);
				}
			}
		}
		private void CompressPage_PreInit(object sender, EventArgs e)
		{
			CompressAttribute ca = AttributeHelper.Get<CompressAttribute>(
				this.GetType().BaseType
			);
			if(null != ca) { CompressionModule.Compress(Response); }
		}
		private void PageAuthorize_PreInit(object sender, EventArgs e)
		{
			if(!Authorizer.FeatureAuthorize(this)){
				MasterPageBase master = this.Master as MasterPageBase;
				string url = (null == master)
					? ResolveUrl("~/Authorize.aspx")
					: master.AuthorizePage;
				Server.Transfer(url);
			}
		}
		private void CachePage_PreInit(object sender, EventArgs e)
		{
			if(BeforeProcessCache != null) { BeforeProcessCache(this, EventArgs.Empty); }

			_CacheDuration = AttributeHelper.Get<CacheDurationAttribute>(
				this.GetType().BaseType
			) ?? new CacheDurationAttribute(0);

			// CacheDuration associated with specific entity
			CacheDurationEntityNameAttribute attrEntityName =
				AttributeHelper.GetCacheDurationEntityNameAttributes(this.GetType());
			CacheDurationEntityKeyValueAttribute attrEntityKeyValue =
				AttributeHelper.GetCacheDurationEntityKeyValueAttributes(this.GetType());
			AttributeHelper.SetCacheDurationEntityNameAndKeyValue(
				this, attrEntityName, attrEntityKeyValue, _CacheDuration
			);

			// cache the last modified time, since it is usually comes from DB.
			_LastModifiedTime = LastModifiedTime;

			if(IsCacheEnable) {
				if(_CacheDuration.ClientOnly) {
					if(Now != _LastModifiedTime && !IsNoCache) {
						if(NotModified(_LastModifiedTime)) { return; }
						Response.Cache.SetLastModified(_LastModifiedTime);
					}
					Response.Cache.SetExpires(Now.AddSeconds(_CacheDuration.Duration));
				} else {
					VaryByParamAttribute vbpa = AttributeHelper.Get<VaryByParamAttribute>(
						this.GetType().BaseType
					) ?? new VaryByParamAttribute();
					_cacheKey = String.Format(
						"{0}?{1}",
						Request.AppRelativeCurrentExecutionFilePath,
						GetKeyByParam(vbpa.Keys)
					);
					_cacheKeyDateTime = String.Concat("::", _cacheKey);

					if(IsCleanCache) {
						Cache.Remove(_cacheKey);
						Cache.Remove(_cacheKeyDateTime);
					}

					if(!IsNoCache) {
						if(Cache[_cacheKey] != null) {
							DateTime? dt = Cache[_cacheKeyDateTime] as DateTime?;
							if(dt.HasValue) {
								DateTime cachedTime = (DateTime)dt;
								if(NotModified(cachedTime)) { return; }

								Response.Cache.SetLastModified(cachedTime);
								Response.Cache.SetExpires(
									_CacheDuration.ExpireTime(cachedTime)
								);
							}
							Response.Clear();
							Response.Write(Convert.ToString(Cache[_cacheKey]));
							Response.End();
							return;
						}
						Response.Cache.SetLastModified(_LastModifiedTime);
						Response.Cache.SetExpires(_CacheDuration.ExpireTime(Now));
					}
				}
			} else if(WebCurrent.Mode != KernelMode.Developing && Now != _LastModifiedTime && !IsNoCache) {
				if(NotModified(_LastModifiedTime)) { return; }
				Response.Cache.SetLastModified(_LastModifiedTime);
			}
		}
		#endregion

		#region override protected methods
		protected override void OnInit(EventArgs e)
		{
			// jQuery
			if(RenderJQueryFile) {
				RunJs(WebCurrent.Web.jQueryFile);
			}

			// AppID
			JsConstant("AppId", WebCurrent.AppID);
			JsConstant("WebRoot", WebTools.GetWebRoot());
			JsConstant("IsMobileClient", IsMobileClient);
			JsConstant("ClickEventName", IsMobileClient ? "touchstart" : "click");

			base.OnInit(e);
		}
		protected override void Render(HtmlTextWriter writer)
		{
			#region RunJs & JsConstant
			if(_RunJs.Length > 0) {
				_RunJs.Insert(0, "$(function(){").Append("});");
			}

			if(_Const.Count > 0) {
				StringBuilder sb = new StringBuilder();
				foreach(var key in _Const.Keys) {
					sb.Append(',').Append(key).Append(':').Append(_Const[key]);
				}
				_RunJs.Insert(
					0,
					sb.Remove(0, 1).Insert(0, "var CONST={").Append("};").ToString()
				);
			}

			//should be call before base.Render
			if(_RunJs.Length > 0) {
				ClientScript.RegisterStartupScript(
					this.GetType(),
					"__runjs",
					MinifyJs(_RunJs),
					true
				);
			}
			#endregion

			if(Header != null) {
				#region meta keywords and description

				if(!String.IsNullOrEmpty(Keywords)) {
					HtmlMeta metaKeywords = new HtmlMeta();
					metaKeywords.Name = "keywords";
					metaKeywords.Content = Keywords;
					Header.Controls.Add(metaKeywords);
				}

				if(!String.IsNullOrEmpty(Description)) {
					HtmlMeta metaDescription = new HtmlMeta();
					metaDescription.Name = "description";
					metaDescription.Content = Description;
					Header.Controls.Add(metaDescription);
				}
				#endregion

				#region Canonical link
				if(!String.IsNullOrEmpty(CanonicalLink)) {
					string href = CanonicalLink;
					if(href.StartsWith("~")) { href = WebTools.ResolveUrl(href, true); }

					HtmlLink canonical = new HtmlLink();
					canonical.Href = href;
					canonical.Attributes["rel"] = "canonical";
					Header.Controls.Add(canonical);
				}
				#endregion
				//OpenGraph.AddTo(Header);
			}


			#region output cache, THIS REGION IS ALWAYS AT LAST.
			if(_CacheDuration.ClientOnly || !IsCacheEnable || IsNoCache) {
				try {
					base.Render(writer);
				} catch {
					//Logger.Error(WebTools.GetCurrentAspxWithExtAndQueryString(), ex);
					Response.End();
				}
				return;
			}
			StringWriter stringWriter = new StringWriter();
			HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
			base.Render(htmlWriter);
			string html = stringWriter.ToString();

			// MinifyHtml need enhance before reopen
			// string html = WebUtility.MinifyHtml(stringWriter.ToString());

			if(Cache[_cacheKey] == null) {
				DateTime d = _CacheDuration.ExpireTime(Now);
				Cache.Insert(_cacheKey, html, null, d, Cache.NoSlidingExpiration);
				Cache.Insert(
					_cacheKeyDateTime,
					Now,
					new CacheDependency(null, new string[1] { _cacheKey })
				);
			}
			writer.Write(html);
			#endregion
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
		#endregion

		#region IPage
		#region Javascript (RunJs, Alert, JsConstant)
		public void JsRedirect(string url)
		{
			RunJs("location.replace({0});", ResolveUrl(url));
		}

		public void JsReload()
		{
			RunJs("location.replace(location);");
		}

		public void JsTopReload()
		{
			RunJs("top.location.replace(top.location);");
		}

		public void JsParentReload()
		{
			RunJs("parent.location.replace(parent.location);");
		}

		public void JsParentSubmit()
		{
			JsParentSubmit(0);
		}

		public void JsParentSubmit(int index)
		{
			if(index < 0) { index = 0; }
			RunJs(string.Format(
				"parent.document.forms[0].submit();",
				index
			));
		}

		public void JsSubmit()
		{
			JsSubmit(0);
		}

		public void JsSubmit(int index)
		{
			if(index < 0) { index = 0; }
			RunJs(string.Format(
				"window.document.forms[0].submit();",
				index
			));
		}

		public void JsClose()
		{
			RunJs("window.close();");
		}

		/// <summary>
		/// Run javascript on client window onload event
		/// Or include javascript file before body tag close.
		/// </summary>
		/// <param name="js">js with format string</param>
		/// <param name="args">format arguments</param>
		public void RunJs(string js, params object[] args)
		{
			RunJs(string.Format(js, args));
		}

		/// <summary>
		/// Run javascript on client window onload event
		/// Or include javascript file before body tag close.
		/// </summary>
		/// <param name="js">js with format string</param>
		public void RunJs(string js)
		{
			RunJs(js, false);
		}

		/// <summary>
		/// Run javascript on client window onload event
		/// Or include javascript file before body tag close.
		/// </summary>
		/// <param name="js">javascript statement or javascript file</param>
		/// <param name="defer">defer for javascript file</param>
		public void RunJs(string js, bool defer)
		{
			bool IsLocalFile =
				js.StartsWith("~")
				|| (js.StartsWith("/") && !js.StartsWith("//"))
				|| js.StartsWith("./")
				|| js.StartsWith("../");

			bool IsRemoteFile =
				js.StartsWith("//")
				|| js.StartsWith(WebConstants.Protocol.Http)
				|| js.StartsWith(WebConstants.Protocol.Https);

			if(IsLocalFile || IsRemoteFile) {
				if(WebCurrent.Mode != KernelMode.Developing) {
					if(js.EndsWith(".src.js")) {
						js = js.Replace(".src.js", ".js");
					} else if(js.EndsWith(".debug.js")) {
						js = js.Replace(".debug.js", ".js");
					}
				} else if(WebCurrent.Mode == KernelMode.Developing && js.EndsWith(".min.js")) {
					js = js.Replace(".min.js", ".js");
				}

				if(IsLocalFile) {
					js = ResolveClientUrl(js);
					js = WebTools.WrapVersion(js);
				} else {
					js = WebTools.RegulateUriProtocol(js);
				}

				ClientScript.RegisterStartupScript(
					this.GetType(),
					js,
					String.Format(
						"<script type=\"text/javascript\" src=\"{0}\"{1}></script>",
						WebTools.HtmlAttributeEncode(js),
						defer ? " defer=\"defer\"" : ""
					),
					false
				);
			} else {
				js = js.TrimEnd('\r', '\n', '\t');
				_RunJs.Append(js);

				if(!js.EndsWith(";") && !js.EndsWith("}")) {
					_RunJs.Append(";");
				}
			}
		}

		#region JSConstant

		/// <summary>
		/// Add constant Kuick.Any for client, for example, 
		/// if key is "KEY" and value is ( Any[] anys = new Any[] {new Any("abc", 123),
		/// new Any("def", 456),}; )
		/// you should use CONST.KEY in javascript to get the value
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="anys">Any array</param>
		public void JsConstant(string key, params Any[] anys)
		{
			JsConstant(key, Jsoner.Serialize(anys), false);
		}

		/// <summary>
		/// Add constant boolean value for client, for example, 
		/// if key is "KEY" and value is "true",
		/// you should use CONST.KEY in javascript to get the value "true"
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public void JsConstant(string key, bool value)
		{
			JsConstant(key, value ? "!0" : "!1", false);
		}

		/// <summary>
		/// Add constant string array for client, for example, 
		/// if key is "KEY" and array is "[7039,6997]",
		/// you should use CONST.KEY in javascript to get the array
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="values">values</param>
		public void JsConstant(string key, params int[] values)
		{
			StringBuilder sb = new StringBuilder();
			foreach(int i in values) {
				sb.Append(',').Append(i);
			}
			JsConstant(key, sb.RemoveFirst().Prepend("[").Append("]").ToString(), false);
		}

		/// <summary>
		/// Add constant value for client, for example, 
		/// if key is "KEY" and value is "Kuick consolting",
		/// you should use CONST.KEY in javascript to get the value "Kuick consolting"
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public void JsConstant(string key, int value)
		{
			JsConstant(key, value.ToString(), false);
		}

		/// <summary>
		/// Add constant value for client, for example, 
		/// if key is "KEY" and value is "Kuick consolting",
		/// you should use CONST.KEY in javascript to get the value "Kuick consolting"
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		public void JsConstant(string key, string value)
		{
			JsConstant(key, value, true);
		}

		/// <summary>
		/// Add constant string array for client, for example, 
		/// if key is "KEY" and array is "['Kuick','consolting']",
		/// you should use CONST.KEY in javascript to get the array
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="values">values</param>
		public void JsConstant(string key, params string[] values)
		{
			StringBuilder sb = new StringBuilder();
			foreach(string s in values) {
				sb.Append(',').Append(WebTools.JsString(s));
			}
			JsConstant(key, sb.RemoveFirst().Prepend("[").Append("]").ToString(), false);
		}

		/// <summary>
		/// Add constant value for client, for example, 
		/// if key is "KEY" and value is "Kuick consolting",
		/// you should use CONST.KEY in javascript to get the value "Kuick consolting"
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		/// <param name="ValueSurroundWithSingleQuotationMarks">
		/// when value is not type of string, such as object or array, 
		/// just set it to false
		/// </param>
		public void JsConstant(
			string key,
			string value,
			bool ValueSurroundWithSingleQuotationMarks)
		{
			value = ValueSurroundWithSingleQuotationMarks ? WebTools.JsString(value) : value;
			if(_Const.ContainsKey(key)) {
				_Const[key] = value;
			} else {
				_Const.Add(key, value);
			}
		}
		#endregion

		/// <summary>
		/// show alert
		/// </summary>
		/// <param name="msg">alert message</param>
		public string Alert(string msg)
		{
			Alert(msg, string.Empty);
			return msg;
		}

		/// <summary>
		/// show alert on client then redirect
		/// </summary>
		/// <param name="msg">alert message</param>
		/// <param name="url">redirect url</param>
		public string Alert(string msg, string url)
		{
			Alert(msg, url, false);
			return msg;
		}

		/// <summary>
		/// show alert on client then redirect(or replace)
		/// </summary>
		/// <param name="msg">alert message</param>
		/// <param name="url">redirect url</param>
		/// <param name="useReplace">
		/// replace current url, that user can't use back button
		/// </param>
		public string Alert(string msg, string url, bool useReplace)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("setTimeout(function(){");
			sb.AppendFormat("alert({0});", WebTools.JsString(msg));

			if(!String.IsNullOrEmpty(url)) {
				if(url.StartsWith("~")) { url = ResolveUrl(url); }

				sb.AppendFormat(
					useReplace ? "location.replace({0});" : "location.href={0};",
					WebTools.JsString(url)
				);
			}

			sb.Append("},0);");
			RunJs(sb.ToString());
			return msg;
		}

		public virtual bool RenderJQueryFile { get { return false; } }
		#endregion

		public bool Logon(string userName, string password, string domain)
		{
			// to do..
			return false;
		}

		public void UpdateSchemaControls(Api api)
		{
			// to do..
		}

		public void RegisterCSS(string href)
		{
			if(WebChecker.IsNull(href)) { return; }
			if(href.StartsWith("~")) { href = Page.ResolveUrl(href); }

			HtmlLink css = new HtmlLink();
			css.Href = href;
			css.Attributes["rel"] = "stylesheet";
			css.Attributes["type"] = "text/css";
			RegisterHeadControl(css);
		}

		public void RegisterJavaScript(string src)
		{
			if(WebChecker.IsNull(src)) { return; }
			if(src.StartsWith("~")) { src = Page.ResolveUrl(src); }

			HtmlGenericControl js = new HtmlGenericControl("script");
			js.Attributes["type"] = "text/javascript";
			js.Attributes["src"] = WebTools.RegulateUriProtocol(src);
			RegisterHeadControl(js);
		}

		public void RegisterMeta(string name, string property, string content)
		{
			if(WebChecker.IsNull(name) && WebChecker.IsNull(property)) { return; }
			LiteralControl lt = new LiteralControl();
			lt.Text = string.Format(
				"<meta {0}{1}{2}/>",
				WebChecker.IsNull(name) ? string.Empty : string.Format("name=\"{0}\" ", name),
				WebChecker.IsNull(property) ? string.Empty : string.Format("property=\"{0}\" ", property),
				WebChecker.IsNull(content) ? string.Empty : string.Format("content=\"{0}\" ", content)
			);
			RegisterHeadControl(lt);
		}

		public void RegisterLink(string rel, string href)
		{
			if(WebChecker.IsNull(href)) { return; }
			if(href.StartsWith("~")) { href = Page.ResolveUrl(href); }

			HtmlLink link = new HtmlLink();
			link.Href = href;
			link.Attributes["rel"] = rel;
			RegisterHeadControl(link);
		}

		public void RegisterHeadString(string str)
		{
			LiteralControl lt = new LiteralControl();
			lt.Text = str;
			RegisterHeadControl(lt);
		}

		// meta or anyothers by LiteralControl
		public void RegisterHeadControl(Control control)
		{
			base.Header.Controls.Add(control);
		}

		public PageAuthorizeAttribute PageAuthorize
		{
			get
			{
				if(WebChecker.IsNull(_PageAuthorize)) {
					lock(_Locking) {
						if(WebChecker.IsNull(_PageAuthorize)) {
							// PageAuthorizeAttribute
							object[] objs = this.GetType().BaseType.GetCustomAttributes(
								typeof(PageAuthorizeAttribute),
								true
							);
							foreach(object obj in objs) {
								if(obj is PageAuthorizeAttribute) {
									_PageAuthorize = (PageAuthorizeAttribute)obj;
									break;
								}
							}

							//if(WebChecker.IsNull(_PageAuthorize)) {
							//    _PageAuthorize = new PageAuthorizeAttribute(string.Empty);
							//}
						}
					}
				}
				return _PageAuthorize;
			}
		}

		//public virtual bool Authorize()
		//{
		//    return true;
		//}

		public PermissionAttribute Permission
		{
			get
			{
				if(WebChecker.IsNull(_Permission)) {
					lock(_Locking) {
						if(WebChecker.IsNull(_Permission)) {
							// PermissionAttribute
							object[] objs = this.GetType().BaseType.GetCustomAttributes(
								typeof(PermissionAttribute),
								true
							);
							foreach(object obj in objs) {
								if(obj is PermissionAttribute) {
									_Permission = (PermissionAttribute)obj;
									break;
								}
							}

							if(WebChecker.IsNull(_Permission)) {
								_Permission = new PermissionAttribute(PermissionAction.Non);
							}

							// DescriptionAttribute
							if(WebChecker.IsNull(_Permission.Description)) {
								object[] descs = this.GetType().BaseType.GetCustomAttributes(
									typeof(DescriptionAttribute),
									true
								);
								foreach(object obj in objs) {
									if(obj is DescriptionAttribute) {
										_Permission.Description =
											((DescriptionAttribute)obj).Description;
										break;
									}
								}
							}
						}
					}
				}
				return _Permission;
			}
		}

		public bool Permit()
		{
			return true;

			//if(Builtins.Security.IsNull) { return true; }
			//if(Permission.Action == PermissionAction.Non) { return true; }
			//if(Checker.IsNull(this.CurrentUser.Identity.Name)) {
			//    throw new ApplicationException(
			//        "Page design error: logon page can not add Permission attribute."
			//    );
			//}

			//return Proxy.Security.CheckPage(
			//    this.CurrentUser.Identity.Name,
			//    WebUtility.GetPathWithoutApplication(),
			//    Permission.Description
			//);
		}

		//public string GetMultilingual(string key)
		//{
		//    return GetMultilingual(key, string.Empty);
		//}
		//public string GetMultilingual(string key, string defaultValue)
		//{
		//    return Builtins.Lang.PathRead(
		//        CurrentUser.Culture.Name,
		//        WebUtility.GetFirstFolderName(),
		//        WebUtility.GetPathWithoutApplication(),
		//        defaultValue
		//    );
		//}

		/// <summary>
		/// meta tag with keywords, seaperate each keywords by comma.
		/// </summary>
		public string Keywords { get; set; }

		/// <summary>
		/// meta tag with description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// canonical link, please try to specify FULL URL.
		/// </summary>
		public string CanonicalLink { get; set; }

		public string ParameterToHidden(params string[] skipParameters)
		{
			bool skipEmpty = true;
			return ParameterToHidden(skipEmpty, skipParameters);
		}
		public string ParameterToHidden(bool skipEmpty, params string[] skipParameters)
		{
			return AttributeHelper.ParameterToHiddenFields(
				this,
				AttributeHelper.GetRequestParameterAttributes(this.GetType()),
				skipEmpty,
				skipParameters
			);
		}
		public void Redirect(string url)
		{
			Response.Redirect(url, true);
		}
		#endregion

		#region IWeb
		public IParameter Parameter
		{
			get
			{
				return WebBase.Parameter;
			}
		}

		public HttpContext GetCurrentContext()
		{
			return WebBase.GetCurrentContext();
		}

		public NameValueCollection GetRequestNVs()
		{
			return WebBase.GetRequestNVs();
		}

		public IParameter GetParameter()
		{
			return WebBase.GetParameter();
		}

		public string RequestValue(string name)
		{
			return WebBase.RequestValue(name);
		}

		public string RequestValue(string name, string airBag)
		{
			return WebBase.RequestValue(name, airBag);
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

		#region Properties
		public WebBase WebBase
		{
			get
			{
				if(WebChecker.IsNull(_WebBase)) {
					_WebBase = new WebBase(HttpContext.Current);
				}
				return _WebBase;
			}
		}

		public new PageBase Page
		{
			get
			{
				return this;
			}
		}

		public virtual DateTime LastModifiedTime
		{
			get
			{
				return Now;
			}
		}

		private bool IsCacheEnable
		{
			get
			{
				return WebCurrent.Mode != KernelMode.Developing
					&& _CacheDuration.Duration > 0;
			}
		}

		private bool IsNoCache
		{
			get
			{
				return null != Request[_CacheDuration.NoCacheParameter];
			}
		}
		private bool IsCleanCache
		{
			get
			{
				return null != Request[_CacheDuration.CleanCacheParameter];
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

		private static string MinifyJs(StringBuilder sb)
		{
			return WebCurrent.Mode == KernelMode.Developing
				? sb.ToString()
				: new JavaScriptMinifier().Minify(sb.Replace("\r", "\n").ToString());
		}

		private string GetKeyByParam(params string[] keys)
		{
			if(keys.Length == 0) { return String.Empty; }

			StringBuilder sb = new StringBuilder();
			foreach(var item in keys) {
				string name = item;
				string value = Request[item] as string;
				sb.AppendFormat(
					"&{0}={1}",
					name,
					null == value ? "" : UrlEncode(value)
				);
			}
			return sb.Remove(0, 1).ToString();
		}

		/// <summary>
		/// A shortcut for WebUtility.HtmlEncode
		/// </summary>
		/// <param name="o">object to HtmlEncode</param>
		/// <returns>string with Html encoded</returns>
		public static string HtmlEncode(object o)
		{
			return WebTools.HtmlEncode(o);
		}

		/// <summary>
		/// A shortcut for WebUtility.HtmlAttributeEncode
		/// </summary>
		/// <param name="o">object to HtmlAttributeEncode</param>
		/// <returns>string with Html encoded</returns>
		public static string HtmlAttributeEncode(object o)
		{
			return WebTools.HtmlAttributeEncode(o);
		}

		/// <summary>
		/// Shortcut to WebUtility.NotFound
		/// </summary>
		public static void NotFound()
		{
			WebTools.NotFound();
		}

		/// <summary>
		/// Shortcut to WebUtility.NotFound
		/// </summary>
		public static void NotFound(string msg)
		{
			WebTools.NotFound();
		}

		public static string UrlEncode(object o)
		{
			return WebTools.UrlEncode(Convert.ToString(o));
		}

		public static string UrlEncode(object o, Encoding e)
		{
			return WebTools.UrlEncode(Convert.ToString(o), e);
		}

		public bool FragmentAuthorize(string fragmentName)
		{
			return FragmentAuthorize(fragmentName, string.Empty);
		}

		private Anys _AuthorizedFragments; // cache
		public bool FragmentAuthorize(string fragmentName, string description)
		{
			if(null == _AuthorizedFragments) { _AuthorizedFragments = new Anys(); }
			if(_AuthorizedFragments.Exists(fragmentName)) {
				return _AuthorizedFragments.ToBoolean(fragmentName);
			}


			if(Authorizer.FragmentAuthorize(this, fragmentName, description)) {
				_AuthorizedFragments.Add(fragmentName, true);
				return true;
			} else {
				Errors.Add(fragmentName, description);
				_AuthorizedFragments.Add(fragmentName, false);
				return false;
			}
		}
		#endregion
	}
}

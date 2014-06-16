// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebUtility.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.MobileControls.Adapters;
using Kuick.Data;
using System.Net;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Kuick.Web
{
	public class WebTools : WebUtility
	{
	}

	//[Obsolete("Instead, use WebTools class.")]
	public class WebUtility
	{
		private static string _LogPhysicalPath;
		private static string _SettingPhysicalPath;

		private static string _DEFAULT_JPG = "default.jpg";

		public static string WrapVersion(string file)
		{
			string ver = string.Empty;
			try {
				string path = string.Format(file, string.Empty);
				if(file.StartWith("~", "/", ".")) {
					path = HttpContext.Current.Server.MapPath(path);
				}
				if(File.Exists(path)) {
					ver = File.GetLastWriteTime(path).ToString("MMHHddyymm");
				}
			} catch {
				// if any access error, ignore it.
			}
			return file.Contains("{0}")
				? string.Format(file, ver)
				: string.IsNullOrEmpty(ver) ? file : string.Concat(file, "?", ver);
		}

		public static string JsString(string str)
		{
			return JsString(str, false);
		}

		public static string JsString(string str, bool useDoubleQuotation)
		{
			return useDoubleQuotation
				? string.Concat("\"", Formator.JavaScriptEncode(str, false), "\"")
				: Formator.JavaScriptEncode(str, true);
		}

		public static string HtmlEncode(object o)
		{
			return Formator.HtmlEncode(
				WebChecker.IsNull(o) ? string.Empty : Convert.ToString(o)
			);
		}


		public static string HtmlAttributeEncode(object o)
		{
			return Microsoft.Security.Application.Encoder.HtmlAttributeEncode(
				WebChecker.IsNull(o) ? string.Empty : Convert.ToString(o)
			);
		}

		public static string UrlEncode(object o)
		{
			return Microsoft.Security.Application.Encoder.UrlEncode(
				WebChecker.IsNull(o) ? string.Empty : Convert.ToString(o)
			);
		}

		public static string UrlEncode(object o, Encoding e)
		{
			return UrlEncode(o, e.CodePage);
		}

		public static string UrlEncode(object o, int codePage)
		{
			return Microsoft.Security.Application.Encoder.UrlEncode(
				WebChecker.IsNull(o) ? string.Empty : Convert.ToString(o), codePage
			);
		}

		public static string AirBagXHtml(object o)
		{
			return Regex.Replace(
				AirBagHtml(o),
				@"(<(img|br|hr)(\s[^>]+)?)>",
				"$1 />",
				RegexOptions.Compiled
			);
		}

		public static string AirBagHtml(object o)
		{
			return Sanitizer.GetSafeHtmlFragment(
				WebChecker.IsNull(o) ? string.Empty : Convert.ToString(o)
			);
		}

		public static string ImgHtml(string file)
		{
			return ImgHtml(file, new Dictionary<string, string>());
		}

		public static string ImgHtml(
			string file, Dictionary<string, string> attributes)
		{
			if(string.IsNullOrEmpty(file)) { return string.Empty; }

			HtmlImage img = new HtmlImage();
			StringBuilder sb = new StringBuilder();

			if(file.StartWith("~", "/")) {
				if(!(attributes.ContainsKey(Html.Width) && attributes.ContainsKey(Html.Height))) {
					try {
						string f = HttpContext.Current.Server.MapPath(file);
						if(File.Exists(f)) {
							int width = 0;
							int hieght = 0;
							using(System.Drawing.Image image = Bitmap.FromFile(f)) {
								width = image.Width;
								hieght = image.Height;
							}
							if(width != 0 && hieght != 0) {
								int t = 0;
								bool b = false;
								if(attributes.ContainsKey(Html.Width)) {
									if(b = int.TryParse(attributes[Html.Width], out t)) {
										hieght = Convert.ToInt32(
											hieght * ((float)t / (float)width)
										);
										width = t;
										if(hieght != 0) {
											attributes.Add(Html.Height, hieght.ToString());
										}
										attributes[Html.Width] = width.ToString();
									}
								} else if(attributes.ContainsKey(Html.Height)) {
									if(b = int.TryParse(attributes[Html.Height], out t)) {
										width = Convert.ToInt32(
											width * ((float)t / (float)hieght)
										);
										hieght = t;
										if(width != 0) {
											attributes.Add(
												Html.Width, width.ToString()
											);
										}
										attributes[Html.Height] = hieght.ToString();
									}
								}
								if(!b) {
									attributes.Add(Html.Width, width.ToString());
									attributes.Add(Html.Height, hieght.ToString());
								}
							}
						}
					} catch {
					}
				}
				file = ResolveUrl(file);
			}

			// img must have alt attribute
			if(!attributes.ContainsKey(Html.Alt)) {
				attributes.Add(Html.Alt, String.Empty);
			}

			// ignore the src specify by parameter attributes
			if(attributes.ContainsKey(Html.Src)) { attributes.Remove(Html.Src); }

			attributes.Add(Html.Src, file);
			foreach(var item in attributes) {
				img.Attributes.Add(item.Key, item.Value);
			}

			StringWriter sw = new StringWriter();
			img.RenderControl(new HtmlTextWriter(sw));
			sw.Flush();
			return sw.ToString();
		}

		public static string GetCurrentUserLanguage()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return WebConstants.Default.UserLanguage;
			}

			string[] userLang = HttpContext.Current.Request.UserLanguages;
			return WebChecker.IsNull(userLang)
				? WebConstants.Default.UserLanguage
				: userLang[0];
		}

		public static string GetWebRoot()
		{
			string webRoot = Current.Application.GetString("Web", "WebRoot", "");
			if(!string.IsNullOrEmpty(webRoot)) { return webRoot; }

			string host = (HttpContext.Current.Request.Url.IsDefaultPort) ?
				HttpContext.Current.Request.Url.Host :
				HttpContext.Current.Request.Url.Authority;
			host = String.Format(
				"{0}://{1}",
				HttpContext.Current.Request.Url.Scheme,
				host
			);
			if(HttpContext.Current.Request.ApplicationPath == "/") {
				return host;
			} else {
				return host + HttpContext.Current.Request.ApplicationPath;
			}
		}

		public static string GetApplicationPath()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return WebConstants.Symbol.Slash;
			}
			return HttpContext.Current.Request.ApplicationPath;
		}

		public static string FixUrl(string url)
		{
			return Regex.Replace(url, "\\/+", "/", RegexOptions.Compiled);
		}

		public static string GetCurrentFullPath()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return string.Empty;
			}
			return HttpContext.Current.Request.Path;
		}

		public static string GetCurrentAspxName()
		{
			string aspx = GetCurrentAspx();
			int start = aspx.Contains(WebConstants.Symbol.Slash)
				? aspx.LastIndexOf(WebConstants.Symbol.Char.Slash) + 1
				: 0;
			return aspx.Substring(start);
		}

		public static string GetCurrentAspx()
		{
			string aspx = GetCurrentAspxWithExt();
			return aspx.Substring(
				0,
				aspx.LastIndexOf(WebConstants.Symbol.Char.Period)
			);
		}

		public static string GetCurrentAspxWithExt()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return string.Empty;
			}

			int subStrLength;
			subStrLength = (GetApplicationPath() == WebConstants.Symbol.Slash)
				? 1
				: HttpContext.Current.Request.ApplicationPath.Length + 1;
			string aspx = HttpContext.Current.Request.Path.Substring(subStrLength);
			return aspx;
		}

		public static string GetCurrentAspxWithExtAndQueryString()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return string.Empty;
			}
			int subStrLength;
			if(GetApplicationPath() == WebConstants.Symbol.Slash) {
				subStrLength = 1;
			} else {
				subStrLength = HttpContext.Current.Request.RawUrl.LastIndexOf(
					WebConstants.Symbol.Char.Slash
				) + 1;
			}
			string aspx = HttpContext.Current.Request.RawUrl.Substring(subStrLength);
			return aspx;
		}

		public static string GetPathWithoutApplication()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return string.Empty;
			}
			int subStrLength;
			subStrLength =
				(
					HttpContext.Current.Request.ApplicationPath
					==
					WebConstants.Symbol.Slash
				)
				? 0
				: HttpContext.Current.Request.ApplicationPath.Length;
			string aspx = HttpContext
				.Current
				.Request
				.CurrentExecutionFilePath
				.Substring(subStrLength);
			return aspx;
		}

		public static string GetRawUrlWithoutApplication()
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return string.Empty;
			}
			int subStrLength;
			subStrLength =
				(
					HttpContext.Current.Request.ApplicationPath
					==
					WebConstants.Symbol.Slash
				)
				? 0
				: HttpContext.Current.Request.ApplicationPath.Length;
			string aspx = HttpContext.Current.Request.RawUrl.Substring(subStrLength);
			return aspx;
		}

		public static string GetFirstFolderName()
		{
			string path = GetPathWithoutApplication();
			if(WebChecker.IsNull(path)) { return string.Empty; }
			if(path.StartWith(WebConstants.Symbol.Slash)) {
				path = path.Substring(1);
			}
			int pos = path.IndexOf(WebConstants.Symbol.Slash);
			return pos > 0 ? path.Substring(0, pos) : string.Empty;
		}

		public static string GetCurrentUserName()
		{
			return HttpContext.Current.User.Identity.Name;
		}

		public static string GetServerIP()
		{
			if(
				null == HttpContext.Current
				||
				null == HttpContext.Current.Request) {
				return string.Empty;
			}

			return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
		}

		public static string GetClientIP()
		{
			if(
				null == HttpContext.Current
				||
				null == HttpContext.Current.Request) {
				return string.Empty;
			}

			string ip = string.Empty;
			string x = HttpContext
				.Current
				.Request
				.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if(string.IsNullOrEmpty(x)) {
				ip = HttpContext
					.Current
					.Request
					.ServerVariables["REMOTE_ADDR"];
			} else {
				if(x.Contains(",")) {
					string[] ips = ip.Split(',');
					ip = ips[0];
				} else {
					ip = x;
				}
			}
			//if(ip == "::1") { ip = Constants.LocalIp; } // IPv6
			return ip;
		}

		public static List<string> GetUrlIPs(string url)
		{
			List<string> list = new List<string>();
			try {
				Uri uri = new Uri(url);
				IPAddress[] addresses = Dns.GetHostAddresses(uri.Authority);
				foreach(var address in addresses) {
					list.Add(address.ToString());
				}
			} catch(Exception ex) {
				Logger.Track(
					"WebUtility.GetUrlIPs",
					ex.ToAny(
						new Any("Url", url)
					)
				);
				throw;
			} finally {
			}
			return list;
		}

		public static string ToApplicationFilePath(string projectFilePath)
		{
			StringBuilder sb = new StringBuilder(GetApplicationPath());
			if(projectFilePath.StartsWith(WebConstants.Symbol.Tilde)) {
				projectFilePath = projectFilePath.Substring(1);
			}
			if(!projectFilePath.StartsWith(WebConstants.Symbol.Slash)) {
				sb.Append(WebConstants.Symbol.Char.Slash);
			}
			sb.Append(projectFilePath);
			return sb.ToString().Replace(
				WebConstants.Symbol.Slash + WebConstants.Symbol.Slash,
				WebConstants.Symbol.Slash
			);
		}

		public static string ToPhysicalApplicationFilePath(string projectFilePath)
		{
			try {
				string appFilePath = ToApplicationFilePath(projectFilePath);
				string physicalFilePath = HttpContext
					.Current
					.Server
					.MapPath(appFilePath);
				return physicalFilePath;
			} catch(Exception ex) {
				Logger.Error(ex);
				return projectFilePath;
			}
		}

		public static string GetLogPhysicalPath()
		{
			if(null == _LogPhysicalPath) {
				_LogPhysicalPath = ToPhysicalApplicationFilePath(
					WebCurrent.Log.Folder
				);
			}
			return _LogPhysicalPath;
		}

		public static string GetLogPhysicalPathFile(string fileName)
		{
			if(!Path.GetExtension(fileName).Equals(
				WebConstants.File.Extension.LOG,
				StringComparison.OrdinalIgnoreCase)) {
				fileName = Path.ChangeExtension(
					fileName,
					WebConstants.File.Extension.LOG
				);
			}
			return Path.Combine(GetLogPhysicalPath(), fileName);
		}

		public static string GetSettingPhysicalPath()
		{
			if(null == _SettingPhysicalPath) {
				_SettingPhysicalPath = ToPhysicalApplicationFilePath(
					WebCurrent.Setting.Folder
				);
			}
			return _SettingPhysicalPath;
		}

		public static string GetConfigPhysicalPathFile(string fileName)
		{
			if(!Path.GetExtension(fileName).Equals(
				WebConstants.File.Extension.CONFIG,
				StringComparison.OrdinalIgnoreCase)) {
				fileName = Path.ChangeExtension(
					fileName,
					WebConstants.File.Extension.CONFIG
				);
			}
			return Path.Combine(GetSettingPhysicalPath(), fileName);
		}

		public static Control PowerFindControl(Control control, string controlID)
		{
			Control namingContainer = control;
			Control ctrl = null;
			if(control != control.Page) {
				while((ctrl == null) && (namingContainer != control.Page)) {
					namingContainer = namingContainer.NamingContainer;
					if(namingContainer == null) {
						throw new HttpException(
							SR.GetString(
								"DataBoundControlHelper_NoNamingContainer",
								new object[] { control.GetType().Name, control.ID }
							)
						);
					}
					ctrl = namingContainer.FindControl(controlID);
				}
				return ctrl;
			}
			return control.FindControl(controlID);
		}

		public static string ResolveUrl(string url)
		{
			return ResolveUrl(url, false);
		}

		public static string ResolveUrl(string url, bool absoluted)
		{
			if(!absoluted) {
				var ctrl = new Control();

				//in MVC, control resolveUrl will fail.
				// When it happend, use absolute instead.
				if(ctrl.ResolveUrl("~/") != "~/") {
					return ctrl.ResolveUrl(url);
				}
			}

			if(url.StartWith(
				WebConstants.Protocol.Http,
				WebConstants.Protocol.Https)) {
				return url;
			}

			HttpRequest Request = HttpContext.Current.Request;
			string ap = Request.ApplicationPath;
			StringBuilder sb = new StringBuilder(url);
			if(!url.StartWith("~/", "/")) {
				string filepath = Request.FilePath;
				string file = Path.GetFileName(Request.FilePath);
				sb
					.Prepend(filepath)
					.Remove(filepath.Length - file.Length, file.Length);
			}

			if(!ap.EndsWith(WebConstants.Symbol.Slash)) {
				ap += WebConstants.Symbol.Slash;
			}
			return sb.Replace("~/", ap).Replace("\\", "/").ToString();
		}

		public static string ResolveUrl(IEntity entity, string columnNameOrPropName)
		{
			return ResolveUrl(entity, columnNameOrPropName, _DEFAULT_JPG, false);
		}

		public static string ResolveUrl(
			IEntity entity,
			string columnNameOrPropName,
			bool absoluted)
		{
			return ResolveUrl(entity, columnNameOrPropName, _DEFAULT_JPG, absoluted);
		}
		public static string ResolveUrl(
			IEntity entity,
			string columnNameOrPropName,
			string defaultFile)
		{
			bool absoluted = false;
			return ResolveUrl(entity, columnNameOrPropName, defaultFile, absoluted);
		}
		public static string ResolveUrl(
			IEntity entity,
			string columnNameOrPropName,
			string defaultFile, bool absoluted)
		{
			string file = Convert.ToString(entity.GetValue(columnNameOrPropName));
			if(string.IsNullOrEmpty(file)) { file = defaultFile; }
			return ResolveUrl(Path.Combine(entity.FileRoot, file), absoluted);
		}

		public static string ResolveUrl(string pattern, string key, string airBag)
		{
			string path = Exists(string.Format(pattern, key))
				? string.Format(pattern, key)
				: string.Format(pattern, airBag);
			return WebTools.ResolveUrl(path);
		}

		public static bool Exists(string url)
		{
			return File.Exists(HttpContext.Current.Server.MapPath(url));
		}

		public static bool IsImage(string file)
		{
			return Regex.IsMatch(
				Path.GetExtension(file), "^\\.(jpg|png|gif)$",
				RegexOptions.IgnoreCase | RegexOptions.Compiled
			);
		}

		public static string RegulateUriProtocol(string uri)
		{
			if(WebChecker.IsNull(HttpContext.Current.Request)) {
				return string.Empty;
			}

			string schema = HttpContext.Current.Request.Url.Scheme;

			if(
				uri.StartWith(WebConstants.Protocol.Http)
				&&
				schema == Uri.UriSchemeHttps) {
				uri = WebConstants.Protocol.Https + uri.Substring(7);
			} else if(
				uri.StartWith(WebConstants.Protocol.Https)
				&&
				schema == Uri.UriSchemeHttp) {
				uri = WebConstants.Protocol.Http + uri.Substring(8);
			}

			return uri;
		}

		public static void ThrowHttpException(HttpStatusCode code, string msg = null)
		{
			throw new HttpException(
				(int)code,
				String.IsNullOrEmpty(msg) ? code.ToString() : msg
			);
		}

		public static void NotFound(string msg = null)
		{
			ThrowHttpException(HttpStatusCode.NotFound, msg);
		}

		public static void BadRequest(string msg = null)
		{
			ThrowHttpException(HttpStatusCode.BadRequest, msg);
		}

		public static void Forbidden(string msg = null)
		{
			ThrowHttpException(HttpStatusCode.Forbidden, msg);
		}

		public static void MovedPermanently(string url)
		{
			MovedPermanently(url, true);
		}
		public static void MovedPermanently(string url, bool endResponse)
		{
			HttpResponse Response = HttpContext.Current.Response;
			Response.RedirectPermanent(url, endResponse);
		}

		public string MineUrl(
			string url, string filter, string startTag, string endTag)
		{
			WebClient client = new WebClient();
			string body = client.DownloadString(url);
			int offFound = body.IndexOf(filter);
			if(offFound <= 0) {
				// Filter not found!
				return string.Empty;
			} else {
				int findNextStart = body.IndexOf(startTag, offFound);
				if(findNextStart <= 0) {
					// StartTag not found!
					return string.Empty;
				} else {
					int findNextEnd = body.IndexOf(
						endTag, findNextStart + startTag.Length
					);
					if(findNextStart <= 0) {
						// EndTag not found!
						return string.Empty;
					} else {
						return body.Substring(
							findNextStart + startTag.Length,
							findNextEnd - findNextStart - startTag.Length
						);
					}
				}
			}
		}

		public static void Bind(Page page, object obj)
		{
			PropertyInfo[] infos = obj.GetType().GetProperties();
			ControlCollection cc = (null == page.Master)
				? page.Controls
				: page.Master.Controls;
			foreach(PropertyInfo one in obj.GetType().GetProperties()) {
				Control ctl = null;
				foreach(Control c in cc) {
					ctl = FindControl(c, one.Name);
					if(null == ctl) {
						ctl = FindControl(
							c,
							WebConstants.Html.ControlPrefix.Checkbox + one.Name
						);
					}
					if(null == ctl) {
						ctl = FindControl(
							c,
							WebConstants.Html.ControlPrefix.TextBox + one.Name
						);
					}
					if(null == ctl) {
						ctl = FindControl(
							c,
							WebConstants.Html.ControlPrefix.DropDownList + one.Name
						);
					}

					if(null != ctl) { break; }
				}

				if(null == ctl) { continue; }

				object val = Reflector.GetValue(one.Name, obj);
				string value = (null == val) ? string.Empty : val.ToString();
				if(ctl is CheckBox) {
					((CheckBox)ctl).Checked = value.AirBagToBoolean();
				} else if(ctl is TextBox) {
					((TextBox)ctl).Text = value.AirBag();
				} else if(ctl is DropDownList) {
					DropDownList ddl = (DropDownList)ctl;
					if(ddl.Items.Count == 0 && obj is IEntity) {
						IEntity instance = (IEntity)obj;
						Column column = instance.GetColumn(one.Name);
						if(!WebChecker.IsNull(column)) {
							if(!column.Spec.NotAllowNull) {
								ddl.Items.Add(
									new ListItem("-- select --", string.Empty)
								);
							}
							throw new NotImplementedException();
							//ddl.Items.AddRange(column.Refer.References);
						}
					}
					ddl.SelectedValue = value.AirBag();
				}
			}
		}

		public static Control FindControl(Control control, string id)
		{
			if(WebChecker.IsNull(control)) { return null; }
			if(control.ID == id) { return control; }
			foreach(Control c in control.Controls) {
				Control one = FindControl(c, id);
				if(null != one) { return one; }
			}
			return null;
		}

		/// <summary>
		/// minified the input html
		/// </summary>
		/// <param name="html">html to minify</param>
		/// <returns></returns>
		public static string MinifyHtml(string html)
		{
			html = Regex.Replace(
				html, @"^\s+|\s+$", "", RegexOptions.Compiled
			);
			html = Regex.Replace(
				html,
				@">([\r\n\s]*|^<)",
				">",
				RegexOptions.Compiled | RegexOptions.Multiline
			);
			html = Regex.Replace(
				html,
				@"([\n\r\s]*|^>)<",
				"<",
				RegexOptions.Compiled | RegexOptions.Multiline
			);
			return html;
		}

		private static WebServerType _CurrentWebServerType = WebServerType.Unknown;
		public static WebServerType CurrentWebServerType
		{
			get
			{
				if(_CurrentWebServerType == WebServerType.Unknown) {

					IServiceProvider provider = HttpContext.Current;
					if(null == provider) { return WebServerType.Unknown; }

					var worker = provider.GetService(typeof(HttpWorkerRequest));
					switch(worker.GetType().Name) {
						case WebConstants.WebServerName.VisualStudioDefaultWebServer:
							_CurrentWebServerType =
								WebServerType.VisualStudioDefaultWebServer;
							break;
						case WebConstants.WebServerName.VisualStudio2012:
							_CurrentWebServerType = WebServerType.VS2012;
							break;
						case WebConstants.WebServerName.IIS6:
							_CurrentWebServerType = WebServerType.IIS6;
							break;
						case WebConstants.WebServerName.IIS7:
							_CurrentWebServerType = WebServerType.IIS7;
							break;
						default:
							_CurrentWebServerType = WebServerType.Unknown;
							break;
					}
				}
				return _CurrentWebServerType;
			}
		}

		public static string BuildQueryString(params Any[] anys)
		{
			return BuildQueryString(string.Empty, anys);
		}

		public static string BuildQueryString(string url, params Any[] anys)
		{
			if(string.IsNullOrEmpty(url)) {
				url = HttpContext.Current.Request.Url.AbsolutePath;
			} else {
				url = ResolveUrl(url);
			}
			NameValueCollection col = HttpUtility.ParseQueryString(string.Empty);
			if(null != anys || anys.Length > 0) {
				foreach(Any any in anys) {
					if(null == any) { continue; }
					col[any.Name] = any.ToString().UrlEncode();
				}
			}
			string parameters = col.ToString();
			if(string.IsNullOrEmpty(parameters)) {
				return url;
			} else {
				return string.Concat(url, "?", parameters);
			}
		}

		public static string RenderControl(Control control)
		{
			using(TextWriter txtWriter = new StringWriter()) {
				using(HtmlTextWriter htmlWriter = new HtmlTextWriter(txtWriter)) {
					control.RenderControl(htmlWriter);
					htmlWriter.Flush();
					return txtWriter.ToString();
				}
			}
		}

		public static Anys QueryStringToAnys(string queryString)
		{
			return queryString.ToAnys(
				Constants.Symbol.Ampersand,
				Constants.Symbol.Equal
			);
		}

		public static string AnysToQueryString(params Any[] anys)
		{
			return new Anys(anys).Join(
				Constants.Symbol.Ampersand,
				Constants.Symbol.Equal
			);
		}
	}
}

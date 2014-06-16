// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AttributeHelper.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Kuick.Data;
using System.Web.UI;
using Newtonsoft.Json;

namespace Kuick.Web
{
	internal sealed class AttributeHelper
	{
		private static Dictionary<Type, Dictionary<Type, object[]>> _Cache =
			new Dictionary<Type, Dictionary<Type, object[]>>();
		private static object _Lock = new object();
		private static object _lock = new object();

		/// <summary>
		/// Get first attribute(T) from given type, not include inhert attributes
		/// </summary>
		/// <typeparam name="T">must inherit from Attribute</typeparam>
		/// <param name="type">The type you want to get from</param>
		/// <returns></returns>
		public static T Get<T>(Type type) where T : Attribute
		{
			List<T> ts = GetAll<T>(type);
			return (ts != null && ts.Count > 0) ? ts[0] : default(T);
		}

		/// <summary>
		/// Get attributes(T) from given type, not include inhert attributes
		/// </summary>
		/// <typeparam name="T">must inherit from Attribute</typeparam>
		/// <param name="type">The type you want to get from</param>
		/// <returns></returns>
		public static List<T> GetAll<T>(Type type) where T : Attribute
		{
			Type attributeType = typeof(T);
			Dictionary<Type, object[]> attributes = null;

			lock(_Lock) {
				if(_Cache.ContainsKey(type)) {
					attributes = _Cache[type];
					if(attributes.ContainsKey(attributeType)) {
						object[] typeAttributes = attributes[attributeType];
						if(typeAttributes != null) {
							return new List<T>(typeAttributes as T[]);
						}
					}
				} else {
					attributes = new Dictionary<Type, object[]>(1);
					try {
						_Cache.Add(type, attributes);
					} catch(Exception ex) {
						Logger.Error("AttributeHelper.GetAll", ex.ToAny());
					}
				}

				if(!attributes.ContainsKey(attributeType)) {
					lock(_lock) {
						if(!attributes.ContainsKey(attributeType)) {
							object[] filters = type.GetCustomAttributes(typeof(T), false);
							attributes.Add(attributeType, filters);
						}
					}
				}
			}
			return new List<T>(attributes[attributeType] as T[]);
		}

		public static void CheckAcceptFilter(Type type, HttpRequest request)
		{
			AcceptFilterAttribute[] filters =
				(AcceptFilterAttribute[])type.GetCustomAttributes(
					typeof(AcceptFilterAttribute),
					false
				);
			foreach(var af in filters) {
				Result r = af.Valid(request);
				if(!r.Success) {
					throw new HttpException((int)HttpStatusCode.Forbidden, r.Message);
				}
			}
		}

		#region CacheDuration
		private static Dictionary<Type, CacheDurationEntityNameAttribute>
			_CacheDurationEntityNameAttributeCache =
			new Dictionary<Type, CacheDurationEntityNameAttribute>();
		public static CacheDurationEntityNameAttribute GetCacheDurationEntityNameAttributes(
			Type type)
		{
			if(!_CacheDurationEntityNameAttributeCache.ContainsKey(type)) {
				lock(_lock) {
					if(!_CacheDurationEntityNameAttributeCache.ContainsKey(type)) {
						PropertyInfo[] infos = type.GetProperties();
						foreach(PropertyInfo info in infos) {
							CacheDurationEntityNameAttribute[] objs =
								info.GetCustomAttributes(
									typeof(CacheDurationEntityNameAttribute),
									false
								) as CacheDurationEntityNameAttribute[];

							if(objs == null || objs.Length == 0) { continue; }
							CacheDurationEntityNameAttribute arg = objs[0];
							arg.PropertyInfo = info;
							_CacheDurationEntityNameAttributeCache.Add(type, arg);
							return arg;
						}
						return null;
					}
				}
			}
			return _CacheDurationEntityNameAttributeCache[type];
		}

		private static Dictionary<Type, CacheDurationEntityKeyValueAttribute>
			_CacheDurationEntityKeyValueAttributeCache =
			new Dictionary<Type, CacheDurationEntityKeyValueAttribute>();
		public static CacheDurationEntityKeyValueAttribute GetCacheDurationEntityKeyValueAttributes(
			Type type)
		{
			if(!_CacheDurationEntityKeyValueAttributeCache.ContainsKey(type)) {
				lock(_lock) {
					if(!_CacheDurationEntityKeyValueAttributeCache.ContainsKey(type)) {
						PropertyInfo[] infos = type.GetProperties();
						foreach(PropertyInfo info in infos) {
							CacheDurationEntityKeyValueAttribute[] objs =
								info.GetCustomAttributes(
									typeof(CacheDurationEntityKeyValueAttribute),
									false
								) as CacheDurationEntityKeyValueAttribute[];

							if(objs == null || objs.Length == 0) { continue; }
							CacheDurationEntityKeyValueAttribute arg = objs[0];
							if(null == arg) { continue; }
							arg.PropertyInfo = info;
							_CacheDurationEntityKeyValueAttributeCache.Add(type, arg);
							return arg;
						}
						return null;
					}
				}
			}
			return _CacheDurationEntityKeyValueAttributeCache[type];
		}

		public static void SetCacheDurationEntityNameAndKeyValue(
			IWeb page,
			CacheDurationEntityNameAttribute attrEntityName,
			CacheDurationEntityKeyValueAttribute attrEntityKeyValue,
			CacheDurationAttribute cacheDuration)
		{
			if(null == cacheDuration) { return; }

			string entityName = (null == attrEntityName)
				? string.Empty
				: Reflector.GetValue(
						attrEntityName.PropertyInfo.PropertyType.Name,
						page
					) as string;
			string entityKeyValue = (null == attrEntityKeyValue)
				? string.Empty
				: Reflector.GetValue(
						attrEntityKeyValue.PropertyInfo.PropertyType.Name,
						page
					) as string;
			if(WebChecker.IsNull(entityName)) { return; }
			cacheDuration.SetAssociatedWithEntity(entityName, entityKeyValue);
		}
		#endregion

		#region Request Parameter Helper
		private static Dictionary<Type, List<RequestParameterAttribute>>
			_RequestParameterAttributeCache =
			new Dictionary<Type, List<RequestParameterAttribute>>();
		public static List<RequestParameterAttribute> GetRequestParameterAttributes(Type type)
		{
			if(!_RequestParameterAttributeCache.ContainsKey(type)) {
				lock(_lock) {
					if(!_RequestParameterAttributeCache.ContainsKey(type)) {

						List<RequestParameterAttribute> paramList =
							new List<RequestParameterAttribute>();

						PropertyInfo[] infos = type.GetProperties();
						foreach(PropertyInfo info in infos) {
							RequestParameterAttribute[] objs =
								info.GetCustomAttributes(
									typeof(RequestParameterAttribute),
									false
								) as RequestParameterAttribute[];

							if(objs == null || objs.Length == 0) { continue; }
							RequestParameterAttribute arg = objs[0];
							arg.Validations = info.GetCustomAttributes(
								typeof(IValidation),
								false
							) as IValidation[];

							if(WebChecker.IsNull(arg.Title)) { arg.Title = info.Name; }
							arg.PropertyInfo = info;

							paramList.Add(arg);
						}
						_RequestParameterAttributeCache.Add(type, paramList);
					}
				}
			}
			return _RequestParameterAttributeCache[type];
		}


		public static void ValidateParameter(
			IWeb page,
			List<RequestParameterAttribute> parameters)
		{
			HttpRequest Request = page.GetCurrentContext().Request;

			foreach(var param in parameters) {

				if(param.IsBinary) {
					HttpPostedFile file = Request.Files[param.Title];
					string fileName = String.Empty;

					if(null == file && !param.ThrowNotFound) {
						continue;
					}

					if(null != file && file.ContentLength != 0) {
						fileName = file.FileName;
					}
					foreach(var validator in param.Validations) {
						Result r = validator.Validate(fileName);
						if(!r.Success) {
							Logger.Error(
								r.Exception,
								"Validate Error",
								new Any("Title", param.Title),
								new Any("Value", file.FileName),
								new Any("Message", r.Message)
							);
							WebTools.BadRequest(String.Format(
								"Request filename of \"{0}\" is not valid.",
								param.Title
							));
						}
					}
				} else {
					string val;
					switch(param.ValueFrom) {
						case RequestType.Query:
							val = Request.QueryString[param.Title];
							break;
						case RequestType.Form:
							val = Request.Form[param.Title];
							break;
						case RequestType.Cookies:
							val = CookieManager.Get(param.Title);
							break;
						default:
							val = page.RequestValue(param.Title);
							break;
					}

					if(null == val && !param.ThrowNotFound) {
						continue;
					}

					foreach(var validator in param.Validations) {
						Result r = validator.Validate(val);
						if(!r.Success) {
							Logger.Error(
								r.Exception,
								new Any("Title", param.Title),
								new Any("Value", val)
							);
							WebTools.BadRequest(String.Format(
								"Request value of \"{0}\" is not valid.",
								param.Title
							));
						}
					}
				}
			}
		}

		public static void SetValueByParameter(
			IWeb page,
			List<RequestParameterAttribute> parameters)
		{
			SetValueByParameter(page, page, parameters);
		}

		public static void SetValueByParameter(
			UserControlBase userControl,
			List<RequestParameterAttribute> parameters)
		{
			SetValueByParameter(userControl.Page, userControl, parameters);
		}

		public static void SetValueByParameter(
			MasterPageBase master,
			List<RequestParameterAttribute> parameters)
		{
			SetValueByParameter(master.Page, master, parameters);
		}

		private static void SetValueByParameter(
			IWeb iWeb,
			object obj,
			List<RequestParameterAttribute> parameters)
		{
			HttpRequest request = iWeb.GetCurrentContext().Request;

			foreach(var param in parameters) {
				if(param.IsBinary) {
					Type t = param.PropertyInfo.PropertyType;
					if(t.Equals((typeof(System.IO.Stream)))) {
						HttpPostedFile file = GetFile(request.Files, param.Title);
						Stream s = (null == file || file.ContentLength == 0)
							? new byte[0].ToStream()
							: file.InputStream;
						Reflector.SetValue(obj, param.PropertyInfo, s);
					} else if(t.Equals((typeof(Byte[])))) {
						HttpPostedFile file = GetFile(request.Files, param.Title);
						byte[] vals = (null == file || file.ContentLength == 0)
							? new byte[0]
							: file.InputStream.ToBytes();
						Reflector.SetValue(obj, param.PropertyInfo, vals);
					}
				} else {
					object val;
					switch(param.ValueFrom) {
						case RequestType.Query:
							val = request.QueryString[param.Title];
							break;
						case RequestType.Form:
							val = request.Form[param.Title];
							break;
						case RequestType.Cookies:
							val = CookieManager.Get(param.Title);
							break;
						default:
							val = iWeb.RequestValue(param.Title);
							break;
					}

					if(param.IsEntity) {
						Type t = param.PropertyInfo.PropertyType;
						object v = null;
						string str = Convert.ToString(val);
						if(param.FromJson && Checker.MayBeJson(str)) {
							v = JsonConvert.DeserializeObject(str, t);
						} else {
							v = Entity.Get(t.Name, str);
						}

						if(WebChecker.IsNull(v) && param.ThrowNotFound) {
							WebTools.NotFound(
								String.Format(
									"EntityName:{0}\nParameterName:{1}\nKeyValue:{2}",
									t.Name,
									param.Title,
									val
								)
							);
						}
						val = v;
					}
					Reflector.SetValue(obj, param.PropertyInfo, val);
				}
			}
		}

		public static string ParameterToHiddenFields(
			IWeb page,
			List<RequestParameterAttribute> parameters,
			bool skipEmpty,
			params string[] skipParameters)
		{
			List<string> skips = new List<string>(skipParameters);
			HttpRequest Request = page.GetCurrentContext().Request;

			StringBuilder sb = new StringBuilder();
			foreach(var param in parameters) {
				if(param.IsBinary || skips.Contains(param.Title)) { continue; }
				string val;

				switch(param.ValueFrom) {
					case RequestType.Query:
						val = Request.QueryString[param.Title];
						break;
					case RequestType.Form:
						val = Request.Form[param.Title];
						break;
					case RequestType.Cookies:
						val = CookieManager.Get(param.Title);
						break;
					default:
						val = page.RequestValue(param.Title);
						break;
				}

				if(skipEmpty && String.IsNullOrEmpty(val)) { continue; }
				sb.AppendFormat(
					"<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>",
					param.Title,
					WebTools.HtmlEncode(val)
				);
			}
			return sb.ToString();
		}

		private static HttpPostedFile GetFile(HttpFileCollection files, string title)
		{
			HttpPostedFile file = files[title];
			if(null == file) {
				foreach(string key in files.AllKeys) {
					if(key.EndWith(WebConstants.Symbol.Dollar + title)) {
						file = files[key];
						break;
					}
				}
			}
			return file;
		}
		#endregion

		#region secure helper
		/// <summary>
		/// Check the if connection is need to replace the uri according to the SecureAttribute
		/// </summary>
		/// <param name="secure"></param>
		/// <param name="Request"></param>
		/// <param name="isReturnUriSecure"></param>
		/// <returns>null for no need to redirect</returns>
		public static UriBuilder CheckSecureConnect(
			SecureAttribute secure,
			HttpRequest Request)
		{
			if(WebChecker.IsNull(secure)) {
				if(Request.Url.Scheme == Uri.UriSchemeHttps) {
					return new UriBuilder(
						String.Format(
							"{0}://{1}:{2}{3}",
							Uri.UriSchemeHttp,
							Request.Url.Host,
							80,
							Request.RawUrl
						)
					);
				}
			} else {
				if(
					Request.Url.Scheme != Uri.UriSchemeHttps
					||
					(
						!String.IsNullOrEmpty(secure.Host)
						&&
						secure.Host != Request.Url.Host
					)) {
					return new UriBuilder(
						String.Format(
							"{0}://{1}:{2}{3}",
							Uri.UriSchemeHttps,
							String.IsNullOrEmpty(secure.Host)
								? Request.Url.Host
								: secure.Host,
							443,
							Request.RawUrl
						)
					);
				}
			}
			return null;
		}
		#endregion
	}
}
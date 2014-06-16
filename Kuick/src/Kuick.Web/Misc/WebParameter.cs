// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebParameter.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Linq;
using Kuick.Data;
using System.Reflection;

namespace Kuick.Web
{
	public class WebParameter : IParameter
	{
		#region constructor
		public WebParameter(HttpRequest request)
			: this(request, false)
		{
		}

		public WebParameter(HttpRequest request, bool queryStringOnly)
		{
			this.Request = request;
			NameValueCollection nvc = new NameValueCollection();
			if(!queryStringOnly) { nvc.Add(request.Form); }
			nvc.Add(request.QueryString);
			this.RequestNVc = nvc;
		}

		public WebParameter(NameValueCollection nvc)
		{
			this.RequestNVc = nvc;
		}
		#endregion

		#region property
		public NameValueCollection RequestNVc { get; private set; }
		public HttpRequest Request { get; set; }

		public HttpFileCollection Files
		{
			get
			{
				if(null == Request) { return null; }
				return this.Request.Files;
			}
		}
		#endregion


		#region RequestValue
		public DateTime RequestDateTime(string name)
		{
			return Formator.AirBagToDateTime(RequestValue(name));
		}
		public DateTime RequestDateTime(string name, DateTime airBag)
		{
			return Formator.AirBagToDateTime(RequestValue(name), airBag);
		}
		public bool RequestBoolean(string name)
		{
			return Formator.AirBagToBoolean(RequestValue(name));
		}
		public bool RequestBoolean(string name, bool airBag)
		{
			return Formator.AirBagToBoolean(RequestValue(name), airBag);
		}
		public int RequestInt(string name)
		{
			return Formator.AirBagToInt(RequestValue(name));
		}
		public int RequestInt(string name, int airBag)
		{
			return Formator.AirBagToInt(RequestValue(name), airBag);
		}
		public decimal RequestDecimal(string name)
		{
			return Formator.AirBagToDecimal(RequestValue(name));
		}
		public decimal RequestDecimal(string name, decimal airBag)
		{
			return Formator.AirBagToDecimal(RequestValue(name), airBag);
		}
		public string RequestValue(string name)
		{
			return RequestValue(name, string.Empty);
		}
		public string RequestValue(string name, string airBag)
		{
			try {
				if(WebChecker.IsNull(this.RequestNVc)) { return airBag; }

				string[] values = this.RequestNVc.GetValues(name);

				if(WebChecker.IsNull(values)) {
					foreach(string key in this.RequestNVc.AllKeys) {
						if(key.EndWith(WebConstants.Symbol.Dollar + name)) {
							values = this.RequestNVc.GetValues(key);
							break;
						}
					}
				}

				if(WebChecker.IsNull(values)) { return airBag; }

				if(values.Length > 1 && null != Request) {
					// Form
					NameValueCollection nvc = new NameValueCollection();
					if(null != Request.Form) { nvc.Add(Request.Form); }
					values = nvc.GetValues(name);
					if(null == values || values.Length == 0) {
						// QueryString
						nvc.Clear();
						if(null != Request.QueryString) { nvc.Add(Request.QueryString); }
						values = nvc.GetValues(name);
					}
				}

				if(null == values) { values = new string[0]; }

				string rtn = string.Empty;
				foreach(string value in values) {
					if(!string.IsNullOrEmpty(rtn)) {
						rtn += WebConstants.Symbol.Comma;
					}
					rtn += value.Trim().UrlDecode();
				}

				return rtn;
			} catch(Exception ex) {
				Logger.Error(
					"WebParameter.RequestValue",
					ex.Message,
					ex.ToAny(
						new Any("name", name),
						new Any("airBag", airBag)
					)
				);
				throw;
			}
		}
		public T RequestEnum<T>(string name)
		{
			string val = RequestValue(name);
			return Formator.AirBagToEnum<T>(val);
		}
		public T RequestEnum<T>(string name, T airBag)
		{
			string val = RequestValue(name);
			return Formator.AirBagToEnum<T>(val, airBag);
		}

		public void RemoveRequest(string name)
		{
			this.RequestNVc.Remove(name);
		}
		#endregion


		#region TryRequestValue
		public bool TryRequestDateTime(string name, out DateTime value)
		{
			return TryRequestDateTime(name, out value, WebConstants.Null.Date);
		}
		public bool TryRequestDateTime(string name, out DateTime value, DateTime airBag)
		{
			string str;
			if(TryRequestValue(name, out str)) {
				value = Formator.AirBagToDateTime(str, airBag);
				return true;
			} else {
				value = airBag;
				return false;
			}
		}
		public bool TryRequestBoolean(string name, out bool value)
		{
			return TryRequestBoolean(name, out value, true);
		}
		public bool TryRequestBoolean(string name, out bool value, bool airBag)
		{
			string str;
			if(TryRequestValue(name, out str)) {
				value = Formator.AirBagToBoolean(str, airBag);
				return true;
			} else {
				value = airBag;
				return false;
			}
		}
		public bool TryRequestInt(string name, out int value)
		{
			return TryRequestInt(name, out value);
		}
		public bool TryRequestInt(string name, out int value, int airBag)
		{
			string str;
			if(TryRequestValue(name, out str)) {
				value = Formator.AirBagToInt(str, airBag);
				return true;
			} else {
				value = airBag;
				return false;
			}
		}
		public bool TryRequestValue(string name, out string value)
		{
			return TryRequestValue(name, out value, string.Empty);
		}
		public bool TryRequestValue(string name, out string value, string airBag)
		{
			if(WebChecker.IsNull(this.RequestNVc)) {
				value = airBag;
				return false;
			}

			string[] values = this.RequestNVc.GetValues(name);
			if(WebChecker.IsNull(values)) {
				value = airBag;
				return false;
			}

			string rtn = string.Empty;
			foreach(string val in values) {
				if(!String.IsNullOrEmpty(rtn)) {
					rtn += WebConstants.Symbol.Semicolon;
				}
				rtn += val;
			}

			value = rtn;
			return true;
		}
		public bool TryRequestEnum<T>(string name, out T value)
		{
			string val = RequestValue(name);
			value = Formator.AirBagToEnum<T>(val);
			return true;
		}
		public bool TryRequestEnum<T>(string name, out T value, T airBag)
		{
			string val = RequestValue(name);
			value = Formator.AirBagToEnum<T>(val, airBag);
			return true;
		}
		#endregion

		#region operation
		public bool Exists(string name)
		{
			return this.RequestNVc.Exists(name);
		}
		public IParameter Add(string name, string value)
		{
			NameValueCollection nvc = this.RequestNVc;
			nvc.Add(name, value);
			this.RequestNVc = nvc;
			return this;
		}
		public IParameter Modify(string name, string value)
		{
			if(Exists(name)) { Remove(name); }
			Add(name, value);
			return this;
		}
		public IParameter Remove(string name)
		{
			NameValueCollection nvc = this.RequestNVc;
			nvc.Remove(name);
			this.RequestNVc = nvc;
			return this;
		}
		public IParameter InsertOrUpdate(string name, string value)
		{
			Remove(name); // whatever, remove first!
			Add(name, value);
			return this;
		}
		#endregion


		#region RequestEntity
		public IEntity RequestEntity(string assemblyName, string entityName)
		{
			IEntity schema = EntityCache.Get(entityName);
			IEntity instance = Reflector.CreateInstance(schema.GetType()) as IEntity;
			return RequestEntity(instance);
		}

		public T RequestEntity<T>() where T : class, IEntity, new()
		{
			T instance = new T();
			return RequestEntity<T>(instance);
		}

		public IEntity RequestEntity(IEntity instance)
		{
			if(null == instance) { return instance; }
			instance = Reflector.Bind(
				instance, instance.ColumnProperties, this.RequestNVc
			) as IEntity;

			if(null == instance.Columns) { return instance; }
			foreach(var column in instance.Columns) {
				if(column.IsBoolean) {                     // Boolean
					bool on = 
						Exists(column.AsName)
						&&
						RequestValue(column.AsName) == "on";
					instance.SetValue(column, on);
				} else if(column.IsDateTime) {             // DateTime
					if(Exists(column.AsName)) {
						if(RequestValue(column.AsName) == string.Empty) {
							instance.IsNull(column.Spec.ColumnName);
						}
					}
				}
			}

			return instance;
		}

		public T RequestEntity<T>(T instance) where T : class, IEntity, new()
		{
			return (T)Reflector.Bind(
				instance, instance.ColumnProperties, this.RequestNVc
			);
		}
		#endregion

		#region BuildQueryString
		public string BuildQueryString()
		{
			return BuildQueryString(Formator.NVc2NVs(this.RequestNVc));
		}

		public string BuildQueryString(params Any[] anys)
		{
			return BuildQueryString(true, anys);
		}

		public string BuildQueryString(bool keepOriginal, params Any[] anys)
		{
			return BuildQueryString(true, false, anys);
		}

		public string BuildQueryString(
			bool keepOriginal,
			bool clearWhenNullOrEmpty,
			params Any[] anys)
		{
			// XXX=1&YYY=2&ZZZ=3
			List<Any> list = new List<Any>();
			if(keepOriginal) {
				list.AddRange(Formator.NVc2NVs(HttpContext.Current.Request.QueryString));
			}

			if(!WebChecker.IsNull(anys)) {
				foreach(Any nv in anys) {
					list.RemoveAll(x => x.Name == nv.Name);
					if(WebChecker.IsNull(nv.Value)) {
						if(clearWhenNullOrEmpty) { continue; }
						list.Add(new Any(nv.Name, string.Empty));
					} else {
						list.Add(new Any(nv.Name, nv.Value));
					}
				}
			}
			return list.ToArray().ToQueryString();
		}

		public string NewQueryString(params Any[] anys)
		{
			return BuildQueryString(false, true, anys);
		}
		#endregion
	}
}

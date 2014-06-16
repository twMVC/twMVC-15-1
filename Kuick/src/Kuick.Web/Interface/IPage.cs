// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IPage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web.UI;
using Kuick.Data;

namespace Kuick.Web
{
	public interface IPage : IWeb
	{
		event EventHandler BeforeProcessCache;
		event EventHandler OnValidRequestBegin;
		event EventHandler OnValidRequestEnd;

		bool Logon(string userName, string password, string domain);
		void UpdateSchemaControls(Api api);
		void RegisterCSS(string href);
		void RegisterJavaScript(string src);
		void RegisterHeadString(string str);
		void RegisterHeadControl(Control control);
		void RunJs(string js);
		void RunJs(string js, params object[] args);
		string Alert(string msg);
		string Alert(string msg, string url);
		string Alert(string msg, string url, bool useReplace);
		void JsConstant(string key, bool value);
		void JsConstant(string key, params int[] values);
		void JsConstant(string key, int value);
		void JsConstant(string key, string value);
		void JsConstant(string key, params string[] values);
		void JsConstant(string key, string value, bool ValueSurroundWithSingleQuotationMarks);
		void JsReload();
		string Keywords { get; set; }
		string Description { get; set; }
		string CanonicalLink { get; set; }

		PermissionAttribute Permission { get; }
		bool Permit();
		bool RenderJQueryFile { get; }

		//string GetMultilingual(string key);
		//string GetMultilingual(string key, string defaultValue);
		DateTime LastModifiedTime { get; }
		string ParameterToHidden(params string[] skipParameters);
		string ParameterToHidden(bool skipEmpty, params string[] skipParameters);

		//
		void Redirect(string url);
	}
}

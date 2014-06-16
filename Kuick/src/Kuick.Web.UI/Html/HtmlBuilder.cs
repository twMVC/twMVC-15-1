// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HtmlBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-05-24 - Creation


using System;
using System.Text;

namespace Kuick.Web.UI
{
	public class HtmlBuilder
	{
		#region constructure
		public HtmlBuilder()
			: this(null)
		{
		}

		public HtmlBuilder(StringBuilder sb)
		{
			if(null == sb) {
				sb = new StringBuilder();
			}
			this.Builder = sb;
		}
		#endregion

		#region static
		public static HtmlBuilder New()
		{
			return new HtmlBuilder(null);
		}
		public static HtmlBuilder New(StringBuilder sb)
		{
			return new HtmlBuilder(sb);
		}
		#endregion

		#region property
		private StringBuilder Builder { get; set; }
		#endregion

		#region public function
		// Tag
		public HtmlBuilder Tag(HtmlTag tag)
		{
			return Tag(tag.ToString().ToLower());
		}
		public HtmlBuilder Tag(string tag)
		{
			Builder.Append("<");
			Builder.Append(tag.ToLower());
			Builder.Append(">");
			return this;
		}
		public HtmlBuilder TagWithClose(HtmlTag tag)
		{
			return TagWithClose(tag.ToString().ToLower());
		}
		public HtmlBuilder TagWithClose(string tag)
		{
			Builder.Append("<");
			Builder.Append(tag.ToLower());
			Builder.Append("/>");
			return this;
		}
		// BeginTag
		public HtmlBuilder BeginTag(HtmlTag tag)
		{
			Builder.Append("<");
			Builder.Append(tag.ToString().ToLower());
			return this;
		}
		public HtmlBuilder BeginTag(string tag)
		{
			Builder.Append("<");
			Builder.Append(tag.ToLower());
			return this;
		}
		// EndTag
		public HtmlBuilder EndTag()
		{
			Builder.Append(">");
			return this;
		}
		// CloseTag
		public HtmlBuilder CloseTag()
		{
			Builder.Append("/>");
			return this;
		}
		public HtmlBuilder CloseTag(HtmlTag tag)
		{
			Builder.Append("</");
			Builder.Append(tag.ToString().ToLower());
			Builder.Append(">");
			return this;
		}
		public HtmlBuilder CloseTag(string tag)
		{
			Builder.Append("</");
			Builder.Append(tag.ToLower());
			Builder.Append(">");
			return this;
		}
		// Br
		public HtmlBuilder Br()
		{
			Builder.Append("<br />");
			return this;
		}
		// Attr
		public HtmlBuilder Attr(HtmlAttr attr)
		{
			return Attr(false, attr);
		}
		public HtmlBuilder Attr(bool skip, HtmlAttr attr)
		{
			return Attr(skip, attr.ToString().ToLower());
		}
		public HtmlBuilder Attr(string attr)
		{
			return Attr(false, attr);
		}
		public HtmlBuilder Attr(bool skip, string attr)
		{
			if(skip) { return this; }
			Builder.Append(" ");
			Builder.Append(attr.ToLower());
			return this;
		}

		public HtmlBuilder Attr(HtmlAttr attr, params string[] values)
		{
			return Attr(false, attr, values);
		}
		public HtmlBuilder Attr(bool skip, HtmlAttr attr, params string[] values)
		{
			return Attr(skip, attr.ToString().ToLower(), values);
		}
		public HtmlBuilder Attr(string attr, params string[] values)
		{
			return Attr(false, attr, values);
		}
		public HtmlBuilder Attr(bool skip, string attr, params string[] values)
		{
			if(skip) { return this; }
			if(Checker.IsNull(values)) { return this; }

			Builder.Append(" ");
			Builder.Append(attr.ToLower());
			Builder.Append("=\"");
			bool has = false;
			foreach(string value in values) {
				if(string.IsNullOrEmpty(value)) { continue; }
				if(has) { Builder.Append(" "); }
				Builder.Append(value);
				has = true;
			}
			Builder.Append("\"");
			return this;
		}

		public HtmlBuilder Attr(HtmlAttr attr, int value)
		{
			return Attr(false, attr, value);
		}
		public HtmlBuilder Attr(bool skip, HtmlAttr attr, int value)
		{
			return Attr(skip, attr.ToString().ToLower(), value);
		}
		public HtmlBuilder Attr(string attr, int value)
		{
			return Attr(false, attr, value);
		}
		public HtmlBuilder Attr(bool skip, string attr, int value)
		{
			if(skip) { return this; }
			Builder.Append(" ");
			Builder.Append(attr.ToLower());
			Builder.Append("=\"");
			Builder.Append(value);
			Builder.Append("\"");
			return this;
		}

		public HtmlBuilder Attr<T>(HtmlAttr attr, T value)
		{
			return Attr<T>(false, attr, value);
		}
		public HtmlBuilder Attr<T>(bool skip, HtmlAttr attr, T value)
		{
			return Attr<T>(skip, attr.ToString().ToLower(), value);
		}
		public HtmlBuilder Attr<T>(string attr, T value)
		{
			return Attr<T>(false, attr, value);
		}
		public HtmlBuilder Attr<T>(bool skip, string attr, T value)
		{
			if(skip) { return this; }
			Builder.Append(" ");
			Builder.Append(attr.ToLower());
			Builder.Append("=\"");
			Builder.Append(value.ToString());
			Builder.Append("\"");
			return this;
		}

		// Append
		public HtmlBuilder Append(params string[] values)
		{
			if(Checker.IsNull(values)) { return this; }
			foreach(string value in values) {
				Builder.Append(value);
			}
			return this;
		}
		public HtmlBuilder AppendFormat(string value, params object[] objs)
		{
			Builder.AppendFormat(value, objs);
			return this;
		}
		public HtmlBuilder AppendLine(string value)
		{
			Builder.AppendLine(value);
			return this;
		}
		public HtmlBuilder AppendFormatLine(string value, params object[] objs)
		{
			Builder.AppendFormat(value, objs);
			Builder.AppendLine();
			return this;
		}
		#endregion

		#region override
		public override string ToString()
		{
			return Builder.ToString();
		}
		#endregion
	}
}

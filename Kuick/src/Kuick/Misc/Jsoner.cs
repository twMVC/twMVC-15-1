// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Jsoner.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Kuick
{
	public class Jsoner
	{
		private const string NULL_PAGE = "{ \"pageSize\": 0, \"pageIndex\": 0, \"count\": 0, \"pageRowCount\": 0, \"firstPageIndex\": 0, \"prePageIndex\": 0, \"nextPageIndex\": 0, \"lastPageIndex\": 0, \"rowFrom\": 0, \"rowTo\": 0, \"items\": " + Constants.Symbol.OpenBracket + Constants.Symbol.CloseBracket + "}";

		#region privaet Formatter

		private static readonly string NEWLINE = System.Environment.NewLine;
		private static readonly int NEWLINE_LENGTH = NEWLINE.Length;



		private static string Formatter(string json)
		{
			if(Current.Released) { return json; }
			return Formatter(new StringBuilder(json));
		}
		private static string Formatter(StringBuilder sb)
		{
			if(Current.Released) { return sb.ToString(); }

			var len = 0;
			var n = 0;
			var instr = false;
			var escaping = false;

			while(len < sb.Length) {
				string tmp = sb.ToString(len, 1);
				if(tmp == Constants.Symbol.Quotation && !escaping) { instr = !instr; }
				escaping = false;
				if(instr) {
					if(tmp == Constants.Symbol.BackwardSlash) { escaping = true; }
				} else {
					if(
						tmp == Constants.Symbol.OpenBrace
						||
						tmp == Constants.Symbol.OpenBracket) {
						++n;
						sb.Insert(len + 1, Constants.Symbol.Tab, n).Insert(len + 1, NEWLINE);
						len += n + NEWLINE_LENGTH;
					} else if(
						tmp == Constants.Symbol.CloseBrace
						||
						tmp == Constants.Symbol.CloseBracket) {
						--n;
						sb.Insert(len, Constants.Symbol.Tab, n).Insert(len, NEWLINE);
						len += n + NEWLINE_LENGTH;
					} else if(tmp == Constants.Symbol.Colon) {
						sb.Insert(len + 1, Constants.Symbol.Space);
						len++;
					} else if(tmp == Constants.Symbol.Comma) {
						sb.Insert(len + 1, Constants.Symbol.Tab, n).Insert(len + 1, NEWLINE);
						len += n + NEWLINE_LENGTH;
					} else if(
						tmp == Constants.Symbol.Space
						||
						tmp == Constants.Symbol.Tab
						||
						tmp == Constants.Symbol.NewLine
						||
						tmp == Constants.Symbol.CarriageReturn) {
						sb.Remove(len, 1);
						len--;
					}
				}
				len++;
			}
			return sb.ToString();
		}
		#endregion


		private static readonly string NV_FORMAT =
			Constants.Symbol.Quotation + "{0}" + Constants.Symbol.Quotation + ":{1}";

		public const string PAGE_SIZE = "pageSize";
		public const string PAGE_INDEX = "pageIndex";
		public const string COUNT = "count";
		public const string PREVIOUS = "previous";
		public const string NEXT = "next";
		public const string ITEMS = "items";

		public static string Serialize(Any[] nvs, params string[] propNames)
		{
			// {"Xxx" : 1, "Yyy" : "Qqq"}

			if(Checker.IsNull(nvs)) {
				return Constants.Json.Null.Object;
			}
			StringBuilder sb = new StringBuilder();

			bool isPartial = !Checker.IsNull(propNames);

			foreach(Any nv in nvs) {
				if(isPartial && !nv.Name.In(propNames)) { continue; }
				sb.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(nv));
			}

			return Formatter(sb.RemoveFirst()
				.Prepend(Constants.Symbol.OpenBrace)
				.Append(Constants.Symbol.CloseBrace)
			);
		}

		public static string Serialize(object obj, params string[] propNames)
		{
			// {"Xxx" : 1, "Yyy" : "Qqq"}
			if(Checker.IsNull(obj)) { return Constants.Json.Null.Object; }

			Type type = obj.GetType();
			switch(Reflector.GetDataType(type)) {
				case DataFormat.String:
				case DataFormat.Boolean:
				case DataFormat.DateTime:
				case DataFormat.Integer:
				case DataFormat.Decimal:
				case DataFormat.Byte:
				case DataFormat.Short:
				case DataFormat.Enum:
					return FormatValue(obj);
				default:
					IEnumerable enumList = obj as IEnumerable;
					if(!Checker.IsNull(enumList)) {
						List<object> list = new List<object>();
						foreach(var x in enumList) {
							list.Add(x);
						}
						return Serialize(list.ToArray(), propNames);
					} else {
						List<Any> anys = Reflector.ToAny(obj, propNames);
						return Serialize(anys.ToArray(), propNames);
					}
			}
		}

		public static string Serialize(object[] objs, params string[] propNames)
		{
			// [
			//	{"Xxx" : 1, "Yyy" : "Qqq"},
			//	{"Xxx" : 1, "Yyy" : "Qqq"},
			//	{"Xxx" : 1, "Yyy" : "Qqq"}
			// ]

			if(Checker.IsNull(objs)) { return Constants.Json.Null.Array; }

			StringBuilder sb = new StringBuilder();

			foreach(object x in objs) {
				sb.Append(Constants.Symbol.Comma);
				sb.Append(Serialize(x, propNames));
			}

			return Formatter(sb.RemoveFirst()
				.Prepend(Constants.Symbol.OpenBracket)
				.Append(Constants.Symbol.CloseBracket)
			);
		}

		public static string Serialize(
			Paginator paginator,
			object[] objs,
			params string[] propNames)
		{
			// {
			//    "pageSize": 10,
			//    "pageIndex": 2,
			//    "count": 16,
			//    "pageRowCount": 6,
			//    "firstPageIndex": 1,
			//    "prePageIndex": 1,
			//    "nextPageIndex": 2,
			//    "lastPageIndex": 2,
			//    "rowFrom": 11,
			//    "rowTo": 16,
			//    "items": [
			//        {"Id" : 3, "Name" : "Ammon Lin"},
			//        {"Id" : 4, "Name" : "Alex Chang"},
			//        {"Id" : 5, "Name" : "Jean Lin"}
			//    ]
			// }

			if(Checker.IsNull(objs)) { return NULL_PAGE; }

			return Formatter(new StringBuilder().Append(Constants.Symbol.OpenBrace)
				.Append(SerializeOne(new Any("pageSize", paginator.PageSize)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("pageIndex", paginator.PageIndex)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("count", paginator.Count)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("pageRowCount", paginator.PageRowCount)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("firstPageIndex", paginator.FirstPageIndex)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("prePageIndex", paginator.PrePageIndex)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("nextPageIndex", paginator.NextPageIndex)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("lastPageIndex", paginator.LastPageIndex)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("rowFrom", paginator.RowFrom)))
				.Append(Constants.Symbol.Comma)
				.Append(SerializeOne(new Any("rowTo", paginator.RowTo)))
				.Append(Constants.Symbol.Comma)
				.Append(String.Format(NV_FORMAT, "items", Serialize(objs, propNames)))
				.Append(Constants.Symbol.CloseBrace)
			);
		}

		public static string MakeSafety(string json)
		{
			if(String.IsNullOrEmpty(json)) { return "\"\""; }

			int i;
			int len = json.Length;
			StringBuilder sb = new StringBuilder(len + 4);
			string t;

			sb.Append('"');
			for(i = 0; i < len; i += 1) {
				char c = json[i];
				if((c == '\\') || (c == '"') || (c == '>')) {
					sb.Append('\\');
					sb.Append(c);
				} else if(c == '\b') {
					sb.Append("\\b");
				} else if(c == '\t') {
					sb.Append("\\t");
				} else if(c == '\n') {
					sb.Append("\\n");
				} else if(c == '\f') {
					sb.Append("\\f");
				} else if(c == '\r') {
					sb.Append("\\r");
				} else {
					if(c < ' ') {
						//t = "000" + Integer.toHexString(c);
						string tmp = new string(c, 1);
						t = "000" + int.Parse(tmp, NumberStyles.HexNumber);
						sb.Append("\\u" + t.Substring(t.Length - 4));
					} else {
						sb.Append(c);
					}
				}
			}
			sb.Append('"');
			return sb.ToString();
		}

		#region private
		private static string SerializeOne(Any nv)
		{
			return (null == nv)
				? String.Empty
				: String.Format(NV_FORMAT, nv.Name, FormatValue(nv.Value));
		}

		private static string FormatValue(object val)
		{
			if(val == null) { return "null"; }

			if(val is DateTime) {       // DateTime
				DateTime d = (DateTime)val;
				return String.Concat(
					Constants.Symbol.Quotation,
					Formator.ToString14(d),
					Constants.Symbol.Quotation
				);
			} else if(val is Boolean) { // Boolean
				bool b = (bool)val;
				return b
					? Constants.StringBoolean.True.ToLower()
					: Constants.StringBoolean.False.ToLower();
			} else if(val is int) {     // Integer
				return val.ToString();
			} else {                    // String
				return MakeSafety(val.ToString());
			}
		}
		#endregion
	}
}

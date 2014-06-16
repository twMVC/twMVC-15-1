// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Formator.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Drawing.Imaging;
//using Microsoft.Security.Application;

namespace Kuick
{
	public static class Formator
	{
		#region Stream, String, Bytes
		public static byte[] StringToBytes(string input)
		{
			if(string.IsNullOrEmpty(input)) { return new byte[0]; }
			return UnicodeEncoding.UTF8.GetBytes(input);
		}

		public static string BytesToString(byte[] input)
		{
			if(Checker.IsNull(input)) { return null; }
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < input.Length; i++) {
				sb.Append(input[i].ToString("x2"));
			}
			return sb.ToString();
		}

		public static Stream BytesToStream(byte[] input)
		{
			if(Checker.IsNull(input)) { return null; }
			return new MemoryStream(input);
		}

		public static Stream StringToStream(string input)
		{
			if(String.IsNullOrEmpty(input)) { return null; }
			return BytesToStream(StringToBytes(input));
		}

		public static string StreamToString(Stream input)
		{
			if(null == input) { return null; }
			byte[] bs = StreamToBtyes(input);
			string str = BytesToString(bs);
			return str;
		}

		public static byte[] StreamToBtyes(Stream input)
		{
			if(null == input) { return new byte[0]; }
			byte[] bytes = new byte[input.Length];
			input.Read(bytes, 0, bytes.Length);
			input.Seek(0, SeekOrigin.Begin);
			return bytes;
		}
		#endregion

		#region String
		public static string HtmlEncode(string value)
		{
			return HttpUtility.HtmlEncode(value ?? string.Empty);
		}

		public static string HtmlDecode(string value)
		{
			return HttpUtility.HtmlDecode(value ?? string.Empty);
		}

		public static string UrlEncode(string value)
		{
			return HttpUtility.UrlEncode(value ?? string.Empty);
		}

		public static string UrlDecode(string value)
		{
			return HttpUtility.UrlDecode(value ?? string.Empty);
		}

		public static bool ContainsAny(string value, params string[] values)
		{
			if(Checker.IsNull(value)) { return false; }

			// params string[], Or operation
			foreach(string x in values) {
				if(null == x) { continue; }
				if(value.Trim().Equals(x.Trim(), StringComparison.Ordinal)) {
					return true;
				}
			}
			return false;
		}

		public static bool ContainsAll(string value, params string[] values)
		{
			if(Checker.IsNull(value)) { return false; }

			// params string[], And operation
			foreach(string x in values) {
				if(!value.Trim().Equals(x.Trim(), StringComparison.Ordinal)) {
					return false;
				}
			}
			return true;
		}

		public static bool StartWith(string value, params string[] values)
		{
			if(Checker.IsNull(value)) { return false; }

			// params string[], Or operation
			foreach(string x in values) {
				if(value.StartsWith(x.Trim(), StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}
			return false;
		}

		public static bool EndWith(string value, params string[] values)
		{
			if(Checker.IsNull(value)) { return false; }

			// params string[], Or operation
			foreach(string x in values) {
				if(value.EndsWith(x.Trim())) { return true; }
			}
			return false;
		}

		public static bool LengthRange(string value, int minimumLength, int maximumLength)
		{
			return
				LengthBiggerThan(value, minimumLength - 1)
				&&
				LengthSmallerThan(value, maximumLength + 1);
		}

		public static bool LengthBiggerThan(string value, int length)
		{
			int len = Checker.IsNull(value) ? 0 : value.Length;
			return len.CompareTo(length) > 0;
		}

		public static bool LengthSmallerThan(string value, int length)
		{
			// int, int, Range check.
			int len = Checker.IsNull(value) ? 0 : value.Length;
			return len.CompareTo(length) < 0;
		}

		public static string ToTitleCase(string word)
		{
			if(Checker.IsNull(word)) { return string.Empty; }
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word);
		}

		public static Any[] ToAnys(string setting, char separator, char connector)
		{
			List<Any> list = new List<Any>();

			if(!Checker.IsNull(setting)) {
				string[] sections = setting.Split(separator);
				foreach(string x in sections) {
					string[] parts = x.Split(connector);
					if(parts.Length == 2) {
						string name = parts[0].Trim();
						string value = parts[1].Trim();
						list.Add(new Any(name, value));
					}
				}
			}

			return list.ToArray();
		}

		/// <summary>
		/// Mask critical words.<br/>
		/// if critical words' length more than or equal to 3, 
		/// return *{part of ogoginal words}*.<br/>
		/// if critical words' length less than 3, return the same length of * characters.
		/// </summary>
		/// <param name="criticalWords">critical words</param>
		/// <returns>Masked string</returns>
		public static string Mask(string criticalWords)
		{
			string masked = criticalWords.Length < 3
				? Formator.Repeater(Constants.Symbol.Asterisk, criticalWords.Length)
				: string.Concat(
					criticalWords.Left(1),
					Formator.Repeater(Constants.Symbol.Asterisk, criticalWords.Length - 2),
					criticalWords.Right(1)
				);
			return masked;
		}

		public static string ToUnicodeEntity(string source)
		{
			StringBuilder sb = new StringBuilder();
			foreach(char c in source) {
				byte[] bytes = Encoding.Unicode.GetBytes(c.ToString());
				sb.Append("&#x");
				for(int i = bytes.Length - 1; i >= 0; i--) {
					sb.Append(Convert.ToString(bytes[i], 16).PadLeft(2, '0'));
				}
				sb.Append(";");
			}
			return sb.ToString();
		}
		#endregion

		#region Base64
		public static string Base64Encode(string input)
		{
			if(String.IsNullOrEmpty(input)) { return null; }
			return Convert.ToBase64String(StringToBytes(input));
		}

		public static string Base64Decode(string input)
		{
			if(String.IsNullOrEmpty(input)) { return null; }
			return BytesToString(Convert.FromBase64String(input));
		}

		public static string Base64ToFileName(string input)
		{
			if(String.IsNullOrEmpty(input)) { return null; }
			return input.Replace("/", "_").Replace("=", string.Empty);
		}

		public static string ToBase64(System.Drawing.Image image)
		{
			MemoryStream ms = new MemoryStream();
			ImageFormat format = GetImageFormat(image);
			image.Save(ms, format);
			byte[] bytes = ms.ToArray();
			string base64 = Convert.ToBase64String(bytes);
			return base64;
		}

		public static ImageFormat GetImageFormat(System.Drawing.Image image)
		{
			if(image.RawFormat.Equals(ImageFormat.Bmp)) { return ImageFormat.Bmp; }
			if(image.RawFormat.Equals(ImageFormat.Emf)) { return ImageFormat.Emf; }
			if(image.RawFormat.Equals(ImageFormat.Exif)) { return ImageFormat.Exif; }
			if(image.RawFormat.Equals(ImageFormat.Gif)) { return ImageFormat.Gif; }
			if(image.RawFormat.Equals(ImageFormat.Icon)) { return ImageFormat.Icon; }
			if(image.RawFormat.Equals(ImageFormat.Jpeg)) { return ImageFormat.Jpeg; }
			if(image.RawFormat.Equals(ImageFormat.MemoryBmp)) { return ImageFormat.MemoryBmp; }
			if(image.RawFormat.Equals(ImageFormat.Png)) { return ImageFormat.Png; }
			if(image.RawFormat.Equals(ImageFormat.Tiff)) { return ImageFormat.Tiff; }
			return ImageFormat.Wmf;
		}
		#endregion

		#region DateTime
		public static DateTime FromDigit8(string digits)
		{
			if(Checker.IsNull(digits) || digits.Length != 8 || !Checker.IsNumeric(digits)) {
				return Constants.Null.Date;
			}

			try {
				DateTime dt = new DateTime(
					Convert.ToInt32(digits.Substring(0, 4)),
					Convert.ToInt32(digits.Substring(4, 2)),
					Convert.ToInt32(digits.Substring(6, 2))
				);
				return dt;
			} catch {
				return Constants.Null.Date;
			}
		}

		public static DateTime FromDigit14(string digits)
		{
			if(Checker.IsNull(digits) || digits.Length != 14 || !Checker.IsNumeric(digits)) {
				return Constants.Null.Date;
			}

			try {
				DateTime dt = new DateTime(
					Convert.ToInt32(digits.Substring(0, 4)),
					Convert.ToInt32(digits.Substring(4, 2)),
					Convert.ToInt32(digits.Substring(6, 2)),
					Convert.ToInt32(digits.Substring(8, 2)),
					Convert.ToInt32(digits.Substring(10, 2)),
					Convert.ToInt32(digits.Substring(12, 2))
				);
				return dt;
			} catch {
				return Constants.Null.Date;
			}
		}

		public static DateTime FromDigit17(string digits)
		{
			if(Checker.IsNull(digits) || digits.Length != 17 || !Checker.IsNumeric(digits)) {
				return Constants.Null.Date;
			}

			try {
				DateTime dt = new DateTime(
					Convert.ToInt32(digits.Substring(0, 4)),
					Convert.ToInt32(digits.Substring(4, 2)),
					Convert.ToInt32(digits.Substring(6, 2)),
					Convert.ToInt32(digits.Substring(8, 2)),
					Convert.ToInt32(digits.Substring(10, 2)),
					Convert.ToInt32(digits.Substring(12, 2)),
					Convert.ToInt32(digits.Substring(14, 3))
				);
				return dt;
			} catch {
				return Constants.Null.Date;
			}
		}

		public static DateTime ToDate8()
		{
			return ToDate8(DateTime.Now);
		}

		public static DateTime ToDate8(DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day);
		}

		public static DateTime ToDate14()
		{
			return ToDate14(DateTime.Now);
		}

		public static DateTime ToDate14(DateTime date)
		{
			return new DateTime(
				date.Year,
				date.Month,
				date.Day,
				date.Hour,
				date.Minute,
				date.Second
			);
		}

		public static DateTime ToDate17()
		{
			return ToDate17(DateTime.Now);
		}

		public static DateTime ToDate17(DateTime date)
		{
			return new DateTime(
				date.Year,
				date.Month,
				date.Day,
				date.Hour,
				date.Minute,
				date.Second,
				date.Millisecond
			);
		}

		public static string ToString4()
		{
			return ToString4(DateTime.Now);
		}

		public static string ToString4(DateTime date)
		{
			return date.ToString("yyyy");
		}

		public static string ToString8()
		{
			return ToString8(DateTime.Now);
		}

		public static string ToString8(DateTime date)
		{
			return date.ToString("yyyy-MM-dd");
		}

		public static string ToString8s()
		{
			return ToString8s(DateTime.Now);
		}

		public static string ToString8s(DateTime date)
		{
			return date.ToString("yyyyMMdd");
		}

		public static string ToString12()
		{
			return ToString12(DateTime.Now);
		}

		public static string ToString12(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm");
		}

		public static string ToString12s()
		{
			return ToString12s(DateTime.Now);
		}

		public static string ToString12s(DateTime date)
		{
			return date.ToString("yyyyMMddHHmm");
		}

		public static string ToString14()
		{
			return ToString14(DateTime.Now);
		}

		public static string ToString14(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static string ToString14s()
		{
			return ToString14s(DateTime.Now);
		}

		public static string ToString14s(DateTime date)
		{
			return date.ToString("yyyyMMddHHmmss");
		}

		public static string ToString17()
		{
			return ToString17(DateTime.Now);
		}

		public static string ToString17(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		public static string ToString17s()
		{
			return ToString17s(DateTime.Now);
		}

		public static string ToString17s(DateTime date)
		{
			return date.ToString("yyyyMMddHHmmssfff");
		}

		public static string ToString20()
		{
			return ToString20(DateTime.Now);
		}

		public static string ToString20(DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
		}

		public static string ToString20s()
		{
			return ToString20s(DateTime.Now);
		}

		public static string ToString20s(DateTime date)
		{
			return date.ToString("yyyyMMddHHmmssffffff");
		}

		/// <summary>
		/// Total seconds from 1970-01-01 to current.</ br>
		/// http://en.wikipedia.org/wiki/Unix_time
		/// </summary>
		/// <param name="value">the time for calculating</param>
		/// <returns>Unix format timestamp</returns>
		public static double ToUnixTimestamp(DateTime timestamp)
		{
			return Math.Floor((timestamp - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
		}

		/// <summary>
		/// Calculating the datetime by Unix format timestamp.</ br>
		/// http://en.wikipedia.org/wiki/Unix_time
		/// </summary>
		/// <param name="seconds">Unix format timestamp</param>
		/// <returns>DateTime format timestamp</returns>
		public static DateTime FromUnixTimestamp(double unixTimestamp)
		{
			return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixTimestamp);
		}

		#endregion

		#region ListAnys & ListAnysAsHtml
		public static string ListAnys(Any[] anys)
		{
			if(Checker.IsNull(anys)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			int index = 0;
			int length = (int)Math.Ceiling(Math.Log10(anys.Length + 1));
			int maxLen = 0;
			foreach(Any any in anys) {
				if(null == any.Name) { continue; }
				maxLen = Math.Max(maxLen, any.Name.Length);
			}
			foreach(Any any in anys) {
				if(index > 0) { sb.AppendLine(""); }

				string name = Formator.AirBag(any.Name);

				if(!Checker.IsNull(any.Value)) {
					Type type = any.Value.GetType();

					// Array
					if(type.IsArray) {
						Array array = any.Value as Array;
						if(!Checker.IsNull(array)) {
							++index;
							int idx = 0;
							int len = (int)Math.Ceiling(Math.Log10(array.Length));
							foreach(var x in array) {
								if(idx > 0) { sb.AppendLine(""); }
								string val = x.ToString();
								if(val.Contains(Environment.NewLine)) {
									val = val.Replace(
										Environment.NewLine,
										String.Concat(
											Environment.NewLine,
											Formator.Repeater(" ", len + maxLen + 5)
										)
									);
								}

								sb.AppendFormat(
									"{0} {1}{2} = {3} {4}",
									Formator.OrderList(index, length),
									name,
									Formator.Repeater(" ", (maxLen - name.Length)),
									Formator.OrderList(++idx, len),
									val
								);
							}
							continue;
						}
					}
				}

				string value = any.ToString();
				if(Checker.IsNull(value)) {
					if(!Checker.IsNull(any.Values)) {
						foreach(object x in any.Values) {
							value += (value.Length == 0)
								? string.Empty
								: Constants.Symbol.Comma;
							value += x.ToString();
						}
					}
				}

				if(value.Contains(Environment.NewLine)) {
					value = value.Replace(
						Environment.NewLine,
						String.Concat(
							Environment.NewLine,
							Formator.Repeater(" ", length + maxLen + 5)
						)
					);
				}

				sb.AppendFormat(
					"{0} {1}{2} = {3}",
					Formator.OrderList(++index, length),
					name,
					Formator.Repeater(" ", (maxLen - name.Length)),
					value
				);
			}
			return sb.ToString();
		}

		public static string ListAnysAsHtml(Any[] anys)
		{
			if(Checker.IsNull(anys)) { return string.Empty; }

			int maxLen = 0;
			foreach(Any any in anys) {
				if(null == any.Name) { continue; }
				maxLen = Math.Max(maxLen, any.Name.Length);
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<ol>");
			foreach(Any any in anys) {
				string name = Formator.AirBag(any.Name);
				if(!Checker.IsNull(any.Value)) {
					Type type = any.Value.GetType();

					// Array
					if(type.IsArray) {
						Array array = any.Value as Array;
						if(!Checker.IsNull(array)) {
							int idx = 0;
							int len = (int)Math.Ceiling(Math.Log10(array.Length));
							foreach(var x in array) {
								if(idx > 0) { sb.AppendLine("<br>"); }
								string val = x.ToString();
								if(val.Contains(Environment.NewLine)) {
									val = val.Replace(
										Environment.NewLine,
										String.Concat(
											Environment.NewLine,
											Formator.Repeater("&nbsp;", len + maxLen + 5)
										)
									);
								}

								sb.AppendFormat(
									"<li>{0}{1} = {2} {3}</li>",
									name,
									Formator.Repeater("&nbsp;", (maxLen - name.Length)),
									Formator.OrderList(++idx, len),
									val
								);
							}
							continue;
						}
					}
				}

				string value = any.ToString();
				if(Checker.IsNull(value)) {
					if(!Checker.IsNull(any.Values)) {
						foreach(object x in any.Values) {
							value += (value.Length == 0)
								? string.Empty
								: Constants.Symbol.Comma;
							value += x.ToString();
						}
					}
				}

				if(value.Contains(Environment.NewLine)) {
					value = value.Replace(
						Environment.NewLine,
						String.Concat(
							Environment.NewLine,
							Formator.Repeater("&nbsp;", maxLen + 5)
						)
					);
				}

				sb.AppendFormat(
					"<li> {0}{1} = {2}</li>",
					name,
					Formator.Repeater("&nbsp;", (maxLen - name.Length)),
					value
				);
			}
			sb.AppendLine("</ol>");
			return sb.ToString();
		}
		#endregion

		#region OrderList
		public static string OrderList(int no, int maxLength)
		{
			return OrderList(no, maxLength, true);
		}

		public static string OrderList(int no, int maxLength, bool withSuffix)
		{
			string s = no.ToString();
			int len = s.Length;
			string pattern = withSuffix ? "{0}{1}." : "{0}{1}";
			return String.Format(
				pattern,
				len >= maxLength
					? ""
					: Repeater("0", maxLength - len),
				s
			);
		}
		#endregion

		#region Repeater
		public static string Repeater(string seed, int times)
		{
			// have better way?
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < times; i++) {
				sb.Append(seed);
			}
			return sb.ToString();
		}
		#endregion

		#region List, ArrayList, Collection, Array, Hashtable
		public static V[] ColsFromDic<V>(Dictionary<string, V> dic)
		{
			List<V> list = new List<V>();
			foreach(KeyValuePair<string, V> kvp in dic) {
				list.Add((V)kvp.Value);
			}
			return list.ToArray();
		}

		public static V DicItem<V>(Dictionary<string, V> dic, string key)
		{
			List<V> list = new List<V>();
			if(dic.ContainsKey(key)) { list.Add((V)dic[key]); }
			V[] vs = list.ToArray();

			return Checker.IsNull(vs) ? default(V) : vs[0];
		}

		public static ArrayList ColsToArrayList<T>(T[] cols)
		{
			ArrayList al = new ArrayList();
			if(null != cols && cols.Length > 0) {
				for(int i = 0; i < cols.Length; i++) {
					al.Add((T)cols[i]);
				}
			}
			return al;
		}

		public static ArrayList ColsToArrayList(object[] cols)
		{
			ArrayList al = new ArrayList();
			if(!Checker.IsNull(cols)) {
				for(int i = 0; i < cols.Length; i++) {
					al.Add(cols[i]);
				}
			}
			return al;
		}

		public static T[] ListToCols<T>(List<T> list)
		{
			Array array = ListToArray<T>(list);
			if(null != list) {
				if(null == array) {
					return new T[0];
				}
			}
			return (T[])array;
		}

		public static object[] ArrayToCols(Array array)
		{
			return (null == array) ? new object[0] : (object[])array;
		}

		public static T[] ArrayToCols<T>(Array array)
		{
			return (null == array) ? new T[0] : (T[])array;
		}

		public static T[] ArrayListToCols<T>(ArrayList al)
		{
			return Checker.IsNull(al) ? new T[0] : al.ToArray(typeof(T)) as T[];
		}

		public static Array ListToArray<T>(List<T> list)
		{
			if(null == list || list.Count == 0) { return null; }
			Array array = Array.CreateInstance(typeof(T), list.Count);
			int i = 0;
			foreach(Object x in list) {
				array.SetValue(x, i++);
			}
			return array;
		}

		public static Array ArrayListToArray<T>(ArrayList arrayList)
		{
			if(null == arrayList || arrayList.Count == 0) { return null; }
			Array array = Array.CreateInstance(typeof(T), arrayList.Count);
			int i = 0;
			foreach(Object x in arrayList) {
				array.SetValue(x, i++);
			}
			return array;
		}

		public static Array ColsToArray<T>(T[] cols)
		{
			if(Checker.IsNull<T>(cols)) { return null; }
			Array array = Array.CreateInstance(typeof(T), cols.Length);
			int i = 0;
			foreach(object x in cols) {
				array.SetValue(x, i++);
			}
			return array;
		}

		public static List<T> ColsToList<T>(T[] cols)
		{
			List<T> list = new List<T>();
			if(null != cols) {
				for(int i = 0; i < cols.Length; i++) {
					list.Add((T)cols[i]);
				}
			}
			return list;
		}

		public static List<T> ArrayToList<T>(params T[] cols)
		{
			List<T> list = new List<T>();
			if(null != cols) {
				for(int i = 0; i < cols.Length; i++) {
					list.Add((T)cols[i]);
				}
			}
			return list;
		}

		public static ListItem[] ListItemsFromAnys(Any[] anys)
		{
			if(null == anys || anys.Length == 0) { return null; }
			List<ListItem> list = new List<ListItem>();
			foreach(Any any in anys) {
				string str = string.Empty;
				if(null != any.Value) { str = any.Value.ToString(); }
				list.Add(new ListItem(str, any.Name));
			}
			return list.ToArray();
		}

		public static ListItem[] ListItemCollectionToArray(
			ListItemCollection listItemCollection
		)
		{
			List<ListItem> list = new List<ListItem>();
			IEnumerator enums = listItemCollection.GetEnumerator();
			while(enums.MoveNext()) {
				list.Add((ListItem)enums.Current);
			}
			return list.ToArray();
		}

		public static ListItemCollection ListItemCollectionFromArray(ListItem[] listItems)
		{
			if(null == listItems || listItems.Length == 0) { return null; }
			ListItemCollection lic = new ListItemCollection();
			foreach(ListItem x in listItems) {
				lic.Add(x);
			}
			return lic;
		}

		public static ListItemCollection ListItemCollectionFromAnys(Any[] anys)
		{
			if(null == anys || anys.Length == 0) { return null; }
			ListItemCollection lic = new ListItemCollection();
			foreach(Any any in anys) {
				lic.Add(new ListItem(any.Name, any.Value.ToString()));
			}
			return lic;
		}
		#endregion

		#region Encodec
		public const string MD5_ENCODE = "MD5";
		public const string SHA512_ENCODE = "SHA512";
		public static string EncodeMD5(string str)
		{
			return Encode(MD5_ENCODE, str);
		}
		public static string EncodeSHA(string str)
		{
			return Encode(SHA512_ENCODE, str);
		}
		private static string Encode(string encodeBy, string str)
		{
			if(String.IsNullOrEmpty(str)) { return null; }
			string rtn = "";
			byte[] buffer = Encoding.Unicode.GetBytes(str);
			byte[] hash = HashAlgorithm.Create(encodeBy).ComputeHash(buffer);
			rtn = Convert.ToBase64String(hash);
			return rtn;
		}
		#endregion

		#region Switch
		public static void Switch(ref DateTime value0, ref DateTime value9)
		{
			if(value0 > value9) {
				DateTime tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref int value0, ref int value9)
		{
			if(value0 > value9) {
				int tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref decimal value0, ref decimal value9)
		{
			if(value0 > value9) {
				decimal tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref long value0, ref long value9)
		{
			if(value0 > value9) {
				long tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref float value0, ref float value9)
		{
			if(value0 > value9) {
				float tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref byte value0, ref byte value9)
		{
			if(value0 > value9) {
				byte tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}
		#endregion

		#region Filter
		public static string FilterSql(string sql)
		{
			return CPlum(sql);
		}

		public static T FilterMainFirst<T>(T[] cols, params Any[] anys)
		{
			T[] all = FilterMain<T>(true, cols, anys);
			if(Checker.IsNull(all)) { return default(T); }
			return all[0];
		}

		public static T[] FilterMain<T>(T[] cols, params Any[] anys)
		{
			return FilterMain<T>(false, cols, anys);
		}

		private static T[] FilterMain<T>(bool onlyFirst, T[] cols, params Any[] anys)
		{
			if(Checker.IsNull<T>(cols)) { return null; }
			if(Checker.IsNull(anys)) { return cols; }

			Type type = typeof(T);
			List<T> list = new List<T>();
			foreach(T x in cols) {
				try {
					bool pick = true;
					foreach(Any any in anys) {
						PropertyInfo info = Reflector.GetProperty(type, any.Name);
						if(null == info) {
							pick = false;
							break;
						}
						object anyValue = Formator.AirBag(any.Value) as object;
						object val = info.GetValue(x, null);
						if(null == val) {
							if(null == anyValue) { continue; }
							pick = false;
							break;
						} else {
							if(val.Equals(anyValue)) { continue; }
							pick = false;
							break;
						}
					}

					if(pick) {
						list.Add(x);
						if(onlyFirst) { return list.ToArray(); }
					}
				} catch(Exception ex) {
					Logger.Error(
						ex,
						"Kuick.Formator.FilterMain",
						ex.ToAny()
					);
				}
			}
			if(null == list || list.Count == 0) { return new T[0]; }

			return ListToCols<T>(list);
		}
		#endregion

		#region RemoveDuplicate
		public static string[] RemoveDuplicate(string[] cols)
		{
			var linq = cols.Distinct();
			return linq.ToArray() as string[];
		}
		#endregion

		#region Exclude
		public static string[] Exclude(string[] cols, params string[] excludeItems)
		{
			List<string> list = new List<string>();
			foreach(string x in cols) {
				if(!x.In(excludeItems)) { list.Add(x); }
			}
			return list.ToArray();
		}

		public static T[] Exclude<T>(T[] originals, T[] froms) where T : IEquatable<T>
		{
			List<T> list = new List<T>();
			foreach(T x in originals) {
				bool need = true;
				foreach(T to in froms) {
					if(x.Equals(to)) {
						need = false;
						break;
					}
				}
				if(need) { list.Add(x); }
			}
			return list.ToArray();
		}
		#endregion

		#region Combine
		public static T[] Combine<T>(T cols1, T[] cols2)
		{
			return Combine(new T[] { cols1 }, cols2);
		}

		public static T[] Combine<T>(T[] cols1, T[] cols2)
		{
			List<T> list = new List<T>();
			if(!Checker.IsNull(cols1)) {
				foreach(T obj1 in cols1) {
					if(!list.Contains(obj1)) { list.Add(obj1); }
				}
			}
			if(!Checker.IsNull(cols2)) {
				foreach(T obj2 in cols2) {
					if(!list.Contains(obj2)) { list.Add(obj2); }
				}
			}
			return list.ToArray();
		}

		public static T[] Combine<T>(T[] cols1, T[] cols2, T[] cols3)
		{
			return Combine<T>(Combine<T>(cols1, cols2), cols3);
		}

		public static T[] Combine<T>(T[] cols1, T[] cols2, T[] cols3, T[] cols4)
		{
			return Combine<T>(Combine<T>(cols1, cols2), Combine<T>(cols3, cols4));
		}
		#endregion

		#region Eliminate
		public static T[] Eliminate<T>(T[] sources, T[] targets)
		{
			List<T> list = new List<T>();
			if(sources != null && sources.Length > 0) {
				foreach(T source in sources) {
					if(!list.Contains(source)) { list.Add(source); }
				}
			}
			if(targets != null && targets.Length > 0) {
				foreach(T target in targets) {
					if(!list.Contains(target)) { list.Remove(target); }
				}
			}
			return list.ToArray();
		}
		#endregion

		#region Hashtable2String
		public static string Hashtable2String(Hashtable ht)
		{
			return Hashtable2String(ht, "&", "{0}={1}");
		}

		public static string Hashtable2String(Hashtable ht, string nvPattern, string seperator)
		{
			if(null == ht) { return string.Empty; }
			StringBuilder sb = new StringBuilder();
			foreach(DictionaryEntry x in ht) {
				if(sb.Length > 0) { sb.Append(seperator); }
				sb.AppendFormat(nvPattern, x.Key.ToString(), x.Value.ToString());
			}
			return sb.ToString();
		}
		#endregion

		#region Hashtable2Cols
		public static T[] Hashtable2Cols<T>(Hashtable ht)
		{
			if(null == ht) { return new T[0]; }
			List<T> al = new List<T>();
			foreach(DictionaryEntry x in ht) {
				T one = (T)x.Value;
				if(!al.Contains(one)) { al.Add(one); }
			}
			return al.ToArray();
		}
		#endregion

		#region ArrayList Add/Remove one
		public static bool ArrayListAdd(ArrayList al, object one)
		{
			if(ArrayListExists(al, one)) {
				return false;
			} else {
				al.Add(one);
				return true;
			}
		}

		public static bool ArrayListRemove(ArrayList al, object one)
		{
			if(null == al || al.Count == 0) { return false; }
			for(int i = 0; i < al.Count; i++) {
				if(al[i].Equals(one)) {
					al.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public static bool ArrayListExists(ArrayList al, object one)
		{
			if(null == al || al.Count == 0) { return false; }
			if(null == one) { return false; }
			foreach(object x in al) {
				if(x.Equals(one)) { return true; }
			}
			return false;
		}
		#endregion

		#region Paging
		/// <returns></returns>
		public static T[] Paging<T>(T[] instances, int pageSize, int pageIndex)
		{
			try {
				bool withPaging = (pageSize > 0) && (pageIndex >= 0) ? true : false;
				if(!withPaging) { return instances; }

				int rowFrom = 0;
				int rowTo = 0;
				if(withPaging) {
					pageIndex = (pageIndex == 0) ? 1 : pageIndex;
					rowFrom = pageSize * (pageIndex - 1) + 1;
					rowTo = pageSize * pageIndex;
				}

				List<T> list = new List<T>();
				int index = 0;
				foreach(T x in instances) {
					index++;
					if(withPaging) {
						if((index < rowFrom) && (index > rowTo)) { continue; }
					}
					list.Add(x);
				}
				return list.ToArray();
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Formator.FilterMain",
					ex.ToAny()
				);
				return new T[0];
			}
		}
		#endregion

		#region Encrypt and Decrypt
		public static string Encrypt(string password, string data)
		{
			try {
				if(null == data || data.Length == 0) { return string.Empty; }

				MemoryStream memStream = new MemoryStream();
				BinaryWriter binWriter = new BinaryWriter(memStream);
				binWriter.Write(data);
				binWriter.Flush();
				binWriter.Close();
				byte[] output = memStream.ToArray();

				RandomNumberGenerator randNumGen = RandomNumberGenerator.Create();
				byte[] salt = new byte[16];
				randNumGen.GetBytes(salt);

				PasswordDeriveBytes passBytes = new PasswordDeriveBytes(password, salt);
				byte[] key = passBytes.GetBytes(16);

				Rijndael cryptoAlg = Rijndael.Create();
				cryptoAlg.Padding = PaddingMode.ISO10126;
				cryptoAlg.Key = key;

				memStream = new MemoryStream();
				memStream.Write(salt, 0, salt.Length);
				memStream.Write(cryptoAlg.IV, 0, cryptoAlg.IV.Length);

				CryptoStream cryptoStream = new CryptoStream(
					memStream,
					cryptoAlg.CreateEncryptor(),
					CryptoStreamMode.Write
				);

				cryptoStream.Write(output, 0, output.Length);
				cryptoStream.FlushFinalBlock();
				cryptoStream.Close();

				byte[] stringBytes = memStream.ToArray();
				string encryptedString = ToHex(stringBytes);
				return encryptedString;
			} catch(Exception ex) {
				Logger.Error("Formator.Encrypt", ex.ToAny());
				throw;
			}
		}

		/// <returns></returns>
		public static string Decrypt(string password, string data)
		{
			try {
				if(Checker.IsEncrypted(data)) {
					data = data.Substring(Constants.Prefix.Encrypted.Length);
				}
				if(null == data || data.Length == 0) { return string.Empty; }

				byte[] b = null;
				try {
					b = FromHex(data);
				} catch {
					return data;
				}

				Rijndael cryptoAlg = Rijndael.Create();
				cryptoAlg.Padding = PaddingMode.ISO10126;
				MemoryStream memStream = new MemoryStream(b);
				byte[] salt = new byte[16];
				byte[] IV = new byte[cryptoAlg.IV.Length];

				memStream.Read(salt, 0, salt.Length);
				memStream.Read(IV, 0, IV.Length);

				PasswordDeriveBytes passBytes = new PasswordDeriveBytes(password, salt);
				byte[] key = passBytes.GetBytes(16);
				cryptoAlg.Key = key;
				cryptoAlg.IV = IV;

				MemoryStream outputStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(
					memStream,
					cryptoAlg.CreateDecryptor(),
					CryptoStreamMode.Read
				);

				int bytesRead = 0;
				byte[] buffer = new byte[256];
				do {
					bytesRead = cryptoStream.Read(buffer, 0, 256);
					outputStream.Write(buffer, 0, bytesRead);
				}
				while(bytesRead > 0);

				cryptoStream.Close();
				outputStream.Close();

				byte[] stringBytes = outputStream.ToArray();
				MemoryStream stringStream = new MemoryStream(stringBytes);
				BinaryReader binRead = new BinaryReader(stringStream);
				string outputString = binRead.ReadString();
				binRead.Close();

				return outputString;
			} catch {
				//Logger.Error("Formator.Decrypt", ex.ToAny());
				//throw;
				return data;
			}
		}
		#endregion

		#region HEX
		/// <returns></returns>
		public static byte[] FromHex(string hex)
		{
			if(null == hex) {
				return null;
			} else if((hex.Length % 2) != 0) {
				throw new Exception("Hex String length must be a multiple of 2.");
			}

			int length = hex.Length / 2;
			byte[] result = new Byte[length];
			string h = hex.ToUpper();

			for(int i = 0; i < length; i++) {
				char c = h[2 * i];
				int index = Constants.HexCode.IndexOf(c);
				if(index == -1) {
					throw new Exception("Hex String can't contain '" + c + "'");
				}

				int j = 16 * index;
				c = h[(2 * i) + 1];
				index = Constants.HexCode.IndexOf(c);
				if(index == -1) {
					throw new Exception("Hex String can't contain '" + c + "'");
				}
				j += index;
				result[i] = (byte)(j & 0xFF);
			}

			return result;
		}

		public static string ToHex(byte[] b)
		{
			StringBuilder sb = new StringBuilder();

			for(int i = 0; i < b.Length; i++) {
				int j = ((int)b[i]) & 0xFF;

				char first = Constants.HexCode[j / 16];
				char second = Constants.HexCode[j % 16];

				sb.Append(first);
				sb.Append(second);
			}

			return sb.ToString();
		}
		#endregion

		#region AirBag
		public static Hashtable AirBag(Hashtable ht)
		{
			return Checker.IsNull(ht) ? new Hashtable() : ht;
		}

		public static DateTime AirBag(DateTime dateTime)
		{
			return Checker.IsNull(dateTime) ? Constants.Date.Min : dateTime;
		}

		public static DateTime AirBag(DateTime dateTime, DateTime airBag)
		{
			return Checker.IsNull(dateTime) ? airBag : dateTime;
		}

		public static string AirBag(string value, params string[] airBags)
		{
			if(!Checker.IsNull(value)) { return value; }
			if(Checker.IsNull(airBags)) { return string.Empty; }
			foreach(string x in airBags) {
				if(!Checker.IsNull(x)) { return x; }
			}
			return string.Empty;
		}

		public static int AirBag(int value)
		{
			return (value == -1) ? 0 : value;
		}

		public static int AirBag(int value, int airBag)
		{
			return (value == -1) ? airBag : value;
		}

		public static long AirBag(long value)
		{
			return (value == -1) ? 0 : value;
		}

		public static long AirBag(long value, long airBag)
		{
			return (value == -1) ? airBag : value;
		}

		public static float AirBag(float value)
		{
			return (value == -1) ? 0 : value;
		}

		public static float AirBag(float value, float airBag)
		{
			return (value == -1) ? airBag : value;
		}

		public static object AirBag(object value)
		{
			return value ?? string.Empty;
		}

		public static T AirBag<T>(T value, params T[] airBags)
		{
			if(!Checker.IsNull<T>(value)) { return value; }
			if(null == airBags || airBags.Length == 0) { return default(T); }
			foreach(T x in airBags) {
				if(!Checker.IsNull<T>(x)) { return x; }
			}
			return default(T);
		}

		public static string AirBagToString(string value, params string[] airBags)
		{
			return AirBag(value, airBags);
		}

		public static int AirBagToInt(string value)
		{
			return AirBagToInt(value, default(int));
		}

		public static int AirBagToInt(string value, int airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsInt(value) ? Convert.ToInt32(value, provider) : airBag;
		}

		public static short AirBagToShort(string value)
		{
			return AirBagToShort(value, default(short));
		}

		public static short AirBagToShort(string value, short airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsInt(value) ? Convert.ToInt16(value, provider) : airBag;
		}

		public static Single AirBagToSingle(string value)
		{
			return AirBagToSingle(value, default(Single));
		}

		public static Single AirBagToSingle(string value, Single airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsInt(value) ? Convert.ToSingle(value, provider) : airBag;
		}

		public static decimal AirBagToDecimal(string value)
		{
			return AirBagToDecimal(value, default(decimal));
		}

		public static decimal AirBagToDecimal(string value, decimal airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsDecimal(value) ? decimal.Parse(value, provider) : airBag;
		}

		public static long AirBagToLong(string value)
		{
			return AirBagToLong(value, default(long));
		}

		public static long AirBagToLong(string value, long airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsLong(value) ? long.Parse(value, provider) : airBag;
		}

		public static float AirBagToFloat(string value)
		{
			return AirBagToFloat(value, default(float));
		}

		public static float AirBagToFloat(string value, float airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsFloat(value) ? float.Parse(value, provider) : airBag;
		}

		public static double AirBagToDouble(string value)
		{
			return AirBagToDouble(value, default(double));
		}

		public static double AirBagToDouble(string value, double airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsDouble(value) ? double.Parse(value, provider) : airBag;
		}

		public static bool AirBagToBoolean(string value)
		{
			return AirBagToBoolean(value, false);
		}

		public static bool AirBagToBoolean(string value, bool airBag)
		{
			return string.IsNullOrEmpty(value)
				? airBag
				: value.ToLower().In("1", "t", "y", "true", "yes", "on");
		}

		public static DateTime AirBagToDateTime(string value)
		{
			return AirBagToDateTime(value, Constants.Null.Date);
		}

		public static DateTime AirBagToDateTime(string value, DateTime airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			DateTimeFormatInfo provider = info.DateTimeFormat;

			try {
				return string.IsNullOrEmpty(value)
					? airBag
					: Convert.ToDateTime(value, provider);
			} catch {
				return airBag;
			}
		}

		public static byte AirBagToByte(string value)
		{
			return AirBagToByte(value, default(byte));
		}

		public static byte AirBagToByte(string value, byte airBag)
		{
			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsByte(value) ? byte.Parse(value) : airBag;
		}

		public static char AirBagToChar(string value)
		{
			return AirBagToChar(value, default(char));
		}

		public static char AirBagToChar(string value, char airBag)
		{
			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsChar(value) ? char.Parse(value) : airBag;
		}

		public static Guid AirBagToGuid(string value)
		{
			return AirBagToGuid(value, Constants.Default.Guid);
		}

		public static Guid AirBagToGuid(string value, Guid airBag)
		{
			return string.IsNullOrEmpty(value)
				? airBag
				: Checker.IsGuid(value) ? new Guid(value) : airBag;
		}

		public static Color AirBagToColor(string value, Color airBag)
		{
			int i;
			if(value.StartsWith("#")) {
				if(!Int32.TryParse(
					value.TrimStart('#'),
					NumberStyles.HexNumber,
					CultureInfo.InvariantCulture,
					out i)) {
					return airBag;
				}
			} else {
				if(!Int32.TryParse(value, out i)) {
					return airBag;
				}
			}
			return Color.FromArgb(i);
		}

		public static string AirBagXmlAttr(XmlNode node, string name)
		{
			return AirBagXmlAttr(node, name, string.Empty);
		}

		public static string AirBagXmlAttr(XmlNode node, string name, string airBag)
		{
			if(null == node || Checker.IsNull(node.Attributes)) { return airBag; }
			IEnumerator attrs = node.Attributes.GetEnumerator();
			while(attrs.MoveNext()) {
				XmlAttribute attr = (XmlAttribute)attrs.Current;
				if(attr.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) {
					return attr.Value.IsNullOrEmpty() 
						? string.Empty 
						: attr.Value.Trim();
				}
			}
			return airBag;
		}

		public static string ChildInnerText(XmlNode node, string tag)
		{
			return ChildInnerText(node, tag, string.Empty);
		}

		public static string ChildInnerText(XmlNode node, string tag, string airBag)
		{
			if(null == node) { return airBag; }
			XmlNode subNode = node.SelectSingleNode(tag);
			if(null == subNode) { return airBag; }
			string txt = subNode.NodeType.EnumIn(XmlNodeType.Text, XmlNodeType.CDATA)
				? node.Value
				: subNode.InnerText;
			return txt.IsNullOrEmpty() ? string.Empty : txt.Trim();
		}

		public static T AirBagToEnum<T>(string val)
		{
			return AirBagToEnum<T>(val, default(T));
		}

		public static T AirBagToEnum<T>(string val, T airBag)
		{
			try {
				return (T)Enum.Parse(typeof(T), val, true);
			} catch {
				if(typeof(T).IsEnum) {
					// by description
					EnumReference<T> ef = EnumCache.Get<T>();
					foreach(var item in ef.Items) {
						if(item.Title == val) { return item.Value; }
					}
				}
				return airBag;
			}
		}

		public static string AirBagConfig(string value)
		{
			return Checker.IsNull(value) ? string.Empty : value;
		}
		#endregion

		#region Distinct
		public static T[] Distinct<T>(T[] cols, string propName)
		{
			Hashtable ht = new Hashtable();
			foreach(T x in cols) {
				string key = Reflector.GetValue(propName, x) as string;
				if(!ht.ContainsKey(key)) { ht.Add(key, x); }
			}
			return Hashtable2Cols<T>(ht);
		}

		public static string[] Distinct(string[] cols)
		{
			IEnumerable<string> distincts = cols.Distinct<string>();
			List<string> list = new List<string>();
			foreach(string x in distincts) {
				list.Add(x);
			}
			return list.ToArray();
		}
		#endregion

		#region RequestValue
		public static string RequestValue(NameValueCollection nvs, string name)
		{
			return RequestValue(nvs, name, string.Empty);
		}

		public static string RequestValue(NameValueCollection nvs, string name, string airBag)
		{
			if(null == nvs || nvs.Count == 0) { return airBag; }

			string[] values = nvs.GetValues(name);
			if(Checker.IsNull(values)) { return airBag; }

			string rtn = string.Empty;
			foreach(string x in values) {
				if(!String.IsNullOrEmpty(rtn)) { rtn += ";"; }
				rtn += x;
			}

			return rtn;
		}
		#endregion

		#region AbstIntoCols
		public static string[] AbstIntoCols<T>(T[] cols, string propName)
		{
			return AbstIntoCols<T>(cols, propName, true);
		}
		public static string[] AbstIntoCols<T>(
			T[] cols, string propName, Func<T, bool> predict)
		{
			return AbstIntoCols<T>(cols, propName, predict, true);
		}

		public static string[] AbstIntoCols<T>(T[] cols, string propName, bool distinct)
		{
			return AbstIntoCols<T>(cols, propName, null, true);
		}

		public static string[] AbstIntoCols<T>(
			T[] cols, string propName, Func<T, bool> predict, bool distinct)
		{
			if(Checker.IsNull<T>(cols)) { return new String[0]; }

			PropertyInfo info = Reflector.GetProperty<T>(propName);

			List<string> list = new List<string>();
			foreach(T x in cols) {
				if(null == x) { continue; }
				if(null != predict && !predict(x)) { continue; }

				object val = (null == info) ? x.ToString() : info.GetValue(x, null);
				if(
					String.IsNullOrEmpty(val.ToString())
					||
					(distinct && list.Contains(val.ToString()))) {
					continue;
				}

				list.Add(val.ToString());
			}
			if(null == list || list.Count == 0) { return new String[0]; }

			return list.ToArray();
		}
		#endregion

		#region ColsIntoStr
		public static string ColsIntoStr<T>(T[] cols)
		{
			return ColsIntoStr<T>(cols, string.Empty, ",", null);
		}

		public static string ColsIntoStr<T>(T[] cols, string propName)
		{
			return ColsIntoStr<T>(cols, propName, ",", null);
		}

		public static string ColsIntoStr<T>(T[] cols, string propName, string separator)
		{
			return ColsIntoStr<T>(cols, propName, separator, null);
		}

		public static string ColsIntoStr<T>(
			T[] cols,
			string propName,
			string separator,
			string quoter)
		{
			if(Checker.IsNull<T>(cols)) { return string.Empty; }
			string[] propValues;
			if(Checker.IsNull(propName)) {
				List<string> sCol = new List<string>(cols.Length);
				foreach(T x in cols) {
					sCol.Add(x.ToString());
				}
				propValues = sCol.ToArray();
			} else {
				propValues = AbstIntoCols<T>(cols, propName);
			}
			return ColsIntoStr(propValues, separator, quoter);
		}

		public static string ColsIntoStr(string[] cols)
		{
			return ColsIntoStr(cols, ",", null);
		}

		public static string ColsIntoStr(string[] cols, string separator)
		{
			return ColsIntoStr(cols, separator, string.Empty);
		}

		public static string ColsIntoStr(string[] cols, string separator, string quoter)
		{
			return ColsIntoStr(cols, separator, quoter, quoter);
		}

		public static string ColsIntoStr(
			string[] cols,
			string separator,
			string quoter0,
			string quoter9)
		{
			if(Checker.IsNull(cols)) { return null; }
			if(String.IsNullOrEmpty(separator)) { separator = string.Empty; }
			if(String.IsNullOrEmpty(quoter0)) { quoter0 = string.Empty; }
			if(String.IsNullOrEmpty(quoter9)) { quoter9 = string.Empty; }

			StringBuilder sb = new StringBuilder();

			foreach(string x in cols) {
				sb.AppendFormat("{0}{1}{2}", quoter0, x, quoter9);
				sb.Append(separator);
			}

			return sb.ToString().TrimEnd(separator.ToCharArray());
		}
		#endregion

		#region NVc & NVs
		public static NameValueCollection Anys2NVc(params Any[] nvs)
		{
			NameValueCollection nvc = new NameValueCollection();
			if(null == nvs || nvs.Length == 0) { return nvc; }

			foreach(Any nv in nvs) {
				nvc.Add(nv.Name, nv.Value.ToString());
			}

			return nvc;
		}

		public static Any[] NVc2NVs(NameValueCollection nvc)
		{
			List<Any> list = new List<Any>();
			if(null == nvc || nvc.Count == 0) { return new Any[0]; }

			for(int i = 0; i < nvc.Count; i++) {
				Any nv = new Any(nvc.GetKey(i), nvc.Get(i));
				list.Add(nv);
			}

			return list.ToArray();
		}
		#endregion

		#region Sort
		public static void Sort<T>(ref T[] cols, string propertyName)
		{
			if(null == cols || cols.Length == 0) { return; }

			int times = 10000;
			SortedList sort = new SortedList();
			foreach(T x in cols) {
				times++;
				object key = Reflector.GetValue(propertyName, x);
				key = Formator.AirBag(key, x);
				key = key.ToString() + times.ToString();
				sort.Add(key, x);
			}

			sort.Values.CopyTo(cols, 0);
		}

		public static void Sort(ref ArrayList al, string propertyName)
		{
			if(null == al || al.Count == 0) { return; }

			int times = 10000;
			SortedList sort = new SortedList();
			foreach(object x in al) {
				times++;
				object key = Reflector.GetValue(propertyName, x);
				key = Formator.AirBag(key, x);
				key = key.ToString() + times.ToString();
				sort.Add(key, x);
			}

			al = new ArrayList();
			for(int i = 0; i < sort.Count; i++) {
				object s = sort.GetByIndex(i);
				al.Add(s);
			}
		}
		#endregion

		#region CPlum
		/// <summary>
		/// same as CPlum in aspfun, to prevent sql injection
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string CPlum(string s)
		{
			return String.IsNullOrEmpty(s) ? string.Empty : s.Replace("'", "''");
		}
		#endregion

		#region Count
		public static int Count(char target, string from)
		{
			Regex r = new Regex(target.ToString());
			return r.Matches(from).Count;
		}
		#endregion

		#region Cast
		public static int[] CastInt(string[] values)
		{
			List<int> list = new List<int>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				int val = Formator.AirBagToInt(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static short[] CastShort(string[] values)
		{
			List<short> list = new List<short>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				short val = Formator.AirBagToShort(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static DateTime[] CastDateTime(string[] values)
		{
			List<DateTime> list = new List<DateTime>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				DateTime val = Formator.AirBagToDateTime(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static double[] CastDouble(string[] values)
		{
			List<double> list = new List<double>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				double val = Formator.AirBagToDouble(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static decimal[] CastDecimal(string[] values)
		{
			List<decimal> list = new List<decimal>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				decimal val = Formator.AirBagToDecimal(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static bool[] CastBoolean(string[] values)
		{
			List<bool> list = new List<bool>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				bool val = Formator.AirBagToBoolean(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static byte[] CastByte(string[] values)
		{
			List<byte> list = new List<byte>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(string x in values) {
				byte val = Formator.AirBagToByte(x);
				list.Add(val);
			}
			return list.ToArray();
		}

		public static string[] CastString(object[] values)
		{
			List<string> list = new List<string>();
			if(Checker.IsNull(values)) { return list.ToArray(); }

			foreach(object x in values) {
				string val = Formator.AirBagToString(x.ToString());
				list.Add(val);
			}
			return list.ToArray();
		}

		public static List<T> Cast<T>(List<object> objs)
		{
			List<T> list = new List<T>();
			if(Checker.IsNull(objs)) { return list; }

			//Type type = typeof(T);
			foreach(object x in objs) {
				try {
					list.Add((T)x);
				} catch(Exception ex) {
					Logger.Error(
						ex,
						"Kuick.Formator.Cast",
						ex.ToAny()
					);
				}
			}

			return list;
		}

		public static T[] Cast<T>(object[] objs)
		{
			List<T> list = new List<T>();
			if(Checker.IsNull(objs)) { return list.ToArray(); }

			//Type type = typeof(T);
			foreach(object x in objs) {
				try {
					list.Add((T)x);
				} catch(Exception ex) {
					Logger.Error(
						ex,
						"Kuick.Formator.Cast",
						ex.ToAny()
					);
				}
			}

			return list.ToArray();
		}
		#endregion

		#region ToString
		public static string ToString(params object[] values)
		{
			if(null == values || values.Length == 0) { return string.Empty; }
			StringBuilder sb = new StringBuilder();
			foreach(object value in values) {
				if(sb.Length > 0) { sb.Append(Constants.Symbol.Semicolon); }


				sb.Append(value.GetType().IsDateTime()
					? ToString17((DateTime)value)
					: value.ToString()
				);
			}
			return sb.ToString();
		}
		#endregion

		#region Cut
		public static string Cut(string val, int maxLength)
		{
			string result = string.Empty;
			int byteLen = Encoding.Default.GetByteCount(val);
			int charLen = val.Length;
			int count = 0;
			int pos = 0;

			if(byteLen > maxLength) {
				for(int i = 0; i < charLen; i++) {
					count += (Convert.ToInt32(val.ToCharArray()[i]) > 255) ? 2 : 1;

					if(count > maxLength) {
						pos = i;
						break;
					} else if(count == maxLength) {
						pos = i + 1;
						break;
					}
				}
				if(pos >= 0) { result = val.Substring(0, pos); }
			} else {
				result = val;
			}

			return result;
		}
		#endregion

		#region Path encode & decode
		public static string PahEncode(string pathPlainText)
		{
			// /, -, .
			pathPlainText = pathPlainText.Replace("\\", "__1__");
			pathPlainText = pathPlainText.Replace("-", "__2__");
			pathPlainText = pathPlainText.Replace(".", "__3__");
			pathPlainText = pathPlainText.Replace(" ", "__4__");
			return pathPlainText;
		}

		public static string PahDecode(string pathEncodeString)
		{
			pathEncodeString = pathEncodeString.Replace("__4__", " ");
			pathEncodeString = pathEncodeString.Replace("__3__", ".");
			pathEncodeString = pathEncodeString.Replace("__2__", "-");
			pathEncodeString = pathEncodeString.Replace("__1__", "\\");
			return pathEncodeString;
		}
		#endregion

		#region Boolean vs. String
		public static string Boolean2String(bool b)
		{
			return b ? Constants.StringBoolean.True : Constants.StringBoolean.False;
		}

		public static bool String2Boolean(string v)
		{
			return v.Equals(
				Constants.StringBoolean.True,
				StringComparison.OrdinalIgnoreCase
			);
		}
		#endregion

		#region Boolean vs. Bit
		public static string Boolean2Bit(bool b)
		{
			return b ? "1" : "0";
		}

		public static bool String2Bit(string v)
		{
			return v.Equals("1", StringComparison.OrdinalIgnoreCase);
		}
		#endregion

		#region Find
		public static T Find<T>(List<T> list, params Any[] anys)
		{
			return Formator.FilterMainFirst<T>(list.ToArray(), anys);
		}
		#endregion

		#region Any
		public static Any[] ToAnys<T>(T[] cols)
		{
			List<Any> list = new List<Any>();
			int index = 0;
			foreach(T x in cols) {
				list.Add(new Any(
					String.Format("{0}[{1}]", cols.GetType().Name, (++index).ToString()),
					x
				));
			}
			return list.ToArray();
		}

		public static Any[] FlagEnumToAnys(Type type)
		{
			if(!Checker.IsFlagEnum(type)) {
				throw new Exception("Only flag enum type can call this method!");
			}

			List<Any> anys = new List<Any>();
			string[] names = Enum.GetNames(type);
			Array values = Enum.GetValues(type);
			for(int i = 0; i < names.Length; i++) {
				anys.Add(new Any(names[i], (int)values.GetValue(i)));
			}
			return anys.ToArray();
		}

		/// <summary>
		/// Any array transfers into IDictionary collection.
		/// </summary>
		/// <param name="anys"></param>
		/// <returns></returns>
		public static IDictionary<string, string> ToDictionary(params Any[] anys)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			foreach(Any any in anys) {
				dic.SafeAdd<string, string>(any.Name, any.ToString());
			}
			return dic;
		}
		#endregion

		#region Compare
		public static bool Between<T>(
			T comparable,
			T comparableA,
			T comparableB) where T : IComparable
		{
			// two parameters
			// ----|XXXX|----
			return
				comparable.CompareTo(comparableA) >= 0
				&&
				comparable.CompareTo(comparableB) <= 0;
		}

		public static bool Besides<T>(
			T comparable,
			T comparableA,
			T comparableB) where T : IComparable
		{
			// two parameters
			// XXXX|----|XXXX
			return
				comparable.CompareTo(comparableA) < 0
				||
				comparable.CompareTo(comparableB) > 0;
		}

		public static bool Greater<T>(T comparable, T comparableA) where T : IComparable
		{
			// one parameter
			// ----|XXXXXXXXX
			return comparable.CompareTo(comparableA) > 0;
		}

		public static bool Smaller<T>(T comparable, T comparableA) where T : IComparable
		{
			// one parameter
			// XXXXXXXXX|----
			return comparable.CompareTo(comparableA) < 0;
		}
		#endregion

		#region Equatable
		public static bool In<T>(T value, params T[] values) where T : IEquatable<T>
		{
			if(null == value) { return false; }
			// params T[]
			// Or operation
			foreach(T x in values) {
				if(value.Equals(x)) { return true; }
			}
			return false;
		}

		public static bool Exclude<T>(T value, params T[] values) where T : IEquatable<T>
		{
			if(null == value) { return false; }
			// params T[]
			// And operation
			foreach(T x in values) {
				if(value.Equals(x)) { return false; }
			}
			return true;
		}
		#endregion

		#region Misc

		/// <summary>
		/// Shortcut for Microsoft.Security.Application.Encoder.JavaScriptEncode, emitQuotes = false
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string JavaScriptEncode(string input)
		{
			return HttpUtility.JavaScriptStringEncode(input, false);
		}

		/// <summary>
		/// Shortcut for Microsoft.Security.Application.Encoder.JavaScriptEncode
		/// </summary>
		/// <param name="input"></param>
		/// <param name="emitQuotes"></param>
		/// <returns></returns>
		public static string JavaScriptEncode(string input, bool emitQuotes)
		{
			return HttpUtility.JavaScriptStringEncode(input ?? String.Empty, emitQuotes);
		}

		public static string StripHTML(string html)
		{
			return StripHTML(html, string.Empty);
		}

		public static string StripHTML(string html, string replacement)
		{
			return HtmlDecode(Regex.Replace(
				html,
				Constants.Pattern.Html,
				replacement,
				RegexOptions.Compiled
			));
		}
		#endregion

		#region Casing
		public static string UpperToPascalCasing(string upperCasing)
		{
			if(Checker.IsNull(upperCasing)) { return string.Empty; }

			string[] words = upperCasing.Split(Constants.Symbol.Char.UnderScore);
			for(int i = 0; i < words.Length; i++) {
				string word = words[i];
				if(word.Length < 3) { continue; }
				if(word != word.ToUpper()) { continue; }
				if(!Checker.IsNull(word)) {
					char firstLetter = char.ToUpper(word[0]);
					words[i] = firstLetter + word.Substring(1).ToLower();
				}
			}

			return string.Join(string.Empty, words);
		}

		public static string PascalToUpperCasing(string pascalCasing)
		{
			if(Checker.IsNull(pascalCasing)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			bool preIsUpper = false;
			char preChar = '1';
			foreach(char letter in pascalCasing.ToCharArray()) {
				if(
					letter == Constants.Symbol.Char.UnderScore
					&& preChar == Constants.Symbol.Char.UnderScore) {
					continue;
				}

				if(char.IsUpper(letter)) {
					if(!preIsUpper) {
						preIsUpper = true;
						if(sb.Length > 0 && preChar != Constants.Symbol.Char.UnderScore) {
							sb.Append(Constants.Symbol.Char.UnderScore);
						}
					}
					sb.Append(letter);
					preChar = letter;
				} else {
					preIsUpper = false;
					sb.Append(char.ToUpper(letter));
				}
			}

			return sb.ToString();
		}

		public static string DividePascalCasing(string pascalCasing)
		{
			if(Checker.IsNull(pascalCasing)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			bool preIsUpper = false;
			foreach(char letter in pascalCasing.ToCharArray()) {
				if(char.IsUpper(letter)) {
					if(!preIsUpper) {
						preIsUpper = true;
						if(sb.Length > 0) { sb.Append(Constants.Symbol.Char.Space); }
					}
					sb.Append(letter);
				} else {
					preIsUpper = false;
					sb.Append(letter);
				}
			}

			return sb.ToString();
		}

		public static string ToLowerAndCompact(string str)
		{
			return null == str
				? string.Empty
				: str
					.Replace(Constants.Symbol.UnderScore, string.Empty)
					.ToLower()
					.Trim();
		}
		#endregion

		#region Networking: IP string / long convertor
		/// <summary>
		/// IP Convertor: IP string to IP long
		/// </summary>
		/// <param name="ip">IP string</param>
		/// <returns>IP long</returns>
		public static long IPToLong(string ip)
		{
			// Checker
			if(!Checker.IsIP(ip)) { return -1; }

			// Convert
			IPAddress ipAddress;
			if(!IPAddress.TryParse(ip, out ipAddress)) { return -1; }

			string[] ips = ip.Split(Constants.Symbol.Char.Period);
			if(ips.Length != 4) { return -1; }

			return
				(long.Parse(ips[0]) << 24)
				+
				(long.Parse(ips[1]) << 16)
				+
				(long.Parse(ips[2]) << 8)
				+
				(long.Parse(ips[3]));
		}

		/// <summary>
		/// IP Convertor: IP long to IP string
		/// </summary>
		/// <param name="ip">IP long</param>
		/// <returns>IP string</returns>
		public static string LongToIP(long ip)
		{
			IPAddress ipAddress = new IPAddress(ip);
			string[] ips = ipAddress.ToString().Split(Constants.Symbol.Char.Period);
			if(ips.Length != 4) { return Constants.Null.Ip; }
			return string.Format("{0}.{1}.{2}.{3}", ips[3], ips[2], ips[1], ips[0]);
		}

		/// <summary>
		/// IP Convertor: IPAddress to IP long
		/// </summary>
		/// <param name="ip">IPAddress object</param>
		/// <returns>IP long</returns>
		public static long IPAddressToLong(IPAddress ip)
		{
			return IPToLong(ip.ToString());
		}

		/// <summary>
		/// IP Convertor: IPAddress to IP string
		/// </summary>
		/// <param name="ip">IPAddress object</param>
		/// <returns>IP string</returns>
		public static string IPAddressToIP(IPAddress ip)
		{
			return ip.ToString();
		}
		#endregion

		#region SecondString
		public static string MillisecondToSecondString(double milliseconds)
		{
			return string.Format("{0:0.000}", milliseconds / 1000f);
		}
		#endregion

		#region CorrectKey
		public static string CorrectedKey(string key)
		{
			if(Checker.IsNull(key)) { return string.Empty; }

			string correctedKey = key;
			if(key.Contains(Constants.Symbol.Dollar)) {
				int pos = key.LastIndexOf(Constants.Symbol.Dollar);
				if(pos < key.Length) { correctedKey = key.Substring(pos + 1); }
			}

			return correctedKey;
		}
		#endregion
	}
}

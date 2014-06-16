// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Extender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Web;
using System.Xml.Linq;
using System.Security;
using System.Runtime.InteropServices;

namespace Kuick
{
	public static class Extender
	{
		private static object _Lock = new object();

		// customer
		#region Any[]
		public static ListItem[] ToListItems(this Any[] anys)
		{
			if(null == anys) { anys = new Any[0]; }

			return Formator.ListItemsFromAnys(anys);
		}

		public static ListItemCollection ToListItemCollection(this Any[] anys)
		{
			if(null == anys) { anys = new Any[0]; }

			return Formator.ListItemCollectionFromAnys(anys);
		}

		public static NameValueCollection Anys2NVc(this Any[] anys)
		{
			if(null == anys) { anys = new Any[0]; }

			return Formator.Anys2NVc(anys);
		}

		public static IDictionary<string, string> ToDictionary(this Any[] anys)
		{
			return Formator.ToDictionary(anys);
		}

		public static string List(this List<Any> anys)
		{
			if(null == anys) { anys = new List<Any>(); }
			return Formator.ListAnys(anys.ToArray());
		}

		public static string List(this Any[] anys)
		{
			if(null == anys) { anys = new Any[0]; }
			return Formator.ListAnys(anys);
		}

		public static string Serialize(this Any[] anys)
		{
			if(null == anys) { anys = new Any[0]; }
			return Serializer.ToXml(anys);
		}
		#endregion

		// original
		#region string
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static string Left(this string input, int length)
		{
			if(Checker.IsNull(input)) { return string.Empty; }
			return input.Strip(length);
		}
		public static string Right(this string input, int length)
		{
			if(Checker.IsNull(input)) { return string.Empty; }
			if(length >= input.Length) { return input; }
			return input.Substring(input.Length - length, length);
		}

		/// <summary>
		/// Short cut for System.Text.RegularExpressions.Regex.Split
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		//public static string[] Split(this string input, string pattern)
		//{
		//    if(Checker.IsNull(input)) { return new string[0]; }
		//    string[] parts = Regex.Split(input, pattern);
		//    List<string> list = new List<string>();
		//    foreach(string x in parts) {
		//        list.Add(x.Trim());
		//    }
		//    return list.ToArray();
		//}

		/// <summary>
		/// Short cut for System.Text.RegularExpressions.Regex.Split
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string[] Split(this string input, string pattern, RegexOptions options)
		{
			if(Checker.IsNull(input)) { return new string[0]; }
			string[] parts = Regex.Split(input, pattern, options);
			List<string> list = new List<string>();
			foreach(string x in parts) {
				if(!Checker.IsNull(x)) { list.Add(x.Trim()); }
			}
			return list.ToArray();
		}

		public static string[] SplitWith(this string input, params string[] symbols)
		{
			if(Checker.IsNull(input)) { return new string[0]; }
			string[] parts = input.Split(symbols, StringSplitOptions.None);
			List<string> list = new List<string>();
			foreach(string x in parts) {
				if(!Checker.IsNull(x)) { list.Add(x.Trim()); }
			}
			return list.ToArray();
		}


		/// <summary>
		/// strip the string if string's length is bigger than the length
		/// </summary>
		/// <param name="length">strip to length</param>
		/// <param name="append">if strip, append this string</param>
		/// <returns></returns>
		public static string Strip(this string input, int length, string append)
		{
			if(Checker.IsNull(input)) { return string.Empty; }
			if(input.Length <= length) { return input; }
			return String.IsNullOrEmpty(append)
				? input.Substring(0, length)
				: String.Concat(input.Substring(0, length), append);
		}

		/// <summary>
		/// strip the string if string's length is bigger than the length
		/// </summary>
		/// <param name="length">strip to length</param>
		/// <returns></returns>
		public static string Strip(this string input, int length)
		{
			if(Checker.IsNull(input)) { return string.Empty; }
			string append = String.Empty;
			return Strip(input, length, append);
		}

		public static byte[] ToBytes(this string input)
		{
			return Formator.StringToBytes(input);
		}

		public static Stream ToStream(this string input)
		{
			return Formator.StringToStream(input);
		}

		public static string ToBase64(this string input)
		{
			return Formator.Base64Encode(input);
		}

		public static string FromBase64(this string input)
		{
			return Formator.Base64Decode(input);
		}

		public static string Repeat(this string seed, int times)
		{
			return Formator.Repeater(seed, times);
		}

		public static string ToMD5(this string str)
		{
			return Formator.EncodeMD5(str);
		}

		public static string ToSHA(this string str)
		{
			return Formator.EncodeSHA(str);
		}

		public static string Encrypt(this string data)
		{
			return Formator.Encrypt(Current.EncryptKey, data);
		}

		public static string Encrypt(this string data, string password)
		{
			return Formator.Encrypt(password, data);
		}

		public static string Decrypt(this string data)
		{
			return Formator.Decrypt(Current.EncryptKey, data);
		}

		public static string Decrypt(this string data, string password)
		{
			return Formator.Decrypt(password, data);
		}

		public static byte[] HexToBytes(this string hex)
		{
			return Formator.FromHex(hex);
		}

		public static string AirBag(this string value)
		{
			return Formator.AirBag(value);
		}

		public static string AirBag(this string value, string airBag)
		{
			return Formator.AirBag(value, airBag);
		}

		public static int AirBagToInt(this string value)
		{
			return Formator.AirBagToInt(value);
		}

		public static int AirBagToInt(this string value, int airBag)
		{
			return Formator.AirBagToInt(value, airBag);
		}

		public static decimal AirBagToDecimal(this string value)
		{
			return Formator.AirBagToDecimal(value);
		}

		public static decimal AirBagToDecimal(this string value, decimal airBag)
		{
			return Formator.AirBagToDecimal(value, airBag);
		}

		public static long AirBagToLong(this string value)
		{
			return Formator.AirBagToLong(value);
		}

		public static long AirBagToLong(this string value, long airBag)
		{
			return Formator.AirBagToLong(value, airBag);
		}

		public static float AirBagToFloat(this string value)
		{
			return Formator.AirBagToFloat(value);
		}

		public static float AirBagToFloat(this string value, float airBag)
		{
			return Formator.AirBagToFloat(value, airBag);
		}

		public static bool AirBagToBoolean(this string value)
		{
			return Formator.AirBagToBoolean(value);
		}

		public static bool AirBagToBoolean(this string value, bool airBag)
		{
			return Formator.AirBagToBoolean(value, airBag);
		}

		public static DateTime AirBagToDateTime(this string value)
		{
			return Formator.AirBagToDateTime(value);
		}

		public static DateTime AirBagToDateTime(this string value, DateTime airBag)
		{
			return Formator.AirBagToDateTime(value, airBag);
		}

		public static T AirBagToEnum<T>(this string value)
		{
			return Formator.AirBagToEnum<T>(value);
		}

		public static T AirBagToEnum<T>(this string value, T airBag)
		{
			return Formator.AirBagToEnum<T>(value, airBag);
		}

		public static string CPlum(this string s)
		{
			return String.IsNullOrEmpty(s) ? string.Empty : s.Replace("'", "''");
		}

		public static string HtmlEncode(this string value)
		{
			return Formator.HtmlEncode(value);
		}

		public static string HtmlDecode(this string value)
		{
			return Formator.HtmlDecode(value);
		}

		public static string UrlEncode(this string value)
		{
			return Formator.UrlEncode(value);
		}

		public static string UrlDecode(this string value)
		{
			return Formator.UrlDecode(value);
		}

		public static string StripHTML(this string value)
		{
			return Formator.StripHTML(value);
		}

		public static bool IsEncrypted(this string value)
		{
			return Checker.IsEncrypted(value);
		}

		public static bool In(this string value, params string[] values)
		{
			return Formator.In<string>(value, values);
		}

		public static bool Exclude(this string value, params string[] values)
		{
			return Formator.Exclude<string>(value, values);
		}

		public static bool ContainsAny(this string value, params string[] values)
		{
			return Formator.ContainsAny(value, values);
		}

		public static bool ContainsAll(this string value, params string[] values)
		{
			return Formator.ContainsAll(value, values);
		}

		public static bool StartWith(this string value, params string[] values)
		{
			return Formator.StartWith(value, values);
		}

		public static bool EndWith(this string value, params string[] values)
		{
			return Formator.EndWith(value, values);
		}

		public static bool IsMatch(this string value, string pattern, bool ignoreCase)
		{
			return Checker.IsMatch(value, pattern, ignoreCase);
		}

		public static bool IsMatch(this string value, string pattern)
		{
			return IsMatch(value, pattern, false);
		}

		public static bool LengthRange(this string value, int minimumLength, int maximumLength)
		{
			return Formator.LengthRange(value, minimumLength, maximumLength);
		}

		public static bool LengthBiggerThan(this string value, int length)
		{
			return Formator.LengthBiggerThan(value, length);
		}

		public static bool LengthSmallerThan(this string value, int length)
		{
			return Formator.LengthSmallerThan(value, length);
		}

		public static Anys ToAnys(this string value)
		{
			return ToAnys(
				value,
				Constants.Default.Separator,
				Constants.Default.Connector
			);
		}

		public static Anys ToAnys(this string value, char separator, char connector)
		{
			if(Checker.IsNull(value)) { return new Anys(); }
			Anys anys = new Anys(value, separator, connector);
			return anys;
		}

		public static Anys ToAnys(this string value, string separator, string connector)
		{
			if(Checker.IsNull(value)) { return new Anys(); }
			Anys anys = new Anys(value, separator, connector);
			return anys;
		}

		public static string TrimStart(this string value, params string[] trimStrings)
		{
			if(string.IsNullOrEmpty(value)) { return value; }
			foreach(string trimString in trimStrings) {
				if(trimString.Length > value.Length) { continue; }
				if(value.StartsWith(trimString, StringComparison.Ordinal)) {
					return value.Substring(
						trimString.Length,
						value.Length - trimString.Length
					);
				}
			}
			return value;
		}

		public static string TrimEnd(this string value, params string[] trimStrings)
		{
			if(string.IsNullOrEmpty(value)) { return value; }
			foreach(string trimString in trimStrings) {
				if(trimString.Length > value.Length) { continue; }
				if(value.EndsWith(trimString, StringComparison.Ordinal)) {
					return value.Substring(0, value.Length - trimString.Length);
				}
			}
			return value;
		}

		public static string ToUnicodeEntity(this string source)
		{
			return Formator.ToUnicodeEntity(source);
		}

		public static string Mask(this string str)
		{
			if(string.IsNullOrEmpty(str)) { return string.Empty; }

			switch(str.Length) {
				case 1:
					return Constants.Symbol.Asterisk;
				case 2:
					return str.Left(1) + Constants.Symbol.Asterisk;
				default:
					return string.Concat(
						str.Left(1),
						Constants.Symbol.Asterisk.Repeat(str.Length - 2),
						str.Right(1)
					);
			}
		}

		public static string Replace(this string template, params string[] strings)
		{
			StringBuilder sb = new StringBuilder(template);
			if(!Checker.IsNull(strings)) {
				foreach(string str in strings) {
					sb.Replace(str, string.Empty);
				}
			}
			return sb.ToString();
		}

		public static string UpperToPascalCasing(this string txt)
		{
			if(txt.IsNullOrEmpty()) { return string.Empty; }
			return Formator.UpperToPascalCasing(txt);
		}

		public static string PascalToUpperCasing(this string txt)
		{
			if(txt.IsNullOrEmpty()) { return string.Empty; }
			return Formator.PascalToUpperCasing(txt);
		}

		public static HtmlString ToHtml(this string txt)
		{
			return new HtmlString(txt.AirBag());
		}

		public static bool IsInteger(this string value)
		{
			return Checker.IsInt(value);
		}

		public static bool IsDecimal(this string value)
		{
			return Checker.IsDecimal(value);
		}

		public static bool IsLong(this string value)
		{
			return Checker.IsLong(value);
		}

		public static bool IsFloat(this string value)
		{
			return Checker.IsFloat(value);
		}

		public static bool IsDouble(this string value)
		{
			return Checker.IsDouble(value);
		}

		public static bool IsByte(this string value)
		{
			return Checker.IsByte(value);
		}

		public static bool IsChar(this string value)
		{
			return Checker.IsChar(value);
		}

		public static bool IsDateTime(this string value)
		{
			return Checker.IsDateTime(value);
		}

		public static bool IsGuid(this string value)
		{
			return Checker.IsGuid(value);
		}

		public static bool IsTaiwanIdentificationCardNumber(this string value)
		{
			return Checker.IsTaiwanIdentificationCardNumber(value);
		}

		public static bool IsChinaIdentificationCardNumber(this string value)
		{
			return Checker.IsChinaIdentificationCardNumber(value);
		}

		public static bool IsCreditcardNumber(this string value)
		{
			return Checker.IsCreditcardNumber(value);
		}

		public static bool IsUnifiedSerialNumber(this string value)
		{
			return Checker.IsUnifiedSerialNumber(value);
		}

		public static bool IsEmailAddress(this string value)
		{
			return Checker.IsEmailAddress(value);
		}

		public static bool IsIPAddress(this string value)
		{
			return Checker.IsIPAddress(value);
		}

		public static bool IsLoopBackIP(this string value)
		{
			return Checker.IsLoopBackIP(value);
		}

		public static bool IsPrivateIP(this string value)
		{
			return Checker.IsPrivateIP(value);
		}

		public static bool IsNullIP(this string value)
		{
			return Checker.IsNullIP(value);
		}

		public static bool IsSqlUUID(this string value)
		{
			return Checker.IsSqlUUID(value);
		}

		public static bool IsAlphabet(this string value)
		{
			return Checker.IsAlphabet(value);
		}

		public static bool IsNumeric(this string value)
		{
			return Checker.IsNumeric(value);
		}

		public static bool IsAlphaNumeric(this string value)
		{
			return Checker.IsAlphaNumeric(value);
		}

		public static bool IsUri(this string value)
		{
			return Checker.IsUri(value);
		}
		#endregion

		#region string[]
		public static string[] Exclude(this string[] cols, params string[] excludeItems)
		{
			return Formator.Exclude(cols, excludeItems);
		}

		public static string[] Distinct(this string[] cols)
		{
			return Formator.Distinct(cols);
		}

		public static string ToString(this string[] cols)
		{
			return Formator.ColsIntoStr(cols);
		}

		public static string ToString(this string[] cols, string separator)
		{
			return Formator.ColsIntoStr(cols, separator);
		}

		public static string ToString(this string[] cols, string separator, string quoter)
		{
			return Formator.ColsIntoStr(cols, separator, quoter);
		}

		public static string ToString(
			this string[] cols,
			string separator,
			string quoter0,
			string quoter9)
		{
			return Formator.ColsIntoStr(cols, separator, quoter0, quoter9);
		}

		//public static string ColsIntoStr(this string[] cols)
		//{
		//    return Formator.ColsIntoStr(cols);
		//}

		//public static string ColsIntoStr(this string[] cols, string separator)
		//{
		//    return Formator.ColsIntoStr(cols, separator);
		//}

		//public static string ColsIntoStr(this string[] cols, string separator, string quoter)
		//{
		//    return Formator.ColsIntoStr(cols, separator, quoter);
		//}

		//public static string ColsIntoStr(
		//    this string[] cols,
		//    string separator,
		//    string quoter0,
		//    string quoter9)
		//{
		//    return Formator.ColsIntoStr(cols, separator, quoter0, quoter9);
		//}

		public static Any[] ToAnys(this string[] cols)
		{
			return Formator.ToAnys<string>(cols);
		}

		public static bool StartWith(this string[] tests, params string[] values)
		{
			if(Checker.IsNull(tests)) { return false; }
			foreach(string x in tests) {
				if(Formator.StartWith(x, values)) { return true; }
			}
			return false;
		}

		public static bool EndWith(this string[] tests, params string[] values)
		{
			if(Checker.IsNull(tests)) { return false; }
			foreach(string x in tests) {
				if(Formator.EndWith(x, values)) { return true; }
			}
			return false;
		}

		public static string Join(this string[] cols, string seperator)
		{
			return ToString(cols, seperator);
		}

		public static string Join(
			this string[] cols,
			string seperator,
			string quoter)
		{
			return ToString(cols, seperator, quoter);
		}

		public static string Join(
			this string[] cols,
			string seperator,
			string quoter0,
			string quoter9)
		{
			return ToString(cols, seperator, quoter0, quoter9);
		}
		#endregion

		#region byte[]
		public static string ToString(this byte[] input)
		{
			return Formator.BytesToString(input);
		}

		public static Stream ToStream(this byte[] input)
		{
			return Formator.BytesToStream(input);
		}

		/// <summary>
		/// Save the byte array to file.
		/// </summary>
		/// <param name="saveTo">path with filename to save</param>
		public static void SaveToFile(this byte[] input, string saveTo)
		{
			if(Checker.IsNull(input)) { return; }
			string p = Path.GetDirectoryName(saveTo);
			if(!Directory.Exists(p)) { Directory.CreateDirectory(p); }

			using(FileStream writeStream =
				new FileStream(saveTo, FileMode.Create, FileAccess.Write)) {
				writeStream.Write(input, 0, input.Length);
				writeStream.Close();
			}
		}

		public static string ToHex(this byte[] b)
		{
			return Formator.ToHex(b);
		}

		public static Any[] ToAnys(this byte[] b)
		{
			return Formator.ToAnys<byte>(b);
		}
		#endregion

		#region Stream
		public static string ToString(this Stream input)
		{
			return Formator.StreamToString(input);
		}

		/// <summary>
		/// convert to byte array
		/// </summary>
		/// <returns></returns>
		public static byte[] ToBytes(this Stream input)
		{
			return Formator.StreamToBtyes(input);
		}

		/// <summary>
		/// save to file
		/// </summary>
		/// <param name="input"></param>
		/// <param name="fileWithPath"></param>
		public static void SaveToFile(this Stream input, string fileWithPath)
		{
			if(Checker.IsNull(input)) { return; }
			using(FileStream fs = new FileStream(fileWithPath, FileMode.Create)) {
				using(BinaryWriter bw = new BinaryWriter(fs)) {
					bw.Write(input.ToBytes());
					bw.Close();
				}
				fs.Close();
			}
		}
		#endregion

		#region DirectoryInfo
		public static void Empty(this DirectoryInfo directory)
		{
			foreach(FileInfo file in directory.GetFiles()) {
				file.Delete();
			}
			foreach(DirectoryInfo subDirectory in directory.GetDirectories()) {
				subDirectory.Delete(true);
			}
		}
		#endregion

		#region char
		public static int Count(this char target, string from)
		{
			if(Checker.IsNull(target)) { return 0; }
			return Formator.Count(target, from);
		}
		#endregion

		#region char[]
		public static Any[] ToAnys(this char[] chars)
		{
			if(Checker.IsNull(chars)) { return new Any[0]; }
			return Formator.ToAnys<char>(chars);
		}
		#endregion

		#region DateTime
		public static string yyyy(this DateTime date)
		{
			return Formator.ToString4(date);
		}

		public static string yyyyMM(this DateTime date)
		{
			return date.ToString("yyyy-MM");
		}

		public static string yyyyMMs(this DateTime date)
		{
			return date.ToString("yyyyMM");
		}

		public static string yyyyMMdd(this DateTime date)
		{
			return Formator.ToString8(date);
		}

		public static string yyyyMMdds(this DateTime date)
		{
			return Formator.ToString8s(date);
		}

		public static string yyyyMMddHH(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH");
		}

		public static string yyyyMMddHHs(this DateTime date)
		{
			return date.ToString("yyyyMMddHH");
		}

		public static string yyyyMMddHHmm(this DateTime date)
		{
			return Formator.ToString12(date);
		}

		public static string yyyyMMddHHmms(this DateTime date)
		{
			return Formator.ToString12s(date);
		}

		public static string yyyyMMddHHmmss(this DateTime date)
		{
			return Formator.ToString14(date);
		}

		public static string yyyyMMddHHmmsss(this DateTime date)
		{
			return Formator.ToString14s(date);
		}

		public static string yyyyMMddHHmmssfff(this DateTime date)
		{
			return Formator.ToString17(date);
		}

		public static string yyyyMMddHHmmssfffs(this DateTime date)
		{
			return Formator.ToString17s(date);
		}

		public static string yyyyMMddHHmmssffffff(this DateTime date)
		{
			return Formator.ToString20(date);
		}

		public static string yyyyMMddHHmmssffffffs(this DateTime date)
		{
			return Formator.ToString20s(date);
		}

		public static string MM(this DateTime date)
		{
			return date.ToString("MM");
		}

		public static string dd(this DateTime date)
		{
			return date.ToString("dd");
		}

		public static string hh(this DateTime date)
		{
			return date.ToString("hh");
		}

		public static string mm(this DateTime date)
		{
			return date.ToString("mm");
		}

		public static string ss(this DateTime date)
		{
			return date.ToString("ss");
		}

		public static string hhmm(this DateTime date)
		{
			return date.ToString("hh:mm");
		}

		public static string hhmmss(this DateTime date)
		{
			return date.ToString("hh:mm:ss");
		}

		public static DateTime AirBag(this DateTime dateTime)
		{
			return Formator.AirBag(dateTime);
		}

		public static DateTime AirBag(this DateTime dateTime, DateTime airBag)
		{
			return Formator.AirBag(dateTime, airBag);
		}

		public static bool Between(this DateTime value, DateTime valueA, DateTime valueB)
		{
			return Formator.Between<DateTime>(value, valueA, valueB);
		}

		public static bool Besides(this DateTime value, DateTime valueA, DateTime valueB)
		{
			return Formator.Besides<DateTime>(value, valueA, valueB);
		}

		public static bool Greater(this DateTime value, DateTime valueA)
		{
			return Formator.Greater<DateTime>(value, valueA);
		}

		public static bool Smaller(this DateTime value, DateTime valueA)
		{
			return Formator.Smaller<DateTime>(value, valueA);
		}

		public static DateTime StartOfYear(this DateTime value)
		{
			return Dater.StartOfYear(value);
		}

		public static DateTime StartOfMonth(this DateTime value)
		{
			return Dater.StartOfMonth(value);
		}

		public static DateTime StartOfWeek(this DateTime value)
		{
			return Dater.StartOfWeek(value);
		}

		public static DateTime StartOfWeek(this DateTime value, DayOfWeek weekStart)
		{
			return Dater.StartOfWeek(value, weekStart);
		}

		public static DateTime StartOfDay(this DateTime value)
		{
			return Dater.StartOfDay(value);
		}

		public static DateTime StartOfHour(this DateTime value)
		{
			return Dater.StartOfHour(value);
		}

		public static DateTime StartOfMinute(this DateTime value)
		{
			return Dater.StartOfMinute(value);
		}

		public static DateTime EndOfYear(this DateTime value)
		{
			return Dater.EndOfYear(value);
		}

		public static DateTime EndOfMonth(this DateTime value)
		{
			return Dater.EndOfMonth(value);
		}

		public static DateTime EndOfWeek(this DateTime value, DayOfWeek weekStart)
		{
			return Dater.EndOfWeek(value, weekStart);
		}

		public static DateTime EndOfWeek(this DateTime value)
		{
			return Dater.EndOfWeek(value);
		}

		public static DateTime EndOfDay(this DateTime value)
		{
			return Dater.EndOfDay(value);
		}

		public static DateTime EndOfHour(this DateTime value)
		{
			return Dater.EndOfHour(value);
		}

		public static DateTime EndOfMinute(this DateTime value)
		{
			return Dater.EndOfMinute(value);
		}

		public static double ToUnixTimestamp(this DateTime value)
		{
			return Formator.ToUnixTimestamp(value);
		}
		#endregion

		#region DateTime[]
		public static Any[] ToAnys(this DateTime[] dateTimes)
		{
			return Formator.ToAnys<DateTime>(dateTimes);
		}
		#endregion

		#region int
		public static int AirBag(this int value)
		{
			return Formator.AirBag(value);
		}

		public static int AirBag(this int value, int airBag)
		{
			return Formator.AirBag(value, airBag);
		}

		public static string OrderList(this int no, int max)
		{
			return Formator.OrderList(no, max);
		}

		public static string OrderList(this int no, int max, bool withSuffix)
		{
			return Formator.OrderList(no, max, withSuffix);
		}

		public static bool Between(this int value, int valueA, int valueB)
		{
			return Formator.Between<int>(value, valueA, valueB);
		}

		public static bool Besides(this int value, int valueA, int valueB)
		{
			return Formator.Besides<int>(value, valueA, valueB);
		}

		public static bool Greater(this int value, int valueA)
		{
			return Formator.Greater<int>(value, valueA);
		}

		public static bool Smaller(this int value, int valueA)
		{
			return Formator.Smaller<int>(value, valueA);
		}

		public static bool In(this int value, params int[] values)
		{
			return Formator.In<int>(value, values);
		}

		public static bool In(this decimal value, params decimal[] values)
		{
			return Formator.In<decimal>(value, values);
		}

		public static bool In(this byte value, params byte[] values)
		{
			return Formator.In<byte>(value, values);
		}

		public static bool Exclude(this int value, params int[] values)
		{
			return Formator.Exclude<int>(value, values);
		}

		public static bool IsEven(this int value)
		{
			return Checker.IsEven(value);
		}

		public static bool IsOdd(this int value)
		{
			return Checker.IsOdd(value);
		}

		public static int Ceiling(this int value, int criticalValue)
		{
			return value > criticalValue ? criticalValue : value;
		}

		public static int Floor(this int value, int criticalValue)
		{
			return value < criticalValue ? criticalValue : value;
		}

		public static int Within(this int value, int ceilingValue, int floorValue)
		{
			// correct
			int max = Math.Max(ceilingValue, floorValue);
			int min = Math.Min(ceilingValue, floorValue);

			return value > max
				? max
				: value < min
					? min
					: value;
		}

		public static bool Divisible(this int dividend, int divisor)
		{
			return Calculator.Divisible(dividend, divisor);
		}

		public static int Remainder(this int dividend, int divisor)
		{
			return Calculator.Remainder(dividend, divisor);
		}

		public static int Quotient(this int dividend, int divisor)
		{
			return Calculator.Quotient(dividend, divisor);
		}
		#endregion

		#region int[]
		public static Any[] ToAnys(this int[] ints)
		{
			return Formator.ToAnys<int>(ints);
		}

		public static string ColsIntoStr(this int[] cols)
		{
			return Formator.ColsIntoStr<int>(cols);
		}
		#endregion

		#region long
		public static long AirBag(this long value)
		{
			return Formator.AirBag(value);
		}

		public static long AirBag(this long value, long airBag)
		{
			return Formator.AirBag(value, airBag);
		}

		public static bool Between(this long value, long valueA, long valueB)
		{
			Formator.Switch(ref valueA, ref valueB);
			return Formator.Between<long>(value, valueA, valueB);
		}

		public static bool Besides(this long value, long valueA, long valueB)
		{
			Formator.Switch(ref valueA, ref valueB);
			return Formator.Besides<long>(value, valueA, valueB);
		}

		public static bool Greater(this long value, long valueA)
		{
			return Formator.Greater<long>(value, valueA);
		}

		public static bool Smaller(this long value, long valueA)
		{
			return Formator.Smaller<long>(value, valueA);
		}

		public static bool In(this long value, params long[] values)
		{
			return Formator.In<long>(value, values);
		}

		public static bool Exclude(this long value, params long[] values)
		{
			return Formator.Exclude<long>(value, values);
		}

		public static bool IsEven(this long value)
		{
			return Checker.IsEven(value);
		}

		public static bool IsOdd(this long value)
		{
			return Checker.IsOdd(value);
		}

		public static bool Divisible(this long dividend, long divisor)
		{
			return Checker.Divisible(dividend, divisor);
		}

		public static long Remainder(this long dividend, long divisor)
		{
			return Calculator.Remainder(dividend, divisor);
		}

		public static long Quotient(this long dividend, long divisor)
		{
			return Calculator.Quotient(dividend, divisor);
		}
		#endregion

		#region float
		public static float AirBag(this float value)
		{
			return Formator.AirBag(value);
		}

		public static float AirBag(this float value, float airBag)
		{
			return Formator.AirBag(value, airBag);
		}

		public static bool Between(this float value, float valueA, float valueB)
		{
			return Formator.Between<float>(value, valueA, valueB);
		}

		public static bool Between(this decimal value, decimal valueA, decimal valueB)
		{
			return Formator.Between<decimal>(value, valueA, valueB);
		}

		public static bool Besides(this float value, float valueA, float valueB)
		{
			return Formator.Besides<float>(value, valueA, valueB);
		}

		public static bool Greater(this float value, float valueA)
		{
			return Formator.Greater<float>(value, valueA);
		}

		public static bool Smaller(this float value, float valueA)
		{
			return Formator.Smaller<float>(value, valueA);
		}

		public static bool In(this float value, params float[] values)
		{
			return Formator.In<float>(value, values);
		}

		public static bool Exclude(this float value, params float[] values)
		{
			return Formator.Exclude<float>(value, values);
		}
		#endregion

		#region bool
		public static string If(this bool isTrue, string text)
		{
			return isTrue ? text : string.Empty;
		}
		#endregion

		#region bool[]
		public static Any[] ToAnys(this bool[] bools)
		{
			if(Checker.IsNull(bools)) { return new Any[0]; }
			return Formator.ToAnys<bool>(bools);
		}
		#endregion

		#region ArrayList
		public static ArrayList Sort(this ArrayList al, string propertyName)
		{
			if(Checker.IsNull(al)) { return al; }
			Formator.Sort(ref al, propertyName);
			return al;
		}

		public static T[] ToCollection<T>(this ArrayList al)
		{
			return Formator.ArrayListToCols<T>(al);
		}

		public static Array ToArray<T>(this ArrayList arrayList)
		{
			return Formator.ArrayListToArray<T>(arrayList);
		}
		#endregion

		#region ListItem[]
		public static ListItemCollection ToListItemCollection(this ListItem[] listItems)
		{
			return Formator.ListItemCollectionFromArray(listItems);
		}

		public static Anys ToAnys(this ListItem[] listItems)
		{
			Anys anys = new Anys();
			if(Checker.IsNull(listItems)) { return anys; }

			if(!Checker.IsNull(listItems)) {
				foreach(ListItem x in listItems) {
					anys.Add(new Any(x.Text, x.Value));
				}
			}
			return anys;
		}
		#endregion

		#region ListItemCollection
		public static ListItem[] ToListItems(this ListItemCollection listItemCollection)
		{
			return Formator.ListItemCollectionToArray(listItemCollection);
		}
		#endregion

		#region Hashtable
		public static Hashtable AirBag(this Hashtable ht)
		{
			return Formator.AirBag(ht);
		}
		#endregion

		#region NameValueCollection
		public static string RequestValue(this NameValueCollection nvs, string name)
		{
			return Formator.RequestValue(nvs, name);
		}

		public static string RequestValue(
			this NameValueCollection nvs,
			string name,
			string airBag)
		{
			return Formator.RequestValue(nvs, name, airBag);
		}

		public static Any[] ToAnys(this NameValueCollection nvc)
		{
			return Formator.NVc2NVs(nvc);
		}

		public static bool Exists(this NameValueCollection nvs, params string[] names)
		{
			if(Checker.IsNull(nvs)) { return false; }
			foreach(string key in nvs.AllKeys) {
				foreach(string name in names) {
					if(name.IsNullOrEmpty()) { continue; }
					if(key.IsNullOrEmpty()) { continue; }
					if(key.Equals(name, StringComparison.OrdinalIgnoreCase)) {
						return true;
					}
				}
			}
			return false;
		}

		public static bool SafeAdd(
			this NameValueCollection nvs, string name, string value)
		{
			if(null == nvs) { return false; }
			lock(_Lock) {
				if(nvs.Exists(name)) {
					nvs.Remove(name);
				}
				nvs.Add(name, value);
				return true;
			}
		}

		public static bool SafeRemove(
			this NameValueCollection nvs, string name)
		{
			if(null == nvs) { return false; }
			lock(_Lock) {
				if(nvs.Exists(name)) {
					nvs.Remove(name);
				}
				return true;
			}
		}
		#endregion

		#region List<T>
		public static T[] ToCollection<T>(this List<T> list)
		{
			return Formator.ListToCols<T>(list);
		}

		public static Array ListToArray<T>(this List<T> list)
		{
			return Formator.ListToArray<T>(list);
		}

		/// <summary>
		/// swap values at position x and y
		/// </summary>
		/// <param name="x">position x</param>
		/// <param name="y">position y</param>
		public static void Swap<T>(this List<T> list, int x, int y)
		{
			if(Checker.IsNull(list)) { return; }
			int len = list.Count;
			if(0 <= x && x < len && 0 <= y && y < len && x != y) {
				T tmp = list[x];
				list[x] = list[y];
				list[y] = tmp;
			}
		}

		/// <summary>
		/// shuffle the list and return itself.
		/// WARNING: The list itself will be MODIFY.
		/// </summary>
		/// <returns>this</returns>
		public static List<T> Shuffle<T>(this List<T> list)
		{
			if(Checker.IsNull(list)) { return new List<T>(); }
			int j = list.Count;
			while(--j > 0) {
				list.Swap(j, Utility.GetRandom(0, j + 1));
			}
			return list;
		}

		/// <summary>
		/// Get the shuffle
		/// The list itself will NOT be modify.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> Shuffle<T>(this List<T> list, int count)
		{
			if(Checker.IsNull(list)) { return new List<T>(); }
			if(count > list.Count) { count = list.Count; }
			if(count < 1) { return new List<T>(); }

			List<T> ls = new List<T>(count);

			int[] tmp = new int[list.Count];
			int i = tmp.Length;
			while(--i >= 0) {
				tmp[i] = i;
			}

			int j = list.Count;
			int t = list.Count - count - 1;
			while(--j > t) {
				tmp.Swap(j, Utility.GetRandom(0, j + 1));
				ls.Add(list[tmp[j]]);
			}

			return ls;
		}

		public static string Join(this List<string> list, string separator)
		{
			if(Checker.IsNull(list)) { return string.Empty; }
			return list.ToArray().Join(separator);
		}

		public static string Join(
			this List<string> list,
			string separator,
			string quoter)
		{
			if(Checker.IsNull(list)) { return string.Empty; }
			return list.ToArray().Join(separator, quoter);
		}

		public static string Join(
			this List<string> list,
			string separator,
			string quoter0,
			string quoter9)
		{
			if(Checker.IsNull(list)) { return string.Empty; }
			return list.ToArray().Join(separator, quoter0, quoter9);
		}

		public static int Remove(this List<Any> list, params string[] names)
		{
			if(Checker.IsNull(list)) { return 0; }
			if(Checker.IsNull(names)) { return 0; }

			List<string> allNames = new List<string>();
			foreach(string name in names) {
				string newName = Formator.UpperToPascalCasing(name);
				if(!newName.ContainsAny(names)) {
					allNames.Add(newName);
				}
			}
			return list.Remove<Any>(allNames.ToArray());
		}

		/// <summary>
		/// Get the distinct item in list
		/// The list itself will NOT be modify.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static List<T> Distinct<T>(this List<T> list)
		{
			IEnumerable<T> distincted = Enumerable.Distinct<T>(list);
			List<T> n = new List<T>(distincted);
			return n;
		}

		public static int Remove<T>(this List<T> list, params string[] names) where T : IName
		{
			if(Checker.IsNull(list)) { return 0; }
			if(Checker.IsNull(names)) { return 0; }

			int removedCount = 0;
			while(true) {
				bool done = false;
				for(int i = 0; i < list.Count; i++) {
					var one = list[i];
					if(one.Name.In(names)) {
						list.RemoveAt(i);
						removedCount++;
						done = true;
						break;
					}
				}
				if(!done) { break; }
			}

			return removedCount;
		}

		//public static int Remove<T>(this List<T> list, Predicate<T> predicate)
		//{
		//    List<T> all = list.FindAll(predicate);
		//    int count = 0;
		//    if(null != all) {
		//        foreach(T one in all) {
		//            list.Remove(one);
		//            count++;
		//        }
		//    }
		//    return count;
		//}

		public static List<T> Filter<T>(this List<T> list, Func<T, bool> filter)
		{
			List<T> t = new List<T>(list.Count);
			foreach(T x in list) {
				if(filter(x)) { t.Add(x); }
			}
			return t;
		}

		public static T FilterFirst<T>(this List<T> list, Func<T, bool> filter)
		{
			foreach(T x in list) {
				if(filter(x)) { return x; }
			}
			return default(T);
		}

		public static List<O> Cast<T, O>(this List<T> list) where T : O
		{
			List<O> newList = new List<O>();
			foreach(T one in list) {
				newList.Add((O)one);
			}
			return newList;
		}

		public static bool Contains<T>(
			this List<T> list,
			Predicate<T> predicate)
		{
			foreach(T one in list) {
				if(predicate(one)) { return true; }
			}
			return false;
		}

		public static List<T> Add<T>(this List<T> list, params T[] values)
		{
			foreach(T one in values) {
				list.Add(one);
			}
			return list;
		}

		public static List<T> Add<T>(
			this List<T> list,
			bool condition,
			params T[] values)
		{
			if(!condition) { return list; }
			foreach(T one in values) {
				list.Add(one);
			}
			return list;
		}
		#endregion

		#region Type
		public static bool IsDerived<B>(this Type tType)
		{
			return Reflector.IsDerived<B>(tType);
		}

		public static bool IsDerived(this Type tType, Type bType)
		{
			return Reflector.IsDerived(tType, bType);
		}

		public static bool IsString(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(string));
		}

		public static bool IsBoolean(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Boolean));
		}

		public static bool IsInteger(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(int));
		}

		public static bool IsInt64(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Int64));
		}

		public static bool IsShort(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Int16));
		}

		public static bool IsSingle(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Single));
		}

		public static bool IsDouble(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Double));
		}

		public static bool IsLong(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Int64));
		}

		public static bool IsFloat(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(float));
		}

		public static bool IsDateTime(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(DateTime));
		}

		public static bool IsDecimal(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Decimal));
		}

		public static bool IsByteArray(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Byte[]));
		}

		public static bool IsByte(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Byte));
		}

		public static bool IsChar(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(char));
		}

		public static bool IsEnum(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.IsEnum;
		}

		public static bool IsStream(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Stream));
		}

		public static bool IsGuid(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Guid));
		}

		public static bool IsColor(this Type type)
		{
			if(Checker.IsNull(type)) { return false; }
			return type.Equals(typeof(Color));
		}
		#endregion

		#region T[]
		public static ArrayList ToArrayList<T>(this T[] cols)
		{
			return Formator.ColsToArrayList(cols);
		}

		public static Array ToArray<T>(this T[] cols)
		{
			return Formator.ColsToArray<T>(cols);
		}

		public static List<T> ToList<T>(this T[] cols)
		{
			return Formator.ColsToList<T>(cols);
		}

		public static void Swap<T>(this T[] cols, int x, int y)
		{
			if(Checker.IsNull(cols)) { return; }
			int len = cols.Length;
			if(0 <= x && x < len && 0 <= y && y < len && x != y) {
				T tmp = cols[x];
				cols[x] = cols[y];
				cols[y] = tmp;
			}
		}

		public static T[] Paging<T>(this T[] instances, int pageSize, int pageIndex)
		{
			return Formator.Paging<T>(instances, pageSize, pageIndex);
		}
		#endregion

		#region Array
		public static T[] ToCollection<T>(this Array array)
		{
			return Formator.ArrayToCols<T>(array);
		}
		#endregion

		#region XmlNode
		public static string Attribute(this XmlNode node, string name)
		{
			return Attribute(node, name, string.Empty);
		}
		public static string Attribute(
			this XmlNode node, string name, string airBag)
		{
			string value = AirBagXmlAttr(node, name, string.Empty);
			return value;
		}

		public static string AttributeToString(this XmlNode node, string name)
		{
			return AttributeToString(node, name, string.Empty);
		}
		public static string AttributeToString(
			this XmlNode node, string name, string airBag)
		{
			string value = AirBagXmlAttr(node, name, string.Empty);
			return value;
		}

		public static int AttributeToInt(this XmlNode node, string name)
		{
			return AttributeToInt(node, name, 0);
		}
		public static int AttributeToInt(
			this XmlNode node, string name, int airBag)
		{
			string value = AirBagXmlAttr(node, name);
			return value.AirBagToInt(airBag);
		}

		public static bool AttributeToBoolean(this XmlNode node, string name)
		{
			return AttributeToBoolean(node, name, false);
		}
		public static bool AttributeToBoolean(
			this XmlNode node, string name, bool airBag)
		{
			string value = AirBagXmlAttr(node, name);
			return value.AirBagToBoolean(airBag);
		}

		public static string AirBagXmlAttr(this XmlNode node, string name)
		{
			return Extender.AirBagXmlAttr(node, name, string.Empty);
		}

		public static string AirBagXmlAttr(this XmlNode node, string name, string airBag)
		{
			return Formator.AirBagXmlAttr(node, name, airBag);
		}

		public static string ChildInnerText(this XmlNode node, string tag)
		{
			return Extender.ChildInnerText(node, tag, string.Empty);
		}

		public static string ChildInnerText(this XmlNode node, string tag, string airBag)
		{
			return Formator.ChildInnerText(node, tag, airBag);
		}

		public static int ChildInnerInt(this XmlNode node, string tag)
		{
			return Extender.ChildInnerInt(node, tag, 0);
		}

		public static int ChildInnerInt(this XmlNode node, string tag, int airBag)
		{
			return Formator.ChildInnerText(node, tag).ToInt(airBag);
		}

		public static decimal ChildInnerDecimal(this XmlNode node, string tag)
		{
			return Extender.ChildInnerDecimal(node, tag, 0);
		}

		public static decimal ChildInnerDecimal(this XmlNode node, string tag, Decimal airBag)
		{
			return Formator.ChildInnerText(node, tag).ToDecimal(airBag);
		}
		public static long ChildInnerLong(this XmlNode node, string tag)
		{
			return Extender.ChildInnerLong(node, tag, 0);
		}

		public static long ChildInnerLong(this XmlNode node, string tag, long airBag)
		{
			return Formator.ChildInnerText(node, tag).ToLong(airBag);
		}
		public static float ChildInnerFloat(this XmlNode node, string tag)
		{
			return Extender.ChildInnerFloat(node, tag, 0);
		}

		public static float ChildInnerFloat(this XmlNode node, string tag, float airBag)
		{
			return Formator.ChildInnerText(node, tag).ToFloat(airBag);
		}
		public static bool ChildInnerBoolean(this XmlNode node, string tag)
		{
			return Extender.ChildInnerBoolean(node, tag, false);
		}

		public static bool ChildInnerBoolean(this XmlNode node, string tag, bool airBag)
		{
			return Formator.ChildInnerText(node, tag).ToBoolean(airBag);
		}
		public static DateTime ChildInnerDateTime(this XmlNode node, string tag)
		{
			return Extender.ChildInnerDateTime(node, tag, DateTime.MinValue);
		}

		public static DateTime ChildInnerDateTime(this XmlNode node, string tag, DateTime airBag)
		{
			return Formator.ChildInnerText(node, tag).ToDateTime(airBag);
		}
		public static T ChildInnerEnum<T>(this XmlNode node, string tag)
		{
			return Formator.ChildInnerText(node, tag).ToEnum<T>();
		}

		public static T ChildInnerEnum<T>(this XmlNode node, string tag, T airBag)
		{
			return Formator.ChildInnerText(node, tag).ToEnum<T>(airBag);
		}
		#endregion

		#region XmlDocument, XDocument
		public static XmlDocument ToXmlDocument(this XDocument xDocument)
		{
			var xmlDocument = new XmlDocument();
			using(var xmlReader = xDocument.CreateReader()) {
				xmlDocument.Load(xmlReader);
			}
			return xmlDocument;
		}

		public static XDocument ToXDocument(this XmlDocument xmlDocument)
		{
			using(var nodeReader = new XmlNodeReader(xmlDocument)) {
				nodeReader.MoveToContent();
				return XDocument.Load(nodeReader);
			}
		}
		#endregion

		#region Object
		public static bool IsDerived<B>(this object obj)
		{
			if(null == obj) { return false; }
			return Reflector.IsDerived<B>(obj.GetType());
		}

		public static bool IsDerived(this object obj, Type bType)
		{
			if(null == obj) { return false; }
			return Reflector.IsDerived(obj.GetType(), bType);
		}

		public static List<Any> ToAny(this Exception ex, params Any[] anys)
		{
			if(Checker.IsNull(ex)) {
				return Checker.IsNull(anys) ? new List<Any>() : new List<Any>(anys);
			}
			List<Any> list = new List<Any>();
			if(!Checker.IsNull(anys)) { list.AddRange(anys); }
			if(null != ex) {
				list.Add(new Any("source", ex.Source));
				list.Add(new Any("Message", ex.Message));
				list.Add(new Any("TargetSite", ex.TargetSite));
				list.Add(new Any("HelpLink", ex.HelpLink));
				list.Add(new Any("Stack Trace", "..." + Environment.NewLine + ex.StackTrace));
			}
			return list;
		}

		public static List<Any> ToAny(this object obj, params Any[] anys)
		{
			if(null == obj) {
				return Checker.IsNull(anys) ? new List<Any>() : new List<Any>(anys);
			}
			return obj.ToAny(true, anys);
		}

		public static List<Any> ToAny(this object obj, bool needCanWrite, params Any[] anys)
		{
			if(null == obj) { return new List<Any>(); }
			return Reflector.ToAny(obj, needCanWrite, anys);
		}

		public static string Serialize(this object obj)
		{
			return Serializer.ToXml(obj);
		}

		public static string SerializeJson(this object obj, params string[] properties)
		{
			return Serializer.ToJson(obj, properties);
		}

		public static string ToJson<T>(this T obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
		public static string ToJson<T>(this List<T> objs)
		{
			return JsonConvert.SerializeObject(objs);
		}
		#endregion

		#region Object[]
		public static Any[] ToAnys(
			this object[] objs,
			string namePropertyName,
			string valuePropertyName)
		{
			if(Checker.IsNull(objs)) { return new Any[0]; }

			List<Any> list = new List<Any>();
			foreach(object obj in objs) {
				list.Add(new Any(
					Reflector.GetValue(namePropertyName, obj).ToString(),
					Reflector.GetValue(valuePropertyName, obj)
				));
			}

			return list.ToArray();
		}

		public static object FindFirst(this object[] objs, params Any[] anys)
		{
			return Formator.FilterMainFirst(objs, anys);
		}

		public static object[] Find(this object[] objs, params Any[] anys)
		{
			return Formator.FilterMain(objs, anys);
		}

		public static object Find(this object[] objs, object obj)
		{
			if(Checker.IsNull(objs)) { return null; }
			if(obj == null) { return null; }
			foreach(var item in objs) {
				if(item == null) { continue; }
				if(item.Equals(objs)) { return item; }
			}
			return null;
		}

		#endregion

		#region StringBuilder
		/// <summary>
		/// Remove last i characters.
		/// </summary>
		/// <param name="i">how much characters to remove</param>
		/// <returns></returns>
		public static StringBuilder RemoveLast(this StringBuilder sb, int i)
		{
			if(Checker.IsNull(sb)) { return sb; }
			int len = sb.Length;
			if(len == 0) { return sb; }
			if(len < i) { return sb.Remove(0, len); }
			return sb.Remove(len - i, i);
		}

		/// <summary>
		/// Remove first i characters.
		/// </summary>
		/// <param name="i">how much characters to remove</param>
		/// <returns></returns>
		public static StringBuilder RemoveFirst(this StringBuilder sb, int i)
		{
			if(Checker.IsNull(sb)) { return sb; }
			int len = sb.Length;
			if(len == 0) { return sb; }
			return sb.Remove(0, i > len ? len : i);
		}

		/// <summary>
		/// Remove first 1 character.
		/// </summary>
		/// <returns></returns>
		public static StringBuilder RemoveFirst(this StringBuilder sb)
		{
			if(Checker.IsNull(sb)) { return sb; }
			return sb.RemoveFirst(1);
		}

		/// <summary>
		/// Prepend str to the begin of StringBuilder, aka Insert(0,str)
		/// </summary>
		/// <returns></returns>
		public static StringBuilder Prepend(this StringBuilder sb, string str)
		{
			if(null == sb) { sb = new StringBuilder(); }
			return sb.Insert(0, str);
		}

		/// <summary>
		/// convert this instance sub string to System.String
		/// </summary>
		/// <param name="startIndex">where the sub string index start</param>
		/// <returns></returns>
		public static string ToString(this StringBuilder sb, int startIndex)
		{
			if(null == sb) { return string.Empty; }
			int len = sb.Length;
			return startIndex >= len ? String.Empty : sb.ToString(startIndex, len - startIndex);
		}

		public static StringBuilder AppendLineFormat(
			this StringBuilder sb,
			string pattern,
			params string[] values)
		{
			sb.AppendFormat(pattern, values).AppendLine();
			return sb;
		}
		#endregion

		#region IPAddress
		/// <summary>
		/// Check whether the IPAddress within the range of the start/end.
		/// </summary>
		/// <param name="ipAddress">IPAddress object</param>
		/// <param name="start">Start IP long</param>
		/// <param name="end">End IP long</param>
		/// <returns>This IPAddress in the start/end range.</returns>
		public static bool InRange(this IPAddress ipAddress, long start, long end)
		{
			// Check
			if(Checker.IsNull(ipAddress)) { return false; }
			if(-1 == start || -1 == end) { return false; }

			// Compare
			long ip = Formator.IPAddressToLong(ipAddress);
			return ip.Between(start, end);
		}

		/// <summary>
		/// Check whether the IPAddress within the range of the start/end.
		/// </summary>
		/// <param name="ipAddress">IPAddress object</param>
		/// <param name="start">Start IP string</param>
		/// <param name="end">End IP string</param>
		/// <returns>This IPAddress in the start/end range.</returns>
		public static bool InRange(this IPAddress ipAddress, string start, string end)
		{
			if(Checker.IsNull(ipAddress)) { return false; }
			long startL = Formator.IPToLong(start);
			long endL = Formator.IPToLong(end);
			return ipAddress.InRange(startL, endL);
		}

		/// <summary>
		/// Check whether the IPAddress within the range of the start/end.
		/// </summary>
		/// <param name="ipAddress">IPAddress object</param>
		/// <param name="start">Start IPAddress</param>
		/// <param name="end">End IPAddress</param>
		/// <returns>This IPAddress in the start/end range.</returns>
		public static bool InRange(this IPAddress ipAddress, IPAddress start, IPAddress end)
		{
			if(Checker.IsNull(ipAddress)) { return false; }
			long startL = Formator.IPAddressToLong(start);
			long endL = Formator.IPAddressToLong(end);
			return ipAddress.InRange(startL, endL);
		}

		/// <summary>
		/// Check whether the IPAddress is one of private ip.
		/// </summary>
		/// <param name="ipAddress">IPAddress object</param>
		/// <returns>This IPAddress is one of private ip.</returns>
		public static bool IsPrivate(this IPAddress ipAddress)
		{
			if(Checker.IsNull(ipAddress)) { return false; }
			long ip = Formator.IPAddressToLong(ipAddress);
			return Checker.IsPrivateIP(ip);
		}
		#endregion

		#region FileStream
		public static string ToMD5(this FileStream fileStream)
		{
			if(Checker.IsNull(fileStream)) { return string.Empty; }
			byte[] hash = HashAlgorithm.Create(Formator.MD5_ENCODE).ComputeHash(fileStream);
			return Convert.ToBase64String(hash);
		}
		public static string ToSHA(this FileStream fileStream)
		{
			if(Checker.IsNull(fileStream)) { return string.Empty; }
			byte[] hash = HashAlgorithm.Create(Formator.SHA512_ENCODE).ComputeHash(fileStream);
			return Convert.ToBase64String(hash);
		}
		#endregion

		#region IDictionary
		/// <summary>
		/// if contains key, override the value. otherwise, add to the collection.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool SafeAdd<TKey, TValue>(
			this IDictionary<TKey, TValue> dict,
			TKey key, TValue value)
		{
			if(null == dict) { return false; }
			lock(_Lock) {
				bool containsKey = dict.ContainsKey(key);
				if(containsKey) {
					dict[key] = value;
				} else {
					dict.Add(key, value);
				}
				return containsKey;
			}
		}

		/// <summary>
		/// if contains key, remove this item. otherwise return false.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool SafeRemove<TKey, TValue>(
			this IDictionary<TKey, TValue> dict, TKey key)
		{
			if(null == dict) { return false; }
			if(null == key) { return false; }
			lock(_Lock) {
				bool containsKey = dict.ContainsKey(key);
				return containsKey ? dict.Remove(key) : false;
			}
		}


		/// <summary>
		/// Clone the Dictionary
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict"></param>
		/// <returns></returns>
		public static Dictionary<TKey, TValue> Clone<TKey, TValue>(
			this Dictionary<TKey, TValue> dict)
		{
			Dictionary<TKey, TValue> clone = new Dictionary<TKey, TValue>();
			if(!Checker.IsNull(dict)) {
				foreach(var key in dict.Keys) {
					clone.Add(key, dict[key]);
				}
			}
			return clone;
		}

		public static T[] ToArray<T>(this Dictionary<string, T>.ValueCollection values)
		{
			List<T> list = new List<T>();
			if(!Checker.IsNull(values)) {
				foreach(T value in values) {
					list.Add(value);
				}
			}
			return list.ToArray();
		}

		public static T[] ToArray<T>(this SortedList<string, T> sortedList)
		{
			List<T> list = new List<T>();
			if(null != sortedList.Values && sortedList.Values.Count > 0) {
				foreach(T value in sortedList.Values) {
					list.Add(value);
				}
			}
			return list.ToArray();
		}
		#endregion

		#region Color

		/// <summary>
		/// short cut for System.Drawing.ColorTranslator.ToHtml,
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string ToHtml(this Color color)
		{
			if(Checker.IsNull(color)) { color = Color.White; }
			return ColorTranslator.ToHtml(color);
		}

		/// <summary>
		/// To hexadecimal string, without lead hash.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string ToHexString(this Color color)
		{
			if(Checker.IsNull(color)) { color = Color.White; }
			return color.ToHtml().TrimStart('#');
		}
		#endregion

		#region Bind
		public static void Bind(this object obj, NameValueCollection nvc)
		{
			Reflector.Bind(obj, nvc);
		}
		public static void Bind(this object obj, params Any[] anys)
		{
			Reflector.Bind(obj, anys);
		}
		#endregion

		#region Generic
		public static bool In(this Enum val, params Enum[] vals)
		{
			return Array.IndexOf(vals, val) != -1;
		}

		public static bool EnumIn<T>(this T val, params T[] vals)
			where T : struct
		{
			if(Checker.IsNull(vals)) { return false; }

			Type type = typeof(T);
			if(!type.IsEnum) {
				throw new ArgumentException(
					"Only Enum type allowed in Extender.EnumIn method."
				);
			}

			foreach(T x in vals) {
				if(val.Equals(x)) { return true; }
			}

			return false;
		}
		#endregion

		#region Random
		public static List<T> Randomize<T>(this List<T> list)
		{
			using(var rng = new RNGCryptoServiceProvider()) {
				for(int i = list.Count; i > 0; i--) {
					int j = rng.Next(i);
					T k = list[j];
					list.RemoveAt(j);
					list.Add(k);
				}
				return list;
			}
		}
		public static Randomizer<T> GetRandomizer<T>(this List<T> list)
		{
			return new Randomizer<T>(list);
		}
		#endregion

		#region Image
		public static string ToBase64(this System.Drawing.Image image)
		{
			return Formator.ToBase64(image);
		}
		#endregion

		#region Currency
		public static string ToMoney(this double money)
		{
			return string.Format("{0:$#,##0}", money);
		}
		public static string ToMoney(this int money)
		{
			return string.Format("{0:$#,##0}", money);
		}
		public static string ToMoney(this Int64 money)
		{
			return string.Format("{0:$#,##0}", money);
		}
		public static string ToThousandPoint(this double money)
		{
			return string.Format("{0:#,##0}", money);
		}
		public static string ToThousandPoint(this int money)
		{
			return string.Format("{0:#,##0}", money);
		}
		public static string ToThousandPoint(this Int64 money)
		{
			return string.Format("{0:#,##0}", money);
		}
		#endregion

		#region IEnumerable
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || enumerable.Count() == 0;
		}
		#endregion

		#region With
		public static T With<T>(this T item, Action<T> action)
		{
			action(item);
			return item;
		}
		#endregion

		#region ToXxx
		public static int ToInt(this string value)
		{
			return Formator.AirBagToInt(value);
		}

		public static int ToInt(this string value, int airBag)
		{
			return Formator.AirBagToInt(value, airBag);
		}

		public static decimal ToDecimal(this string value)
		{
			return Formator.AirBagToDecimal(value);
		}

		public static decimal ToDecimal(this string value, decimal airBag)
		{
			return Formator.AirBagToDecimal(value, airBag);
		}

		public static long ToLong(this string value)
		{
			return Formator.AirBagToLong(value);
		}

		public static long ToLong(this string value, long airBag)
		{
			return Formator.AirBagToLong(value, airBag);
		}

		public static float ToFloat(this string value)
		{
			return Formator.AirBagToFloat(value);
		}

		public static float ToFloat(this string value, float airBag)
		{
			return Formator.AirBagToFloat(value, airBag);
		}

		public static bool ToBoolean(this string value)
		{
			return Formator.AirBagToBoolean(value);
		}

		public static bool ToBoolean(this string value, bool airBag)
		{
			return Formator.AirBagToBoolean(value, airBag);
		}

		public static string ToBooleanString(this string value)
		{
			return Formator.AirBagToBoolean(value)
				? Constants.StringBoolean.True
				: Constants.StringBoolean.False;
		}

		public static DateTime ToDateTime(this string value)
		{
			return Formator.AirBagToDateTime(value);
		}

		public static DateTime ToDateTime(this string value, DateTime airBag)
		{
			return Formator.AirBagToDateTime(value, airBag);
		}

		public static T ToEnum<T>(this string value)
		{
			return Formator.AirBagToEnum<T>(value);
		}

		public static T ToEnum<T>(this string value, T airBag)
		{
			return Formator.AirBagToEnum<T>(value, airBag);
		}

		public static List<T> ToEnums<T>(this string value, string separator)
		{
			List<T> list = new List<T>();
			if(value.IsNullOrEmpty()) { return list; }
			foreach(var one in value.SplitWith(separator)) {
				list.Add(Formator.AirBagToEnum<T>(one));
			}
			return list;
		}

		public static List<T> ToEnums<T>(this string value, string separator, T airBag)
		{
			List<T> list = new List<T>();
			if(value.IsNullOrEmpty()) { return list; }
			foreach(var one in value.SplitWith(separator)) {
				list.Add(Formator.AirBagToEnum<T>(one, airBag));
			}
			return list;
		}
		#endregion

		#region GroupNameValue
		public static List<GroupValues> ToGroupValues(
			this List<GroupNameValue> list)
		{
			List<string> groups = new List<string>();
			list.ForEach(x => {
				if(!groups.Contains(x.Group)) { groups.Add(x.Group); }
			});

			List<GroupValues> gvs = new List<GroupValues>();
			groups.ForEach(x => {
				GroupValues gv = new GroupValues() {
					Group = x
				};
				list.ForEach(y => {
					if(x == y.Group) { gv.Values.Add(y.Name, y.Value); }
				});
				gvs.Add(gv);
			});

			return gvs;
		}
		#endregion

		#region RNGCryptoServiceProvider
		public static int Next(this RNGCryptoServiceProvider rng, int max)
		{
			return Next(rng, 0, max);
		}
		public static int Next(this RNGCryptoServiceProvider rng, int min, int max)
		{
			if(min > max) { throw new ArgumentOutOfRangeException("min"); }
			if(min == max) { return min; }
			Int64 diff = max - min;
			byte[] _uint32Buffer = new byte[4];

			while(true) {
				rng.GetBytes(_uint32Buffer);
				UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);
				Int64 maxValue = (1 + (Int64)UInt32.MaxValue);
				Int64 remainder = maxValue % diff;
				if(rand < maxValue - remainder) {
					return (Int32)(min + (rand % diff));
				}
			}
		}
		#endregion

		#region SecureString
		public static SecureString ToSecureString(this string str)
		{
			SecureString secureStr = new SecureString();
			if(!str.IsNullOrEmpty()) {
				str.ToList().ForEach(x =>
					secureStr.AppendChar(x)
				);
			}
			return secureStr;
		}

		// non secure
		//public static string ToString(this SecureString secureStr)
		//{
		//	if(null == secureStr) {
		//		throw new ArgumentNullException("secureStr");
		//	}

		//	IntPtr bstr = Marshal.SecureStringToBSTR(secureStr);
		//	try {
		//		return Marshal.PtrToStringBSTR(bstr);
		//	} finally {
		//		Marshal.FreeBSTR(bstr);
		//	}
		//}
		#endregion

		#region IQueryable<T>
		public static List<Any> ToAnys<T>(
			this IQueryable<T> all, 
			Func<T, string> toName, 
			Func<T, object> toValue)
		{
			var list = new List<Any>();
			if(null == all || null == toName || null == toValue) { return list; }
			foreach(T one in all) {
				list.Add(new Any(toName(one), toValue(one)));
			}
			return list;
		}
		#endregion
	}
}

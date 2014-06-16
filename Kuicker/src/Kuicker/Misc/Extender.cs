// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Drawing;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Security;
using Newtonsoft.Json;

namespace Kuicker
{
	public static class Extender
	{
		private static object _Lock = new object();

		#region Array
		public static bool IsNullOrEmpty(Array array)
		{
			return null == array || array.Length == 0;
		}
		#endregion

		#region ArrayList
		public static bool IsNullOrEmpty(this ArrayList arrayList)
		{
			return null == arrayList || arrayList.Count == 0;
		}
		#endregion

		#region string
		public static bool IsNullOrEmpty(this string str)
		{
			return null == str || string.IsNullOrEmpty(str);
		}

		public static string Left(this string input, int length)
		{
			if(input.IsNullOrEmpty()) { return string.Empty; }
			return input.Substring(0, length);
		}

		public static string Right(this string input, int length)
		{
			if(input.IsNullOrEmpty()) { return string.Empty; }
			if(length >= input.Length) { return input; }
			return input.Substring(input.Length - length, length);
		}

		public static string[] Split(
			this string input, string pattern, RegexOptions options)
		{
			if(input.IsNullOrEmpty()) { return new string[0]; }
			string[] parts = Regex.Split(input, pattern, options);
			List<string> list = new List<string>();
			foreach(string x in parts) {
				list.Add(x.Trim());
			}
			return list.ToArray();
		}

		public static string[] Split(this string input, params string[] symbols)
		{
			if(input.IsNullOrEmpty()) { return new string[0]; }
			string[] parts = input.Split(symbols, StringSplitOptions.None);
			List<string> list = new List<string>();
			foreach(string x in parts) {
				list.Add(x.Trim());
			}
			return list.ToArray();
		}

		public static byte[] ToBytes(this string input)
		{
			if(string.IsNullOrEmpty(input)) { return new byte[0]; }
			return UnicodeEncoding.UTF8.GetBytes(input);
		}

		public static Stream ToStream(this string input)
		{
			if(String.IsNullOrEmpty(input)) { return null; }
			return ToStream(ToBytes(input));
		}

		public static string ToBase64(this string input)
		{
			if(input.IsNullOrEmpty()) { return string.Empty; }
			return Convert.ToBase64String(ToBytes(input));
		}

		public static string FromBase64(this string input)
		{
			if(input.IsNullOrEmpty()) { return string.Empty; }
			return ToString(Convert.FromBase64String(input));
		}

		public static string Repeat(this string seed, int times)
		{
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < times; i++) {
				sb.Append(seed);
			}
			return sb.ToString();
		}

		public static string ToMD5(this string str)
		{
			return str.Encode("MD5");
		}

		public static string ToSHA(this string str)
		{
			return str.Encode("SHA512");
		}
		public static string Encode(this string str, string encodeBy)
		{
			if(str.IsNullOrEmpty()) { return string.Empty; }
			if(!encodeBy.In("MD5", "SHA512")) {
				throw new ArgumentException("encodeBy");
			}
			string rtn = "";
			byte[] buffer = Encoding.Unicode.GetBytes(str);
			byte[] hash = HashAlgorithm.Create(encodeBy).ComputeHash(buffer);
			rtn = Convert.ToBase64String(hash);
			return rtn;
		}

		public static byte[] HexToBytes(this string hex)
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

		public static string AirBag(this string value)
		{
			return AirBag(value, string.Empty);
		}

		public static string AirBag(this string value, string airBag)
		{
			if(value.IsNullOrEmpty()) {
				if(airBag.IsNullOrEmpty()) {
					return string.Empty;
				} else {
					return airBag;
				}
			} else {
				return value;
			}
		}

		public static int ToInt(this string value)
		{
			return value.ToInt(default(int));
		}

		public static int ToInt(this string value, int airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return value.IsNullOrEmpty()
				? airBag
				: value.IsInteger() ? Convert.ToInt32(value, provider) : airBag;
		}

		public static decimal ToDecimal(this string value)
		{
			return value.ToDecimal(default(decimal));
		}

		public static decimal ToDecimal(this string value, decimal airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return value.IsNullOrEmpty()
				? airBag
				: value.IsDecimal() ? decimal.Parse(value, provider) : airBag;
		}

		public static long ToLong(this string value)
		{
			return value.ToLong(default(long));
		}

		public static long ToLong(this string value, long airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return value.IsNullOrEmpty()
				? airBag
				: value.IsLong() ? long.Parse(value, provider) : airBag;
		}

		public static float ToFloat(this string value)
		{
			return value.ToFloat(default(float));
		}

		public static float ToFloat(this string value, float airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			return value.IsNullOrEmpty()
				? airBag
				: value.IsFloat() ? float.Parse(value, provider) : airBag;
		}

		public static bool ToBoolean(this string value)
		{
			return value.ToBoolean(default(bool));
		}

		public static bool ToBoolean(this string value, bool airBag)
		{
			return value.IsNullOrEmpty()
				? airBag
				: value.ToLower().In("1", "t", "y", "true", "yes", "on");
		}

		public static DateTime ToDateTime(this string value)
		{
			return value.ToDateTime(default(DateTime));
		}

		public static DateTime ToDateTime(this string value, DateTime airBag)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			DateTimeFormatInfo provider = info.DateTimeFormat;

			try {
				return value.IsNullOrEmpty()
					? airBag
					: Convert.ToDateTime(value, provider);
			} catch {
				return airBag;
			}
		}

		public static T ToEnum<T>(this string value)
		{
			return value.ToEnum<T>(default(T));
		}

		public static T ToEnum<T>(this string value, T airBag)
		{
			try {
				return (T)Enum.Parse(typeof(T), value, true);
			} catch {
				if(typeof(T).IsEnum) {
					//EnumReference ef = Kernel.EnumCache[typeof(T)];
					//foreach(var item in ef.Items) {
					//	if(item.Title == value || item.Value == value) {
					//		return (T)item.Value;
					//	}
					//}
					return default(T);
				}
				return airBag;
			}
		}

		public static bool In(this string value, params string[] values)
		{
			return value.In(StringComparison.OrdinalIgnoreCase, value);
		}

		public static bool In(
			this string value,
			StringComparison comparison,
			params string[] values)
		{
			if(values.IsNullOrEmpty()) { return false; }

			foreach(string x in values) {
				if(null == x && null == value) { return true; }
				if(null == x || null == value) { return false; }
				if(value.Trim().Equals(x.Trim(), comparison)) {
					return true;
				}
			}
			return false;
		}

		public static bool StartWith(this string value, params string[] values)
		{
			return value.StartWith(StringComparison.OrdinalIgnoreCase, value);
		}

		public static bool StartWith(
			this string value,
			StringComparison comparison,
			params string[] values)
		{
			if(values.IsNullOrEmpty()) { return false; }

			foreach(string x in values) {
				if(null == x && null == value) { return true; }
				if(null == x || null == value) { return false; }
				if(value.StartsWith(x.Trim(), comparison)) { return true; }
			}
			return false;
		}

		public static bool EndWith(this string value, params string[] values)
		{
			return value.EndWith(StringComparison.OrdinalIgnoreCase, value);
		}

		public static bool EndWith(
			this string value,
			StringComparison comparison,
			params string[] values)
		{
			if(values.IsNullOrEmpty()) { return false; }

			foreach(string x in values) {
				if(null == x && null == value) { return true; }
				if(null == x || null == value) { return false; }
				if(value.EndsWith(x.Trim(), comparison)) { return true; }
			}
			return false;
		}

		public static bool IsMatch(this string value, string pattern)
		{
			return value.IsMatch(pattern, false);
		}

		public static bool IsMatch(
			this string value, string pattern, bool ignoreCase)
		{
			return Regex.IsMatch(
				value,
				pattern,
				ignoreCase
					? RegexOptions.Compiled | RegexOptions.IgnoreCase
					: RegexOptions.Compiled
				);
		}

		public static string TrimStart(
			this string value,
			params string[] trimStrings)
		{
			if(value.IsNullOrEmpty()) { return value; }
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
			if(value.IsNullOrEmpty()) { return value; }
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

		public static string UpperToPascalCasing(this string txt)
		{
			if(txt.IsNullOrEmpty()) { return string.Empty; }

			string[] words = txt.Split(Constants.Symbol.Char.UnderScore);
			for(int i = 0; i < words.Length; i++) {
				string word = words[i];
				if(word.Length < 3) { continue; }
				if(word != word.ToUpper()) { continue; }
				if(!word.IsNullOrEmpty()) {
					char firstLetter = char.ToUpper(word[0]);
					words[i] = firstLetter + word.Substring(1).ToLower();
				}
			}

			return string.Join(string.Empty, words);
		}

		public static string PascalToUpperCasing(this string txt)
		{
			if(txt.IsNullOrEmpty()) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			bool preIsUpper = false;
			char preChar = '1';
			foreach(char letter in txt.ToCharArray()) {
				if(
					letter == Constants.Symbol.Char.UnderScore
					&& preChar == Constants.Symbol.Char.UnderScore) {
					continue;
				}

				if(char.IsUpper(letter)) {
					if(!preIsUpper) {
						preIsUpper = true;
						if(
							sb.Length > 0
							&&
							preChar != Constants.Symbol.Char.UnderScore) {
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

		public static bool IsInteger(this string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			int result = default(int);
			return int.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsDecimal(this string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			decimal result = default(decimal);
			return decimal.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsLong(this string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			long result = default(long);
			return long.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsFloat(this string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			float result = default(float);
			return float.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsDouble(this string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			double result = default(double);
			return double.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsByte(this string value)
		{
			byte result = default(byte);
			return byte.TryParse(value, out result);
		}

		public static bool IsChar(this string value)
		{
			char result = default(char);
			return char.TryParse(value, out result);
		}

		public static bool IsDateTime(this string value)
		{
			DateTime result;
			return DateTime.TryParse(value, out result);
		}

		public static bool IsTaiwanIDNumber(this string value)
		{
			if(!Regex.IsMatch(
				value,
				@"^[A-Z][12]\d{8}$",
				RegexOptions.IgnoreCase | RegexOptions.Compiled)) {
				return false;
			}

			char[] id = value.ToUpper().ToCharArray();
			int c;
			Dictionary<char, int> magic = new Dictionary<char, int>() 
			{
				{'A',  1}, {'B', 10}, {'C', 19}, {'D', 28}, {'E', 37}, {'F', 46}, 
				{'G', 55}, {'H', 64}, {'I', 39}, {'J', 73}, {'K', 82}, {'L',  2},
				{'M', 11}, {'N', 20}, {'O', 48}, {'P', 29}, {'Q', 38}, {'R', 47}, 
				{'S', 56}, {'T', 65}, {'U', 74}, {'V', 83}, {'W', 21}, {'X',  3},
				{'Y', 12}, {'Z', 30}
			};
			if(!magic.TryGetValue(id[0], out c)) { return false; }

			int[] b = { 1, 8, 7, 6, 5, 4, 3, 2, 1 };
			for(var i = b.Length - 1; i > 0; i--) {
				c += int.Parse(id[i].ToString()) * b[i];
			}

			return (10 - c % 10) % 10 == int.Parse(id[9].ToString());
		}

		public static bool IsEmail(this string value)
		{
			Regex regex = new Regex(Constants.Pattern.Email, RegexOptions.Compiled);
			return regex.IsMatch(value);
		}

		public static bool IsIP(this string value)
		{
			return Regex.IsMatch(value, Constants.Pattern.Ip, RegexOptions.Compiled);
		}

		public static bool IsLoopBackIP(this string value)
		{
			if(!IsIP(value)) { return false; }
			return value.StartsWith("127.");
		}

		public static long IPToLong(this string ip)
		{
			// Checker
			if(!ip.IsIP()) { return -1; }

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

		public static bool IsPrivateIP(this string value)
		{
			return IsIP(value)
				? IsPrivateIP(value.IPToLong())
				: false;
		}

		public static bool IsNullIP(this string value)
		{
			return "0.0.0.0" == value || !IsIP(value);
		}

		public static bool IsAlphabet(this string value)
		{
			Regex regex = new Regex(
				Constants.Pattern.Alphabet, RegexOptions.Compiled
			);
			return regex.IsMatch(value);
		}

		public static bool IsNumeric(this string value)
		{
			Regex regex = new Regex(
				Constants.Pattern.Numeric, RegexOptions.Compiled
			);
			return regex.IsMatch(value);
		}

		public static bool IsAlphaNumeric(this string value)
		{
			Regex regex = new Regex(
				Constants.Pattern.AlphaNumeric, RegexOptions.Compiled
			);
			return regex.IsMatch(value);
		}

		public static bool IsUri(this string value)
		{
			Regex regex = new Regex(
				Constants.Pattern.Uri, RegexOptions.Compiled
			);
			return regex.IsMatch(value);
		}

		public static string DividePascalCasing(this string pascalCasing)
		{
			if(pascalCasing.IsNullOrEmpty()) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			bool preIsUpper = false;
			foreach(char letter in pascalCasing.ToCharArray()) {
				if(char.IsUpper(letter)) {
					if(!preIsUpper) {
						preIsUpper = true;
						if(sb.Length > 0) {
							sb.Append(Constants.Symbol.Char.Space);
						}
					}
					sb.Append(letter);
				} else {
					preIsUpper = false;
					sb.Append(letter);
				}
			}

			return sb.ToString();
		}

		public static string ToLowerAndCompact(this string str)
		{
			return null == str
				? string.Empty
				: str
					.Replace(Constants.Symbol.UnderScore, string.Empty)
					.ToLower()
					.Trim();
		}
		#endregion

		#region string[]
		public static bool IsNullOrEmpty(this string[] cols)
		{
			if(null == cols) { return true; }
			foreach(string x in cols) {
				if(!string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)) {
					return false;
				}
			}
			return true;
		}

		public static string Join(this string[] cols, string separator)
		{
			return cols.Join(separator, string.Empty);
		}

		public static string Join(
			this string[] cols,
			string separator,
			string quoter)
		{
			return cols.Join(separator, quoter, quoter);
		}

		public static string Join(
			this string[] cols,
			string separator,
			string quoter0,
			string quoter9)
		{
			if(cols.IsNullOrEmpty()) { return string.Empty; }
			if(separator.IsNullOrEmpty()) { separator = string.Empty; }
			if(quoter0.IsNullOrEmpty()) { quoter0 = string.Empty; }
			if(quoter9.IsNullOrEmpty()) { quoter9 = string.Empty; }

			StringBuilder sb = new StringBuilder();
			foreach(string x in cols) {
				sb.AppendFormat("{0}{1}{2}", quoter0, x, quoter9);
				sb.Append(separator);
			}

			return sb.ToString().TrimEnd(separator.ToCharArray());
		}
		#endregion

		#region byte[]
		public static bool IsNullOrEmpty(this byte[] input)
		{
			return null == input || input.Length == 0;
		}

		public static string ToString(this byte[] input)
		{
			if(input.IsNullOrEmpty()) { return string.Empty; }
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < input.Length; i++) {
				sb.Append(input[i].ToString("x2"));
			}
			return sb.ToString();
		}

		public static Stream ToStream(this byte[] input)
		{
			if(input.IsNullOrEmpty()) { return null; }
			return new MemoryStream(input);
		}

		public static string ToHex(this byte[] b)
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

		#region Stream
		public static bool IsNullOrEmpty(this Stream input)
		{
			return null == input;
		}

		public static string ToString(this Stream input)
		{
			if(null == input) { return null; }
			byte[] bs = input.ToBytes();
			string str = bs.ToString();
			return str;
		}

		public static byte[] ToBytes(this Stream input)
		{
			if(null == input) { return new byte[0]; }
			byte[] bytes = new byte[input.Length];
			input.Read(bytes, 0, bytes.Length);
			input.Seek(0, SeekOrigin.Begin);
			return bytes;
		}
		#endregion

		#region DateTime
		public static bool IsNullOrEmpty(this DateTime dateTime)
		{
			return dateTime == DateTime.MinValue;
		}


		public static string yyyy(this DateTime date)
		{
			return date.ToString("yyyy");
		}

		public static string yyyy_MM(this DateTime date)
		{
			return date.ToString("yyyy-MM");
		}

		public static string yyyyMM(this DateTime date)
		{
			return date.ToString("yyyyMM");
		}

		public static string yyyyMMdd(this DateTime date)
		{
			return date.ToString("yyyyMMdd");
		}

		public static string yyyy_MM_dd(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd");
		}

		public static string yyyyMMddHHmmss(this DateTime date)
		{
			return date.ToString("yyyyMMddHHmmss");
		}

		public static string yyyy_MM_dd_HH_mm_ss(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static string yyyyMMddHHmmssfff(this DateTime date)
		{
			return date.ToString("yyyyMMddHHmmssfff");
		}

		public static string yyyy_MM_dd_HH_mm_ss_fff(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
			return date.ToString("hhmm");
		}

		public static string hh_mm(this DateTime date)
		{
			return date.ToString("hh:mm");
		}

		public static string hhmmss(this DateTime date)
		{
			return date.ToString("hhmmss");
		}

		public static string hh_mm_ss(this DateTime date)
		{
			return date.ToString("hh:mm:ss");
		}

		public static DateTime AirBag(this DateTime dateTime)
		{
			return dateTime.AirBag(DateTime.MinValue);
		}

		public static DateTime AirBag(this DateTime dateTime, DateTime airBag)
		{
			return dateTime.IsNullOrEmpty() ? airBag : dateTime;
		}

		public static DateTime StartOfYear(this DateTime value)
		{
			return new DateTime(value.Year, 1, 1, 0, 0, 0, 0);
		}

		public static DateTime StartOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1, 0, 0, 0, 0);
		}

		public static DateTime StartOfWeek(this DateTime value)
		{
			return value.StartOfWeek(DayOfWeek.Sunday);
		}

		public static DateTime StartOfWeek(this DateTime value, DayOfWeek weekStart)
		{
			DateTime date = new DateTime(
				value.Year, value.Month, value.Day, 0, 0, 0
			);
			int difference = (int)weekStart - (int)date.DayOfWeek;
			if(difference > 0) { difference -= 6; }
			return date.AddDays(difference);
		}

		public static DateTime StartOfDay(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0);
		}

		public static DateTime StartOfHour(this DateTime value)
		{
			return new DateTime(
				value.Year, value.Month, value.Day, value.Hour, 0, 0
			);
		}

		public static DateTime StartOfMinute(this DateTime value)
		{
			return new DateTime(
				value.Year, value.Month, value.Day, value.Hour, value.Minute, 0
			);
		}

		public static DateTime EndOfYear(this DateTime value)
		{
			return new DateTime(
				value.Year,
				12,
				DateTime.DaysInMonth(value.Year, 12),
				23,
				59,
				59,
				999
			);
		}

		public static DateTime EndOfMonth(this DateTime value)
		{
			return new DateTime(
				value.Year,
				value.Month,
				DateTime.DaysInMonth(value.Year, value.Month),
				23,
				59,
				59,
				999
			);
		}

		public static DateTime EndOfWeek(this DateTime value)
		{
			return value.EndOfWeek(DayOfWeek.Sunday);
		}

		public static DateTime EndOfWeek(this DateTime value, DayOfWeek weekStart)
		{
			DateTime date = new DateTime(
				value.Year, value.Month, value.Day, 23, 59, 59
			);
			int difference = (int)weekStart - (int)date.DayOfWeek;
			if(difference > 0) { difference -= 6; }
			return date.AddDays(difference + 6);
		}

		public static DateTime EndOfDay(this DateTime value)
		{
			return new DateTime(
				value.Year, value.Month, value.Day, 23, 59, 59, 999
			);
		}

		public static DateTime EndOfHour(this DateTime value)
		{
			return new DateTime(
				value.Year, value.Month, value.Day, value.Hour, 59, 59, 999
			);
		}

		public static DateTime EndOfMinute(this DateTime value)
		{
			return new DateTime(
				value.Year, value.Month, value.Day, value.Hour, value.Minute, 59, 999
			);
		}

		public static double ToUnixTimestamp(this DateTime value)
		{
			return Math.Floor(
				(value - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
			);
		}
		#endregion

		#region ICollection
		//public static bool IsNullOrEmpty(this ICollection cols)
		//{
		//	return null == cols || cols.Count == 0;
		//}
		#endregion

		#region int
		public static int AirBag(this int value)
		{
			return value.AirBag(default(int));
		}

		public static int AirBag(this int value, int airBag)
		{
			return (value == -1) ? airBag : value;
		}

		public static string OrderList(this int no, int max)
		{
			return no.OrderList(max, ".");
		}

		public static string OrderList(this int no, int max, string suffix)
		{
			string s = no.ToString();
			int len = s.Length;
			string pattern = string.Concat("{0}{1}", suffix);
			return string.Format(
				pattern,
				len >= max
					? string.Empty
					: "0".Repeat(max - len),
				s
			);
		}

		public static bool IsEven(this int value)
		{
			return !value.IsOdd();
		}

		public static bool IsOdd(this int value)
		{
			return value >= 0 && ((value & 1) == 1);
		}

		public static int Ceiling(this int value, int criticalValue)
		{
			return value > criticalValue ? criticalValue : value;
		}

		public static int Floor(this int value, int criticalValue)
		{
			return value < criticalValue ? criticalValue : value;
		}

		public static int Within(
			this int value, int ceilingValue, int floorValue)
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
			return divisor >= 0 && dividend >= 0 && ((dividend % divisor) == 0);
		}

		public static int Remainder(this int dividend, int divisor)
		{
			if(divisor >= 0 && dividend >= 0) {
				return dividend % divisor;
			} else {
				return -1;
			}
		}

		public static int Quotient(this int dividend, int divisor)
		{
			return (dividend - Remainder(dividend, divisor)) / divisor;
		}
		#endregion

		#region int[]
		#endregion

		#region long
		public static long AirBag(this long value)
		{
			return (value == -1) ? 0 : value;
		}

		public static long AirBag(this long value, long airBag)
		{
			return (value == -1) ? airBag : value;
		}

		public static bool IsEven(this long value)
		{
			return value >= 0 && ((value & 1) == 0);
		}

		public static bool IsOdd(this long value)
		{
			return value >= 0 && ((value & 1) == 1);
		}

		public static bool Divisible(this long dividend, long divisor)
		{
			return divisor >= 0 && dividend >= 0 && ((dividend % divisor) == 0);
		}

		public static long Remainder(this long dividend, long divisor)
		{
			if(divisor >= 0 && dividend >= 0) {
				return dividend % divisor;
			} else {
				return -1;
			}
		}

		public static long Quotient(this long dividend, long divisor)
		{
			return (dividend - Remainder(dividend, divisor)) / divisor;
		}

		public static bool IsIP(this long ip)
		{
			return ip.Between(Constants.Ip.MinAsLong, Constants.Ip.MaxAsLong);
		}

		public static bool IsPrivateIP(this long ip)
		{
			if(!ip.IsIP()) {
				return false;
			}

			return
				ip.Between( // Class A
					Constants.Ip.Private.ClassA.StartAsLong,
					Constants.Ip.Private.ClassA.EndAsLong
				)
				||
				ip.Between( // Class B
					Constants.Ip.Private.ClassB.StartAsLong,
					Constants.Ip.Private.ClassB.EndAsLong
				)
				||
				ip.Between( // Class C
					Constants.Ip.Private.ClassC.StartAsLong,
					Constants.Ip.Private.ClassC.EndAsLong
				)
				||
				ip.Between( // Loopback
					Constants.Ip.Private.Loopback.StartAsLong,
					Constants.Ip.Private.Loopback.EndAsLong
				);
		}
		#endregion

		#region float
		public static float AirBag(this float value)
		{
			return (value == -1) ? 0 : value;
		}

		public static float AirBag(this float value, float airBag)
		{
			return (value == -1) ? airBag : value;
		}
		#endregion

		#region bool
		public static string If(this bool isTrue, string text)
		{
			return isTrue ? text : string.Empty;
		}
		#endregion

		#region bool[]
		#endregion

		#region ArrayList
		public static ArrayList Sort(this ArrayList al, string propertyName)
		{
			if(al.IsNullOrEmpty()) { return new ArrayList(); }

			int times = 10000;
			SortedList sort = new SortedList();
			foreach(object x in al) {
				times++;
				object key = Reflector.GetValue(propertyName, x) ?? x;
				key = key.ToString() + times.ToString();
				sort.Add(key, x);
			}

			var list = new ArrayList();
			for(int i = 0; i < sort.Count; i++) {
				object s = sort.GetByIndex(i);
				list.Add(s);
			}
			return list;
		}
		#endregion

		#region Hashtable
		public static bool IsNullOrEmpty(this Hashtable ht)
		{
			return null == ht || ht.Count == 0;
		}

		public static Hashtable AirBag(this Hashtable ht)
		{
			return ht.IsNullOrEmpty() ? new Hashtable() : ht;
		}
		#endregion

		#region NameValueCollection
		public static bool IsNullOrEmpty(this NameValueCollection nvs)
		{
			return null == nvs || nvs.Count == 0;
		}

		public static bool Exists(
			this NameValueCollection nvs, params string[] names)
		{
			if(nvs.IsNullOrEmpty()) { return false; }
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

		#region IList
		//public static bool IsNullOrEmpty(this IList list)
		//{
		//	return null == list || list.Count == 0;
		//}
		#endregion

		#region List<T>
		public static bool IsNullOrEmpty(this List<string> list)
		{
			return null == list || list.Count == 0;
		}

		public static List<Any> Remove(
			this List<Any> list, params string[] names)
		{
			if(list.IsNullOrEmpty()) { return new List<Any>(); }
			if(names.IsNullOrEmpty()) { return list; }

			var newList = list.Where(x => x.Name.In(names) == false);

			return newList.ToList();
		}

		public static List<T> Distinct<T>(this List<T> list)
		{
			IEnumerable<T> distincted = Enumerable.Distinct<T>(list);
			List<T> n = new List<T>(distincted);
			return n;
		}

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

		#region double
		public static string MillisecondToSecondString(this double milliseconds)
		{
			return string.Format("{0:0.000}", milliseconds / 1000f);
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
			if(null == type) { return false; }
			return type.Equals(typeof(string));
		}

		public static bool IsBoolean(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Boolean));
		}

		public static bool IsInteger(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(int));
		}

		public static bool IsInt64(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Int64));
		}

		public static bool IsShort(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Int16));
		}

		public static bool IsSingle(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Single));
		}

		public static bool IsDouble(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Double));
		}

		public static bool IsLong(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Int64));
		}

		public static bool IsFloat(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(float));
		}

		public static bool IsDateTime(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(DateTime));
		}

		public static bool IsDecimal(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Decimal));
		}

		public static bool IsByteArray(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Byte[]));
		}

		public static bool IsByte(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Byte));
		}

		public static bool IsChar(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(char));
		}

		public static bool IsEnum(this Type type)
		{
			if(null == type) { return false; }
			return type.IsEnum;
		}

		public static bool IsStream(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Stream));
		}

		public static bool IsGuid(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Guid));
		}

		public static bool IsColor(this Type type)
		{
			if(null == type) { return false; }
			return type.Equals(typeof(Color));
		}
		#endregion

		#region T
		public static bool IsNullOrEmpty<T>(T obj)
		{
			// null
			if(null == obj) { return true; }

			// others
			if(obj is string) { return (obj as string).IsNullOrEmpty(); }
			if(obj is DateTime) { return Convert.ToDateTime(obj).IsNullOrEmpty(); }

			// ICollection
			ICollection c = obj as ICollection;
			if(null != c) { return c.Count == 0; }
			// Hashtable
			Hashtable h = obj as Hashtable;
			if(null != h) { return h.IsNullOrEmpty(); }
			// ArrayList
			ArrayList a = obj as ArrayList;
			if(null != a) { return a.IsNullOrEmpty(); }
			// NameValueCollection
			NameValueCollection n = obj as NameValueCollection;
			if(null != n) { return n.IsNullOrEmpty(); }
			// XmlAttributeCollection
			XmlAttributeCollection x = obj as XmlAttributeCollection;
			if(null != x) { return x.Count == 0; }
			// XmlAttribute
			XmlAttribute xa = obj as XmlAttribute;
			if(null != xa) { return xa.IsNullOrEmpty(); }
			// XmlNodeList
			XmlNodeList xn = obj as XmlNodeList;
			if(null != xn) { return xn.Count == 0; }
			// object[]
			if(obj is object[]) { return (obj as object[]).IsNullOrEmpty(); }
			// object
			if(null == obj) { return true; }
			return false;
		}

		public static bool Between<T>(
			this T comparable, T comparableA, T comparableB)
			where T : IComparable
		{
			// two parameters
			// ----|XXXX|----
			return
				comparable.CompareTo(comparableA) >= 0
				&&
				comparable.CompareTo(comparableB) <= 0;
		}

		public static bool Besides<T>(
			this T comparable, T comparableA, T comparableB)
			where T : IComparable
		{
			// two parameters
			// XXXX|----|XXXX
			return
				comparable.CompareTo(comparableA) < 0
				||
				comparable.CompareTo(comparableB) > 0;
		}

		public static bool Greater<T>(
			this T comparable, T comparableA)
			where T : IComparable
		{
			// one parameter
			// ----|XXXXXXXXX
			return comparable.CompareTo(comparableA) > 0;
		}

		public static bool Smaller<T>(
			this T comparable, T comparableA)
			where T : IComparable
		{
			// one parameter
			// XXXXXXXXX|----
			return comparable.CompareTo(comparableA) < 0;
		}

		public static bool In<T>(this T value, params T[] values)
		{
			if(null == value) { return false; }
			foreach(T x in values) {
				if(value.Equals(x)) { return true; }
			}
			return false;
		}
		public static bool Exclude<T>(this T value, params T[] values)
		{
			if(null == value) { return false; }
			foreach(T x in values) {
				if(value.Equals(x)) { return false; }
			}
			return true;
		}
		#endregion

		#region T[]
		public static ArrayList ToArrayList<T>(this T[] cols)
		{
			return new ArrayList(cols);
		}

		public static Array ToArray<T>(this T[] cols)
		{
			return cols.ToArray();
		}

		public static List<T> ToList<T>(this T[] cols)
		{
			return new List<T>(cols);
		}

		public static void Swap<T>(this T[] cols, int x, int y)
		{
			if(cols.IsNullOrEmpty()) { return; }
			int len = cols.Length;
			if(0 <= x && x < len && 0 <= y && y < len && x != y) {
				T tmp = cols[x];
				cols[x] = cols[y];
				cols[y] = tmp;
			}
		}

		public static bool IsNullOrEmpty<T>(T[] objs)
		{
			if(null == objs || objs.Length == 0) { return true; }

			foreach(var one in objs) {
				if(null != one) { return false; }
			}
			return true;
		}
		#endregion

		#region Array
		#endregion

		#region XmlNode
		public static bool IsNullOrEmpty(this XmlNode node)
		{
			return
				null == node
				||
				null == node.ChildNodes
				||
				node.ChildNodes.Count == 0;
		}

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
			return value.ToInt(airBag);
		}

		public static bool AttributeToBoolean(this XmlNode node, string name)
		{
			return AttributeToBoolean(node, name, false);
		}
		public static bool AttributeToBoolean(
			this XmlNode node, string name, bool airBag)
		{
			string value = AirBagXmlAttr(node, name);
			return value.ToBoolean(airBag);
		}

		public static string AirBagXmlAttr(this XmlNode node, string name)
		{
			return node.AirBagXmlAttr(name, string.Empty);
		}

		public static string AirBagXmlAttr(
			this XmlNode node, string name, string airBag)
		{
			if(null == node || null == node.Attributes) { return airBag; }
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

		public static string ChildInnerText(this XmlNode node, string tag)
		{
			return Extender.ChildInnerText(node, tag, string.Empty);
		}

		public static string ChildInnerText(
			this XmlNode node, string tag, string airBag)
		{
			if(null == node) { return airBag; }
			XmlNode subNode = node.SelectSingleNode(tag);
			if(null == subNode) { return airBag; }
			string txt = subNode
				.NodeType
				.EnumIn(XmlNodeType.Text, XmlNodeType.CDATA)
					? node.Value
					: subNode.InnerText;
			return txt.IsNullOrEmpty() ? string.Empty : txt.Trim();
		}

		public static int ChildInnerInt(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToInt();
		}

		public static int ChildInnerInt(
			this XmlNode node, string tag, int airBag)
		{
			return node.ChildInnerText(tag).ToInt(airBag);
		}

		public static decimal ChildInnerDecimal(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToDecimal();
		}

		public static decimal ChildInnerDecimal(
			this XmlNode node, string tag, decimal airBag)
		{
			return node.ChildInnerText(tag).ToDecimal(airBag);
		}
		public static long ChildInnerLong(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToLong();
		}

		public static long ChildInnerLong(
			this XmlNode node, string tag, long airBag)
		{
			return node.ChildInnerText(tag).ToLong(airBag);
		}
		public static float ChildInnerFloat(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToFloat();
		}

		public static float ChildInnerFloat(
			this XmlNode node, string tag, float airBag)
		{
			return node.ChildInnerText(tag).ToFloat(airBag);
		}
		public static bool ChildInnerBoolean(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToBoolean();
		}

		public static bool ChildInnerBoolean(
			this XmlNode node, string tag, bool airBag)
		{
			return node.ChildInnerText(tag).ToBoolean(airBag);
		}
		public static DateTime ChildInnerDateTime(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToDateTime();
		}

		public static DateTime ChildInnerDateTime(
			this XmlNode node, string tag, DateTime airBag)
		{
			return node.ChildInnerText(tag).ToDateTime(airBag);
		}
		public static T ChildInnerEnum<T>(this XmlNode node, string tag)
		{
			return node.ChildInnerText(tag).ToEnum<T>();
		}

		public static T ChildInnerEnum<T>(
			this XmlNode node, string tag, T airBag)
		{
			return node.ChildInnerText(tag).ToEnum<T>(airBag);
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

		public static List<Any> ToAnys(this Exception ex, params Any[] anys)
		{
			if(null == ex) {
				return anys.IsNullOrEmpty()
					? new List<Any>()
					: new List<Any>(anys);
			}
			List<Any> list = new List<Any>();
			if(!anys.IsNullOrEmpty()) { list.AddRange(anys); }
			if(null != ex) {
				list.Add(new Any("source", ex.Source));
				list.Add(new Any("Message", ex.Message));
				list.Add(new Any("TargetSite", ex.TargetSite));
				list.Add(new Any("HelpLink", ex.HelpLink));
				list.Add(new Any(
					"Stack Trace",
					"..." + Environment.NewLine + ex.StackTrace
				));
			}
			return list;
		}

		//public static List<Any> ToAnys(this object obj, params Any[] anys)
		//{
		//	if(null == obj) {
		//		return anys.IsNullOrEmpty()
		//			? new List<Any>()
		//			: new List<Any>(anys);
		//	}
		//	return obj.ToAnys(true, anys);
		//}

		public static List<Any> ToAnys(
			this object obj, bool needCanWrite, params Any[] anys)
		{
			if(null == obj) { return new List<Any>(); }
			return Reflector.ToAny(obj, needCanWrite, anys);
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
		//public static bool IsNullOrEmpty(this object[] objs)
		//{
		//	return null == objs || objs.Length == 0;
		//}

		public static Any[] ToAnys(
			this object[] objs,
			string namePropertyName,
			string valuePropertyName)
		{
			if(objs.IsNullOrEmpty()) { return new Any[0]; }

			List<Any> list = new List<Any>();
			foreach(object obj in objs) {
				list.Add(new Any(
					Reflector.GetValue(namePropertyName, obj).ToString(),
					Reflector.GetValue(valuePropertyName, obj)
				));
			}

			return list.ToArray();
		}
		#endregion

		#region StringBuilder
		public static bool IsNullOrEmpty(this StringBuilder sb)
		{
			return null == sb || sb.Length == 0;
		}

		/// <summary>
		/// Remove last i characters.
		/// </summary>
		/// <param name="i">how much characters to remove</param>
		/// <returns></returns>
		public static StringBuilder RemoveLast(this StringBuilder sb, int i)
		{
			if(null == sb) { return sb; }
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
			if(sb.IsNullOrEmpty()) { return sb; }
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
			if(sb.IsNullOrEmpty()) { return sb; }
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
			if(null == ipAddress) { return false; }
			if(-1 == start || -1 == end) { return false; }

			// Compare
			long ip = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);
			//long ip = ipAddress.To Formator.IPAddressToLong(ipAddress);
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
			if(null == ipAddress) { return false; }
			long startL = BitConverter.ToInt32(IPAddress.Parse(start).GetAddressBytes(), 0);
			long endL = BitConverter.ToInt32(IPAddress.Parse(end).GetAddressBytes(), 0);
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
			if(null == ipAddress) { return false; }
			long startL = BitConverter.ToInt32(start.GetAddressBytes(), 0);
			long endL = BitConverter.ToInt32(end.GetAddressBytes(), 0);
			return ipAddress.InRange(startL, endL);
		}

		/// <summary>
		/// Check whether the IPAddress is one of private ip.
		/// </summary>
		/// <param name="ipAddress">IPAddress object</param>
		/// <returns>This IPAddress is one of private ip.</returns>
		public static bool IsPrivate(this IPAddress ipAddress)
		{
			if(null == ipAddress) { return false; }
			long ip = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);
			return false; // <<<
		}
		#endregion

		#region FileStream
		public static string ToMD5(this FileStream fileStream)
		{
			if(null == fileStream) { return string.Empty; }
			byte[] hash = HashAlgorithm.Create(Constants.Encode.MD5).ComputeHash(fileStream);
			return Convert.ToBase64String(hash);
		}
		public static string ToSHA(this FileStream fileStream)
		{
			if(null == fileStream) { return string.Empty; }
			byte[] hash = HashAlgorithm.Create(Constants.Encode.SHA512).ComputeHash(fileStream);
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
			if(null != dict) {
				foreach(var key in dict.Keys) {
					clone.Add(key, dict[key]);
				}
			}
			return clone;
		}

		public static T[] ToArray<T>(this Dictionary<string, T>.ValueCollection values)
		{
			List<T> list = new List<T>();
			if(null == values) {
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
			if(null == color) { color = Color.White; }
			return ColorTranslator.ToHtml(color);
		}

		/// <summary>
		/// To hexadecimal string, without lead hash.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string ToHexString(this Color color)
		{
			if(null == color) { color = Color.White; }
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
			if(null == vals) { return false; }

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

		#region Image
		public static string ToBase64(this System.Drawing.Image image)
		{
			return string.Empty; // Formator.ToBase64(image);
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

		public static string Join(this IEnumerable<string> list)
		{
			if(list.IsNullOrEmpty()) { return string.Empty; }
			return list.ToArray().Join(Constants.Symbol.Comma);
		}

		public static string Join(
			this IEnumerable<string> list, 
			string separator)
		{
			if(list.IsNullOrEmpty()) { return string.Empty; }
			return list.ToArray().Join(separator);
		}

		public static string Join(
			this IEnumerable<string> list,
			string separator,
			string quoter)
		{
			if(list.IsNullOrEmpty()) { return string.Empty; }
			return list.ToArray().Join(separator, quoter);
		}

		public static string Join(
			this IEnumerable<string> list,
			string separator,
			string quoter0,
			string quoter9)
		{
			if(list.IsNullOrEmpty()) { return string.Empty; }
			return list.ToArray().Join(separator, quoter0, quoter9);
		}
		#endregion

		#region ICollection
		//public static bool IsNullOrEmpty<T>(this ICollection<T> cols)
		//{
		//	return cols == null || cols.Count == 0;
		//}
		#endregion

		#region With
		public static T With<T>(this T item, Action<T> action)
		{
			action(item);
			return item;
		}
		#endregion

		#region ToXxx
		//public static int ToInt(this string value)
		//{
		//	return Formator.AirBagToInt(value);
		//}

		//public static int ToInt(this string value, int airBag)
		//{
		//	return Formator.AirBagToInt(value, airBag);
		//}

		//public static decimal ToDecimal(this string value)
		//{
		//	return Formator.AirBagToDecimal(value);
		//}

		//public static decimal ToDecimal(this string value, decimal airBag)
		//{
		//	return Formator.AirBagToDecimal(value, airBag);
		//}

		//public static long ToLong(this string value)
		//{
		//	return Formator.AirBagToLong(value);
		//}

		//public static long ToLong(this string value, long airBag)
		//{
		//	return Formator.AirBagToLong(value, airBag);
		//}

		//public static float ToFloat(this string value)
		//{
		//	return Formator.AirBagToFloat(value);
		//}

		//public static float ToFloat(this string value, float airBag)
		//{
		//	return Formator.AirBagToFloat(value, airBag);
		//}

		//public static bool ToBoolean(this string value)
		//{
		//	return Formator.AirBagToBoolean(value);
		//}

		//public static bool ToBoolean(this string value, bool airBag)
		//{
		//	return Formator.AirBagToBoolean(value, airBag);
		//}

		//public static string ToBooleanString(this string value)
		//{
		//	return Formator.AirBagToBoolean(value)
		//		? Constants.StringBoolean.True
		//		: Constants.StringBoolean.False;
		//}

		//public static DateTime ToDateTime(this string value)
		//{
		//	return Formator.AirBagToDateTime(value);
		//}

		//public static DateTime ToDateTime(this string value, DateTime airBag)
		//{
		//	return Formator.AirBagToDateTime(value, airBag);
		//}

		//public static T ToEnum<T>(this string value)
		//{
		//	return Formator.AirBagToEnum<T>(value);
		//}

		//public static T ToEnum<T>(this string value, T airBag)
		//{
		//	return Formator.AirBagToEnum<T>(value, airBag);
		//}

		//public static List<T> ToEnums<T>(this string value, string separator)
		//{
		//	List<T> list = new List<T>();
		//	if(value.IsNullOrEmpty()) { return list; }
		//	foreach(var one in value.SplitWith(separator)) {
		//		list.Add(Formator.AirBagToEnum<T>(one));
		//	}
		//	return list;
		//}

		//public static List<T> ToEnums<T>(this string value, string separator, T airBag)
		//{
		//	List<T> list = new List<T>();
		//	if(value.IsNullOrEmpty()) { return list; }
		//	foreach(var one in value.SplitWith(separator)) {
		//		list.Add(Formator.AirBagToEnum<T>(one, airBag));
		//	}
		//	return list;
		//}
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

		#region XmlAttributeCollection
		public static bool IsNullOrEmpty(XmlAttributeCollection xmlAttrs)
		{
			return null == xmlAttrs || xmlAttrs.Count == 0;
		}
		#endregion

		#region XmlAttribute
		public static bool IsNullOrEmpty(XmlAttribute xmlAttr)
		{
			return null == xmlAttr || xmlAttr.Value.IsNullOrEmpty();
		}
		#endregion

		#region XmlNodeList
		public static bool IsNullOrEmpty(XmlNodeList list)
		{
			return null == list || list.Count == 0;
		}
		#endregion

		#region List<Any>
		// ToIntegers
		public static int[] ToIntegers(this List<Any> anys, string name)
		{
			return anys.ToIntegers(name, Constants.Symbol.Comma);
		}
		public static int[] ToIntegers(
			this List<Any> anys, string name, string seperator)
		{
			return anys.ToIntegers(name, seperator, new int[0]);
		}
		public static int[] ToIntegers(
			this List<Any> anys, string name, int[] airBag)
		{
			return anys.ToIntegers(name, Constants.Symbol.Comma, airBag);
		}
		public static int[] ToIntegers(
			this List<Any> anys, string name, string seperator, int[] airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToIntegers(seperator, airBag);
		}

		// ToStrings
		public static string[] ToStrings(this List<Any> anys, string name)
		{
			return anys.ToStrings(name, Constants.Symbol.Comma);
		}
		public static string[] ToStrings(
			this List<Any> anys, string name, string seperator)
		{
			return anys.ToStrings(name, Constants.Symbol.Comma, new string[0]);
		}
		public static string[] ToStrings(
			this List<Any> anys, string name, string[] airBag)
		{
			return anys.ToStrings(name, Constants.Symbol.Comma, airBag);
		}
		public static string[] ToStrings(
			this List<Any> anys, string name, string seperator, string[] airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToStrings(seperator, airBag);
		}

		// ToString
		public static string ListAnys(this List<Any> anys)
		{
			if(anys.IsNullOrEmpty()) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			int index = 0;
			int length = (int)Math.Ceiling(Math.Log10(anys.Count() + 1));
			int maxLen = 0;
			foreach(var any in anys) {
				if(null == any.Name) { continue; }
				maxLen = Math.Max(maxLen, any.Name.Length);
			}
			foreach(var any in anys) {
				if(index > 0) { sb.AppendLine(); }

				string name = any.Name.AirBag();

				if(null != any.Value) {
					Type type = any.Value.GetType();

					// Array
					if(type.IsArray) {
						Array array = any.Value as Array;
						if(null != array) {
							++index;
							int idx = 0;
							int len = (int)Math.Ceiling(
								Math.Log10(array.Length)
							);
							foreach(var x in array) {
								if(idx > 0) { sb.AppendLine(); }
								string val = x.ToString();
								if(val.Contains(Environment.NewLine)) {
									val = val.Replace(
										Environment.NewLine,
										String.Concat(
											Environment.NewLine,
											" ".Repeat(len + maxLen + 5)
										)
									);
								}

								sb.AppendFormat(
									"{0} {1}{2} = {3} {4}",
									index.OrderList(length),
									name,
									" ".Repeat(maxLen - name.Length),
									(++idx).OrderList(len),
									val
								);
							}
							continue;
						}
					}
				}

				string value = any.ToString();
				if(null == value) {
					if(!any.Values.IsNullOrEmpty()) {
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
							" ".Repeat(length + maxLen + 5)
						)
					);
				}

				sb.AppendFormat(
					"{0} {1}{2} = {3}",
					(++index).OrderList(length),
					name,
					" ".Repeat(maxLen - name.Length),
					value
				);
			}
			return sb.ToString();
		}
		public static string ToString(this List<Any> anys, string name)
		{
			return anys.ToString(name, string.Empty);
		}
		public static string ToString(
			this List<Any> anys, string name, string airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToString(airBag);
		}

		// ToBoolean
		public static bool ToBoolean(this List<Any> anys, string name)
		{
			return anys.ToBoolean(name, default(bool));
		}
		public static bool ToBoolean(
			this List<Any> anys, string name, bool airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToBoolean(airBag);
		}

		// ToDateTime
		public static DateTime ToDateTime(this List<Any> anys, string name)
		{
			return default(DateTime); //anys.ToDateTime(default(DateTime));
		}
		public static DateTime ToDateTime(
			this List<Any> anys, string name, DateTime airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToDateTime(airBag);
		}

		// ToByte
		public static byte ToByte(this List<Any> anys, string name)
		{
			return default(byte); //anys.ToByte(default(byte));
		}
		public static byte ToByte(this List<Any> anys, string name, byte airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToByte(airBag);
		}

		// ToChar
		public static char ToChar(this List<Any> anys, string name)
		{
			return default(char); //anys.ToChar(default(char));
		}
		public static char ToChar(this List<Any> anys, string name, char airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToChar(airBag);
		}

		// ToInteger
		public static int ToInteger(this List<Any> anys, string name)
		{
			return default(int); //anys.ToInteger(default(int));
		}
		public static int ToInteger(
			this List<Any> anys, string name, int airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToInteger(airBag);
		}

		// ToFloat
		public static float ToFloat(this List<Any> anys, string name)
		{
			return default(float); //anys.ToFloat(default(float));
		}
		public static float ToFloat(
			this List<Any> anys, string name, float airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToFloat(airBag);
		}

		// ToDouble
		public static double ToDouble(this List<Any> anys, string name)
		{
			return default(double); //anys.ToDouble(default(double));
		}
		public static double ToDouble(
			this List<Any> anys, string name, double airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToDouble(airBag);
		}

		// ToShort
		public static short ToShort(this List<Any> anys, string name)
		{
			return default(short); //anys.ToShort(default(short));
		}
		public static short ToShort(
			this List<Any> anys, string name, short airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToShort(airBag);
		}

		// ToLong
		public static long ToLong(this List<Any> anys, string name)
		{
			return default(long); //anys.ToLong(default(long));
		}
		public static long ToLong(
			this List<Any> anys, string name, long airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToLong(airBag);
		}

		// ToDecimal
		public static decimal ToDecimal(this List<Any> anys, string name)
		{
			return anys.ToDecimal(name, default(decimal));
		}
		public static decimal ToDecimal(
			this List<Any> anys, string name, decimal airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToDecimal(airBag);
		}

		// ToColor
		public static Color ToColor(this List<Any> anys, string name)
		{
			return anys.ToColor(name, default(Color));
		}
		public static Color ToColor(
			this List<Any> anys, string name, Color airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToColor(airBag);
		}

		// ToGuid
		public static Guid ToGuid(this List<Any> anys, string name)
		{
			return anys.ToGuid(name, default(Guid));
		}
		public static Guid ToGuid(this List<Any> anys, string name, Guid airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToGuid(airBag);
		}

		// ToByteArray
		public static byte[] ToByteArray(this List<Any> anys, string name)
		{
			return anys.ToByteArray(name, new byte[0]);
		}
		public static byte[] ToByteArray(
			this List<Any> anys, string name, byte[] airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToByteArray(airBag);
		}

		// ToEnum
		public static T ToEnum<T>(this List<Any> anys, string name)
		{
			return anys.ToEnum(name, default(T));
		}
		public static T ToEnum<T>(this List<Any> anys, string name, T airBag)
		{
			if(anys.IsNullOrEmpty()) { return airBag; }
			var any = anys.FirstOrDefault(x => x.Name == name);
			if(null == any) { return airBag; }
			return any.ToEnum(airBag);
		}
		#endregion

		#region List<Many>
		// ToAnys
		public static List<Any> ToAnys(this List<Many> manys)
		{
			if(manys.IsNullOrEmpty()) { return new List<Any>(); }
			List<Any> anys = new List<Any>();
			manys.ForEach(x => anys.Add(x.ToAny()));
			return anys;
		}
		public static List<Any> ToAnys(this List<Many> manys, string group)
		{
			if(manys.IsNullOrEmpty()) { return new List<Any>(); }
			var list = manys.Where(x => x.Group == group);
			return manys.ToAnys();
		}

		// ToIntegers
		public static int[] ToIntegers(
			this List<Many> manys, string group, string name)
		{
			return manys.ToIntegers(
				group, name, Constants.Symbol.Comma, new int[0]
			);
		}
		public static int[] ToIntegers(
			this List<Many> manys, string group, string name, string seperator)
		{
			return manys.ToIntegers(
				group, name, seperator, new int[0]
			);
		}
		public static int[] ToIntegers(
			this List<Many> manys, string group, string name, int[] airBag)
		{
			return manys.ToIntegers(
				group, name, Constants.Symbol.Comma, airBag
			);
		}
		public static int[] ToIntegers(
			this List<Many> manys,
			string group,
			string name,
			string seperator,
			int[] airBag)
		{
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToIntegers(seperator, airBag);
		}

		// ToStrings
		public static string[] ToStrings(
			this List<Many> manys, string group, string name)
		{
			return manys.ToStrings(
				group, name, Constants.Symbol.Comma, new string[0]
			);
		}
		public static string[] ToStrings(
			this List<Many> manys, string group, string name, string seperator)
		{
			return manys.ToStrings(
				group, name, seperator, new string[0]
			);
		}
		public static string[] ToStrings(
			this List<Many> manys, string group, string name, string[] airBag)
		{
			return manys.ToStrings(
				group, name, Constants.Symbol.Comma, airBag
			);
		}
		public static string[] ToStrings(
			this List<Many> manys,
			string group,
			string name,
			string seperator,
			string[] airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToStrings(seperator, airBag);
		}

		// ToString
		public static string ToString(
			this List<Many> manys, string group, string name)
		{
			return manys.ToString(group, name, string.Empty);
		}
		public static string ToString(
			this List<Many> manys, string group, string name, string airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToString(airBag);
		}

		// ToBoolean
		public static bool ToBoolean(
			this List<Many> manys, string group, string name)
		{
			return manys.ToBoolean(group, name, default(bool));
		}
		public static bool ToBoolean(
			this List<Many> manys, string group, string name, bool airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToBoolean(airBag);
		}

		// ToDateTime
		public static DateTime ToDateTime(
			this List<Many> manys, string group, string name)
		{
			return manys.ToDateTime(group, name, default(DateTime));
		}
		public static DateTime ToDateTime(
			this List<Many> manys, string group, string name, DateTime airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToDateTime(airBag);
		}

		// ToByte
		public static byte ToByte(
			this List<Many> manys, string group, string name)
		{
			return manys.ToByte(group, name, default(byte));
		}
		public static byte ToByte(
			this List<Many> manys, string group, string name, byte airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToByte(airBag);
		}

		// ToChar
		public static char ToChar(
			this List<Many> manys, string group, string name)
		{
			return manys.ToChar(group, name, default(char));
		}
		public static char ToChar(
			this List<Many> manys, string group, string name, char airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToChar(airBag);
		}

		// ToInteger
		public static int ToInteger(
			this List<Many> manys, string group, string name)
		{
			return manys.ToInteger(group, name, default(int));
		}
		public static int ToInteger(
			this List<Many> manys, string group, string name, int airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToInteger(airBag);
		}

		// ToFloat
		public static float ToFloat(
			this List<Many> manys, string group, string name)
		{
			return manys.ToFloat(group, name, default(float));
		}
		public static float ToFloat(
			this List<Many> manys, string group, string name, float airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToFloat(airBag);
		}

		// ToDouble
		public static double ToDouble(
			this List<Many> manys, string group, string name)
		{
			return manys.ToDouble(group, name, default(double));
		}
		public static double ToDouble(
			this List<Many> manys, string group, string name, double airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToDouble(airBag);
		}

		// ToShort
		public static short ToShort(
			this List<Many> manys, string group, string name)
		{
			return manys.ToShort(group, name, default(short));
		}
		public static short ToShort(
			this List<Many> manys, string group, string name, short airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToShort(airBag);
		}

		// ToLong
		public static long ToLong(
			this List<Many> manys, string group, string name)
		{
			return manys.ToLong(group, name, default(long));
		}
		public static long ToLong(
			this List<Many> manys, string group, string name, long airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToLong(airBag);
		}

		// ToDecimal
		public static decimal ToDecimal(
			this List<Many> manys, string group, string name)
		{
			return manys.ToDecimal(group, name, default(decimal));
		}
		public static decimal ToDecimal(
			this List<Many> manys, string group, string name, decimal airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToDecimal(airBag);
		}

		// ToColor
		public static Color ToColor(
			this List<Many> manys, string group, string name)
		{
			return manys.ToColor(group, name, default(Color));
		}
		public static Color ToColor(
			this List<Many> manys, string group, string name, Color airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToColor(airBag);
		}

		// ToGuid
		public static Guid ToGuid(
			this List<Many> manys, string group, string name)
		{
			return manys.ToGuid(group, name, default(Guid));
		}
		public static Guid ToGuid(
			this List<Many> manys, string group, string name, Guid airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToGuid(airBag);
		}

		// ToByteArray
		public static byte[] ToByteArray(
			this List<Many> manys, string group, string name)
		{
			return manys.ToByteArray(group, name, new byte[0]);
		}
		public static byte[] ToByteArray(
			this List<Many> manys, string group, string name, byte[] airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToByteArray(airBag);
		}

		// ToEnum
		public static T ToEnum<T>(
			this List<Many> manys, string group, string name)
		{
			return manys.ToEnum(group, name, default(T));
		}
		public static T ToEnum<T>(
			this List<Many> manys, string group, string name, T airBag)
		{
			if(manys.IsNullOrEmpty()) { return airBag; }
			var many = manys.FirstOrDefault(x =>
				x.Group == group && x.Name == name
			);
			if(null == many) { return airBag; }
			return many.ToEnum(airBag);
		}
		#endregion

		#region Any[]
		public static bool IsNullOrEmpty(this Any[] objs)
		{
			return null == objs || objs.Length == 0;
		}
		#endregion
	}
}

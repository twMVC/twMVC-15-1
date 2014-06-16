// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Checker.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kuick
{
	public class Checker
	{
		#region constants & readonly
		public static readonly string[] SQL_PARAMETER_BLACK_LIST = {
			"--", ";--", ";", "/*", "*/", "@@",
			"fetch", "kill", "open",
			"begin", "end", 
			"create", "alter", "drop", "table", 
			"exec", "execute", "cursor", 
			"insert", "update", "delete", 
			"sys", "sysobjects", "syscolumns"
		};

		private static readonly Dictionary<char, int> TAIWAN_IDENTIFICATION_CARD_NUMBERS =
			new Dictionary<char, int>() 
		{
			{'A',  1}, {'B', 10}, {'C', 19}, {'D', 28},
			{'E', 37}, {'F', 46}, {'G', 55}, {'H', 64},
			{'I', 39}, {'J', 73}, {'K', 82}, {'L',  2},
			{'M', 11}, {'N', 20}, {'O', 48}, {'P', 29},
			{'Q', 38}, {'R', 47}, {'S', 56}, {'T', 65},
			{'U', 74}, {'V', 83}, {'W', 21}, {'X',  3},
			{'Y', 12}, {'Z', 30}
		};
		private static readonly Regex REGEX_UUID = new Regex(
			Constants.Pattern.Uuid,
			RegexOptions.Compiled
		);
		#endregion

		#region IsXxx
		#region IsNull
		public static bool IsNull(IBuiltin service)
		{
			return null == service || service.IsNull;
		}

		public static bool IsNull(Hashtable ht)
		{
			return null == ht || ht.Count == 0;
		}

		public static bool IsNull(DateTime date)
		{
			return date.Ticks == 0 || Constants.Date.Min.CompareTo(date) >= 0;
		}

		public static bool IsNull(params string[] strs)
		{
			if(null == strs) { return true; }
			foreach(string x in strs) {
				if(!string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)) {
					return false;
				}
			}
			return true;
		}

		public static bool IsNull(ArrayList al)
		{
			return null == al || al.Count == 0;
		}

		public static bool IsNull(NameValueCollection nvs)
		{
			return null == nvs || nvs.Count == 0;
		}

		public static bool IsNull(XmlAttributeCollection xmlAttrs)
		{
			return null == xmlAttrs || xmlAttrs.Count == 0;
		}

		public static bool IsNull(XmlAttribute xmlAttr)
		{
			return null == xmlAttr || IsNull(xmlAttr.Value);
		}

		public static bool IsNull(XmlNodeList list)
		{
			return null == list || list.Count == 0;
		}


		public static bool IsNull(object[] objs)
		{
			return null == objs || objs.Length == 0;
		}


		public static bool IsNull(Anys anys)
		{
			return null == anys || anys.Count == 0;
		}

		public static bool IsNull(ICollection c)
		{
			return null == c || c.Count == 0;
		}

		public static bool IsNull<T>(T obj)
		{
			// null
			if(null == obj) { return true; }

			// others
			if(obj is string) { return IsNull(obj as string); }
			if(obj is DateTime) { return IsNull(Convert.ToDateTime(obj)); }

			// ICollection
			ICollection c = obj as ICollection;
			if(null != c) { return Checker.IsNull(c); }
			// Hashtable
			Hashtable h = obj as Hashtable;
			if(null != h) { return Checker.IsNull(h); }
			// ArrayList
			ArrayList a = obj as ArrayList;
			if(null != a) { return Checker.IsNull(a); }
			// NameValueCollection
			NameValueCollection n = obj as NameValueCollection;
			if(null != n) { return Checker.IsNull(n); }
			// XmlAttributeCollection
			XmlAttributeCollection x = obj as XmlAttributeCollection;
			if(null != x) { return Checker.IsNull(x); }
			// XmlAttribute
			XmlAttribute xa = obj as XmlAttribute;
			if(null != xa) { return Checker.IsNull(xa); }
			// XmlNodeList
			XmlNodeList xn = obj as XmlNodeList;
			if(null != xn) { return Checker.IsNull(xn); }
			// Anys
			Anys anys = obj as Anys;
			if(null != anys) { return Checker.IsNull(anys); }
			// object[]
			if(obj is object[]) { return IsNull(obj as object[]); }
			// object
			if(null == obj) { return true; }
			return false;
		}

		public static bool IsNull<T>(T[] objs)
		{
			return null == objs || objs.Length == 0;
		}

		public static bool IsNull<T>(List<T> list)
		{
			if(null == list || list.Count == 0) { return true; }
			foreach(T item in list) {
				if(!Checker.IsNull<T>(item)) { return false; }
			}
			return true;
		}
		#endregion

		public static bool IsNullReference<T>(List<T> list)
		{
			return null == list;
		}

		public static bool IsNullReference<T>(T[] parts)
		{
			return null == parts;
		}

		/// <summary>
		/// Returns true if value is an integer.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsInt(string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			int result = default(int);
			return int.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsDecimal(string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			decimal result = default(decimal);
			return decimal.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsLong(string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			long result = default(long);
			return long.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsFloat(string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			float result = default(float);
			return float.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsDouble(string value)
		{
			CultureInfo info = CultureInfo.CurrentCulture;
			NumberFormatInfo provider = info.NumberFormat;

			double result = default(double);
			return double.TryParse(value, NumberStyles.Any, provider, out result);
		}

		public static bool IsByte(string value)
		{
			byte result = default(byte);
			return byte.TryParse(value, out result);
		}

		public static bool IsChar(string value)
		{
			char result = default(char);
			return char.TryParse(value, out result);
		}

		public static bool IsDateTime(string value)
		{
			DateTime result;
			return DateTime.TryParse(value, out result);
		}

		public static bool IsGuid(string value)
		{
			try {
				if(string.IsNullOrEmpty(value)) { return false; }
				if(value.Length != 32 && value.Length != 26) { return false; }

				Guid guid = value.Length == 32
					? new Guid(value)
					: new Guid(Utility.Base32Encoding.ToBytes(value));
				return true;
			} catch {
				return false;
			}
		}

		/// <summary>
		/// Returns true if value is an even number.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		public static bool IsEven(int number)
		{
			return number >= 0 && ((number & 1) == 0);
		}

		public static bool IsOdd(int number)
		{
			return number >= 0 && ((number & 1) == 1);
		}

		public static bool IsEven(long number)
		{
			return number >= 0 && ((number & 1) == 0);
		}

		public static bool IsOdd(long number)
		{
			return number >= 0 && ((number & 1) == 1);
		}

		public static bool Divisible(long dividend, long divisor)
		{
			return divisor >= 0 && dividend >= 0 && ((dividend % divisor) == 0);
		}

		public static bool Divisible(int dividend, int divisor)
		{
			return divisor >= 0 && dividend >= 0 && ((dividend % divisor) == 0);
		}

		[Obsolete("This method move to Kuick.Calculator.")]
		public static long Remainder(long dividend, long divisor)
		{
			if(divisor >= 0 && dividend >= 0) {
				return dividend % divisor;
			} else {
				return -1;
			}
		}

		[Obsolete("This method move to Kuick.Calculator.")]
		public static int Remainder(int dividend, int divisor)
		{
			if(divisor >= 0 && dividend >= 0) {
				return dividend % divisor;
			} else {
				return -1;
			}
		}

		/// <summary>
		/// string pattern, Regular Expression match check
		/// </summary>
		/// <param name="value">The target string</param>
		/// <param name="pattern">Regular Expression Pattern</param>
		/// <param name="ignoreCase">Ignore case or not</param>
		/// <returns>Match or not</returns>
		public static bool IsMatch(string value, string pattern, bool ignoreCase)
		{
			return Regex.IsMatch(
				value,
				pattern,
				ignoreCase
					? RegexOptions.Compiled | RegexOptions.IgnoreCase
					: RegexOptions.Compiled
				);
		}

		public static bool IsMatch(string value, string pattern)
		{
			return IsMatch(value, pattern, false);
		}

		public static bool IsMatch(string cipher, string plaintext, Encryption method)
		{
			try {
				if(!string.IsNullOrEmpty(cipher)) {
					cipher = cipher.TrimStart(Constants.Prefix.Encrypted);
				}

				if(Checker.IsNull(cipher) && Checker.IsNull(plaintext)) {
					return true;
				}
				if(Checker.IsNull(cipher) || Checker.IsNull(plaintext)) {
					return false;
				}

				switch(method) {
					case Encryption.Symmetry:
						return cipher.Decrypt().Equals(
							plaintext, StringComparison.Ordinal
						);
					case Encryption.Asymmetry:
						if(cipher.Equals(plaintext, StringComparison.Ordinal)) { 
							return true; 
						}
						return cipher.Equals(
							plaintext.ToSHA(), StringComparison.Ordinal
						);
				}
			} catch(Exception ex) {
				Logger.Error("Kuick.Checker.IsMatch", ex);
			}

			return false;
		}

		public static bool IsEncrypted(string value)
		{
			return Checker.IsNull(value)
				? false
				: value.StartsWith(Constants.Prefix.Encrypted);
		}

		public static bool IsMatch(DateTime date, Frequency frequency)
		{
			DateTime now = DateTime.Now;

			switch(frequency) {
				case Frequency.Once:
					return true;
				case Frequency.Annual:
					return now.yyyy().Equals(date.yyyy());
				case Frequency.Monthly:
					return now.yyyyMM().Equals(date.yyyyMM());
				case Frequency.Weekly:
					return Dater.StartOfWeek().Equals(Dater.StartOfWeek(date));
				case Frequency.Daily:
					return now.yyyyMMdd().Equals(date.yyyyMMdd());
				case Frequency.Hourly:
					return now.yyyyMMddHH().Equals(date.yyyyMMddHH());
				case Frequency.Minutely:
					return now.yyyyMMddHHmm().Equals(date.yyyyMMddHHmm());
				default:
					// Frequency.Once
					return true;
			}
		}

		public static bool IsFlagEnum(Type type)
		{
			if(null == type) { return false; }
			return type.IsEnum
				? !Checker.IsNull(type.GetCustomAttributes(typeof(FlagsAttribute), false))
				: false;
		}

		public static bool IsPowerOf2(int value)
		{
			while(value > 1) {
				int remainder;
				int quotient = Math.DivRem(value, 2, out remainder);
				if(remainder > 0) { return false; }
				value = quotient;
			}
			return value == 1;
		}

		public static bool Overdue(DateTime date, Frequency duration)
		{
			DateTime now = DateTime.Now;

			long x = 0L;
			long y = 0L;
			switch(duration) {
				case Frequency.Annual:
					x = now.Year;
					y = date.Year;
					break;
				case Frequency.Monthly:
					x = Formator.AirBagToLong(now.yyyyMMs());
					y = Formator.AirBagToLong(date.yyyyMMs());
					break;
				case Frequency.Daily:
					x = Formator.AirBagToLong(now.yyyyMMdds());
					y = Formator.AirBagToLong(date.yyyyMMdds());
					break;
				case Frequency.Hourly:
					x = Formator.AirBagToLong(now.yyyyMMddHHs());
					y = Formator.AirBagToLong(date.yyyyMMddHHs());
					break;
				case Frequency.Minutely:
				default:
					x = Formator.AirBagToLong(now.yyyyMMddHHmms());
					y = Formator.AirBagToLong(date.yyyyMMddHHmms());
					break;
			}
			return x > y;
		}

		/// <summary>
		/// Is Taiwan Identification Card Number, ex: A123456789
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsTaiwanIdentificationCardNumber(string value)
		{
			if(!Regex.IsMatch(
				value,
				@"^[A-Z][12]\d{8}$",
				RegexOptions.IgnoreCase | RegexOptions.Compiled)) {
				return false;
			}

			char[] id = value.ToUpper().ToCharArray();
			int c;
			if(!TAIWAN_IDENTIFICATION_CARD_NUMBERS.TryGetValue(id[0], out c)) {
				Logger.Error(
					"Checker.IsTaiwanIdentificationCardNumber",
					"Can not find specify char in the TAIWAN_IDENTIFICATION_CARD_NUMBERS dictionary.",
					new Any("char", id[0])
				);
				return false;
			}

			int[] b = { 1, 8, 7, 6, 5, 4, 3, 2, 1 };
			for(var i = b.Length - 1; i > 0; i--) {
				c += int.Parse(id[i].ToString()) * b[i];
			}

			return (10 - c % 10) % 10 == int.Parse(id[9].ToString());
		}

		public static bool IsChinaIdentificationCardNumber(string value)
		{
			// TODO: IsChinaIdentificationCardNumber
			throw new NotImplementedException();
		}

		public static bool IsCreditcardNumber(string value)
		{
			// TODO: IsCreditcardNumber
			throw new NotImplementedException();
		}

		public static bool IsUnifiedSerialNumber(string value)
		{
			// TODO: IsUnifiedSerialNumber
			throw new NotImplementedException();
		}

		public static bool IsEmailAddress(string value)
		{
			return Regex.IsMatch(value, Constants.Pattern.Email, RegexOptions.Compiled);
		}

		public static bool IsIPAddress(string value)
		{
			return Regex.IsMatch(value, Constants.Pattern.Ip, RegexOptions.Compiled);
		}

		public static bool IsIP(string ip)
		{
			return IsIPAddress(ip);
		}

		public static bool IsIP(long ip)
		{
			return ip.Between(Constants.Ip.MinAsLong, Constants.Ip.MaxAsLong);
		}

		public static bool IsLoopBackIP(string ip)
		{
			if(!IsIPAddress(ip)) { return false; }
			return ip.StartsWith("127.");
		}

		public static bool IsLoopBackIP(long ip)
		{
			return ip == Constants.Ip.LoopBackAsLong;
		}

		public static bool IsPrivateIP(string ip)
		{
			return IsIP(ip)
				? IsPrivateIP(Formator.IPToLong(ip))
				: false;
		}

		public static bool IsPrivateIP(long ip)
		{
			if(!IsIP(ip)) {
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

		public static bool IsNullIP(string ip)
		{
			return Constants.Null.Ip == ip || !IsIP(ip);
		}

		public static bool IsNullIP(long ip)
		{
			return 0 == ip || !IsIP(ip);
		}

		public static bool IsNetworkAvailable
		{
			get
			{
				return NetworkInterface.GetIsNetworkAvailable();
			}
		}

		public static bool IsSqlUUID(string uuid)
		{
			return REGEX_UUID.IsMatch(uuid);
		}
		#endregion

		#region MayBe
		public static bool MayBeJson(string raw)
		{
			raw = raw.Trim();
			return
				(
					raw.StartsWith(Constants.Symbol.OpenBrace) 
					&& 
					raw.EndsWith(Constants.Symbol.CloseBrace)
				)
				||
				(
					raw.StartsWith(Constants.Symbol.OpenBracket) 
					&& 
					raw.EndsWith(Constants.Symbol.CloseBracket)
				);
		}
		#endregion

		#region Formating check with Regular Expression
		/// <summary>
		/// Returns true if value is a valid email address.
		/// </summary>
		/// <param name="emailAddress"></param>
		/// <returns></returns>
		public static bool IsEmail(string emailAddress)
		{
			Regex regex = new Regex(Constants.Pattern.Email, RegexOptions.Compiled);
			return regex.IsMatch(emailAddress);
		}

		/// <summary>
		/// Returns true if value is composed of alphabets.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsAlphabet(string str)
		{
			Regex regex = new Regex(Constants.Pattern.Alphabet, RegexOptions.Compiled);
			return regex.IsMatch(str);
		}

		/// <summary>
		/// Returns true if value is composed of numerics.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsNumeric(string str)
		{
			Regex regex = new Regex(Constants.Pattern.Numeric, RegexOptions.Compiled);
			return regex.IsMatch(str);
		}

		/// <summary>
		/// Returns true if value is composed of alphanumeric characters.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsAlphaNumeric(string str)
		{
			Regex regex = new Regex(Constants.Pattern.AlphaNumeric, RegexOptions.Compiled);
			return regex.IsMatch(str);
		}

		/// <summary>
		/// Returns true if value is a valid URI.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static bool IsUri(string uri)
		{
			Regex regex = new Regex(Constants.Pattern.Uri, RegexOptions.Compiled);
			return regex.IsMatch(uri);
		}
		#endregion

		#region Misc
		/// <summary>
		/// Compares two DateTime objects.
		/// Returns True if they are equal to the precision of milliseconds.
		/// </summary>
		/// <param name="date1"></param>
		/// <param name="date2"></param>
		/// <returns></returns>
		public static bool Equals(DateTime date1, DateTime date2)
		{
			string a = Formator.ToString17(date1);
			string b = Formator.ToString17(date2);
			return a.Equals(b);
		}

		public static bool Equals(string str1, string str2)
		{
			if(IsNull(str1) || IsNull(str2)) { return false; }
			string a = Formator.ToLowerAndCompact(str1);
			string b = Formator.ToLowerAndCompact(str2);
			return a.Equals(b);
		}

		/// <summary>
		/// Do argument contains DBCS(Double Byte Character Set)
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>Contain DBCS or not.</returns>
		public static bool ContainDBCS(string str)
		{
			return Checker.IsNull(str)
				? false
				: Encoding.UTF8.GetByteCount(str) > str.Length;
		}

		/// <summary>
		/// Checks a string for SQL injection attacks.
		/// Returns true if string contains black-listed characters.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static bool HasSqlInjection(string parameter)
		{
			// kevinjong: 2011-06-14 marked
			return false;

			//string[] parts = parameter.Split(
			//    new string[] { " ", Environment.NewLine, "\n" },
			//    StringSplitOptions.RemoveEmptyEntries
			//);
			//for(int i = 0; i < SQL_PARAMETER_BLACK_LIST.Length; i++) {
			//    foreach(string part in parts) {
			//        if(part.Equals(
			//            SQL_PARAMETER_BLACK_LIST[i],
			//            StringComparison.OrdinalIgnoreCase)) {
			//            return true;
			//        }
			//    }
			//}
			//return false;
		}

		public static void NullArgument<T>(T obj)
		{
			if(Checker.IsNull<T>(obj)) {
				throw new ArgumentNullException(typeof(T).GetType().Name);
			}
		}

		/// <summary>
		/// Get javascript function to check the string is Taiwan identification card 
		/// number or not
		/// </summary>
		/// <param name="functionName">client javascript function name</param>
		public static string GetJs_IsTaiwanIdentificationCardNumber(string functionName)
		{
			return String.Concat(
				"function ",
				functionName,
				@"(s){if(!/^[A-Z][12]\d{8}$/i.test(s))return!1;var a=s.toUpperCase().split(''),o={A:1,B:10,C:19,D:28,E:37,F:46,G:55,H:64,I:39,J:73,K:82,L:2,M:11,N:20,O:48,P:29,Q:38,R:47,S:56,T:65,U:74,V:83,W:21,X:3,Y:12,Z:30},b=[1,8,7,6,5,4,3,2,1],c=o[a[0]];for(var i=b.length-1;i;i--){c+=a[i]*b[i]}return((10-c%10)%10==a[9])}");
		}
		#endregion

		#region Exception
		public static void ArgumentNotNull(object argumentValue, string argumentName)
		{
			if(argumentValue == null) { throw new ArgumentNullException(argumentName); }
		}
		#endregion

		#region InRange
		public bool InRange(Frequency frequency, DateTime timeStamp)
		{
			DateTime from;
			DateTime to;

			switch(frequency) {
				case Frequency.Annual:
					from = timeStamp.StartOfYear();
					to = timeStamp.EndOfYear();
					break;
				case Frequency.Monthly:
					from = timeStamp.StartOfMonth();
					to = timeStamp.EndOfMonth();
					break;
				case Frequency.Weekly:
					from = timeStamp.StartOfWeek();
					to = timeStamp.EndOfWeek();
					break;
				case Frequency.Daily:
					from = timeStamp.StartOfDay();
					to = timeStamp.EndOfDay();
					break;
				case Frequency.Hourly:
					from = timeStamp.StartOfHour();
					to = timeStamp.EndOfHour();
					break;
				case Frequency.Minutely:
					from = timeStamp.StartOfMinute();
					to = timeStamp.EndOfMinute();
					break;
				case Frequency.Once:
					from = new DateTime(1755, 1, 1, 0, 0, 0, 0);
					to = Constants.Date.Max;
					break;
				default:
					throw new NotImplementedException(string.Format(
						"Kuick.Checker.InRange: {0}",
						frequency.ToString()
					));
			}

			return DateTime.Now.Between(from, to);
		}
		#endregion

		#region Flag
		public class Flag
		{
			public static int Grant(Type type, int current, int grant)
			{
				IsFlagCheck(type);
				return (int)Enum.Parse(type, (current | grant).ToString());
			}

			public static T Grant<T>(int current, int grant)
			{
				Type type = typeof(T);
				IsFlagCheck(type);
				return (T)Enum.Parse(type, (current | grant).ToString());
			}

			public static int Revoke(Type type, int current, int revoke)
			{
				IsFlagCheck(type);
				return (int)Enum.Parse(type, (current & (current ^ revoke)).ToString());
			}

			public static T Revoke<T>(int current, int revoke)
			{
				Type type = typeof(T);
				IsFlagCheck(type);
				return (T)Enum.Parse(type, (current & (current ^ revoke)).ToString());
			}

			public static bool Check(int current, int check)
			{
				return (current & check) == check;
			}

			//public static bool Check<T>(T current, T check)
			//{
			//    Type type = typeof(T);
			//    IsFlagCheck(type);
			//    return Check((int)current, (int)check);
			//}

			public static Any[] ToAnys<T>()
			{
				Type type = typeof(T);
				IsFlagCheck(type);
				return ToAnys(type);
			}

			public static Any[] ToAnys(Type type)
			{
				IsFlagCheck(type);
				return Checker.IsFlagEnum(type)
					? Formator.FlagEnumToAnys(type)
					: new Any[0];
			}

			public static string[] BelongsTo(Type type, int check)
			{
				IsFlagCheck(type);
				List<string> list = new List<string>();
				Any[] anys = ToAnys(type);
				foreach(Any any in anys) {
					if(Check(any.ToInteger(), check)) { list.Add(any.Name); }
				}
				return list.ToArray();
			}

			public static string[] CombinedBy(Type type, int check)
			{
				IsFlagCheck(type);
				List<string> list = new List<string>();
				Any[] anys = ToAnys(type);
				foreach(Any any in anys) {
					int current = any.ToInteger();
					if(Check(check, current) && current != 0 && current != check) {
						list.Add(any.Name);
					}
				}
				return list.ToArray();
			}

			private static void IsFlagCheck<T>()
			{
				IsFlagCheck(typeof(T));
			}

			private static void IsFlagCheck(Type type)
			{
				if(!Checker.IsFlagEnum(type)) {
					throw new ArgumentException(string.Format(
						"'{0}' -- Only flag enum was allowed!",
						type.Name
					));
				}
			}
		}
		#endregion

		#region Fuzzy Comparison
		public static bool Like(string left, string right)
		{
			string a = left.ToLower().Replace("_", string.Empty);
			string b = right.ToLower().Replace("_", string.Empty);
			return a == b;
		}
		#endregion

		#region Barcode
		/// <summary>
		/// http://stackoverflow.com/questions/10143547/how-do-i-validate-a-upc-or-ean-code
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static bool Barcode(string code)
		{
			if(!IsNumeric(code)) { return false; }

			switch(code.Length) {
				case 8:
					code = "000000" + code;
					break;
				case 12:
					code = "00" + code;
					break;
				case 13:
					code = "0" + code;
					break;
				case 14:
					break;
				default:
					return false;
			}

			int[] a = new int[13];
			a[0] = int.Parse(code[0].ToString()) * 3;
			a[1] = int.Parse(code[1].ToString());
			a[2] = int.Parse(code[2].ToString()) * 3;
			a[3] = int.Parse(code[3].ToString());
			a[4] = int.Parse(code[4].ToString()) * 3;
			a[5] = int.Parse(code[5].ToString());
			a[6] = int.Parse(code[6].ToString()) * 3;
			a[7] = int.Parse(code[7].ToString());
			a[8] = int.Parse(code[8].ToString()) * 3;
			a[9] = int.Parse(code[9].ToString());
			a[10] = int.Parse(code[10].ToString()) * 3;
			a[11] = int.Parse(code[11].ToString());
			a[12] = int.Parse(code[12].ToString()) * 3;
			int sum = 
				a[0] + a[1] + a[2] + a[3] + a[4] + a[5] + a[6] + 
				a[7] + a[8] + a[9] + a[10] + a[11] + a[12];
			int check = (10 - (sum % 10)) % 10;

			int last = int.Parse(code[13].ToString());
			return check == last;
		}
		#endregion

		#region TaxUniformNo
		public static bool TaxUniformNo(string code)
		{
			if(IsNull(code) || code.Length != 8 || !IsNumeric(code)) {
				return false;
			}

			int[] factors = new int[] { 1, 2, 1, 2, 1, 2, 4, 1 };
			int sum = 0;
			for(int i = 0; i < 8; i++) {
				int factor = factors[i];
				sum += Utility.SumIntoSingleDigit(
					Convert.ToInt32(code.Substring(i, 1)) * factor
				);
			}

			return sum % 10 == 0;
		}
		#endregion
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Constants.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Globalization;
using System.Net.Mime;

namespace Kuick
{
	public class Constants
	{
		// misc.
		public const string LocalHost = "localhost";
		public const string LocalIp = "127.0.0.1";
		public const string LocalIpIPv6 = "::1";
		public const long OneSecondTicks = 10 * 10 * 10 * 10 * 10 * 10 * 10;
		public const string HexCode = "0123456789ABCDEF";
		public const char Seperator = (char)1;

		public class Framework
		{
			public const string Name = "Kuick";
		}

		public class Application
		{
			public const string Name = "Kuick App";
		}

		public class Global
		{
			public const string AppID = "GlobalApp";
		}

		public class Assembly
		{
			public const string Version = "Version";
			public const string PublicKeyToken = "PublicKeyToken";
			public const string Culture = "Culture";
		}

		public class Xml
		{
			public const string Namespace = "http://kuicker.org/";
		}

		public class Error
		{
			public class Message
			{
				public const string InvalidType = "invalid type";
			}
		}

		public class StringBoolean
		{
			public const string True = "True";
			public const string False = "False";
			public const string Yes = "Yes";
			public const string No = "No";
		}

		public class Date
		{
			public static readonly DateTime Max = new DateTime(9999, 12, 31, 23, 59, 59, 0);
			public static readonly DateTime Min = new DateTime(1753, 1, 1, 0, 0, 0, 0);
			public static readonly DateTime Null = Min;
		}

		public class Null
		{
			public const string Uuid = "00000000000000000000000000000000";
			public const int Integer = 0;
			public const string StringInteger = "0";
			public static readonly string String = string.Empty;
			public static readonly DateTime Date = Constants.Date.Null;
			public const string JsonArray =
				Constants.Symbol.OpenBracket + Constants.Symbol.CloseBracket;
			public const string Json =
				Constants.Symbol.OpenBrace + Constants.Symbol.CloseBrace;
			public const string Ip = "0.0.0.0";
		}

		public class Protocol
		{
			public const string Ldap = "LDAP://";
			public const string WinNt = "WinNT://";
			public static readonly string Http = Uri.UriSchemeHttp + "://";
			public static readonly string Https = Uri.UriSchemeHttps + "://";
			public static readonly string Ftp = Uri.UriSchemeFtp + "://";
		}

		public class HttpMethod
		{
			public const string Get = "GET";
			public const string Head = "HEAD";
			public const string Post = "POST";
			public const string Put = "PUT";
			public const string Delete = "DELETE";
			public const string Trace = "TRACE";
			public const string Options = "OPTIONS";
		}

		public class Prefix
		{
			public const string Encrypted = "Encrypted:";
			public const string Namespace = "Kuick";
			public const string PropertyGet = "get_";
			public const string PropertySet = "set_";
		}

		public class Pattern
		{
			public const string Email = @"^\w+([-+.']\w*)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
			public const string EmailSeaperateByComma = @"^(\w+([-+.']\w*)*@\w+([-.]\w+)*\.\w+([-.]\w+)*,\s*)*\w+([-+.']\w*)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

			public const string Ip = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

			public const string Alphabet = @"[^a-zA-Z]*";
			public const string Numeric = @"[^0-9]*";
			public const string AlphaNumeric = @"[^a-zA-Z0-9]*";
			public const string Uri = @"^(?:http|https|ftp)://[a-zA-Z0-9\.\-]+(?:\:\d{1,5})?(?:[A-Za-z0-9\.\;\:\@\&\=\+\$\,\?/]|%u[0-9A-Fa-f]{4}|%[0-9A-Fa-f]{2})*$";
			public const string Html = @"<\/?[^>]+>";

			public const string Uuid = @"^[A-F0-9]{32}$";
		}

		public class Symbol
		{
			public const string Space = " ";
			public const string At = "@";
			public const string Asterisk = "*";
			public const string Ellipsis = "…";
			public const string Tilde = "~";
			public const string Percent = "%";
			public const string Pound = "#";
			public const string GreaterThan = ">";
			public const string LessThan = "<";
			public const string Exclamation = "!";
			public const string Plus = "+";
			public const string Dollar = "$";
			public const string Caret = "^";
			public const string Ampersand = "&";
			public const string OpenParenthesis = "(";
			public const string CloseParenthesis = ")";
			public const string OpenBracket = "[";
			public const string CloseBracket = "]";
			public const string OpenBrace = "{";
			public const string CloseBrace = "}";
			public const string Colon = ":";
			public const string Semicolon = ";";
			public const string Comma = ",";
			public const string Quotation = "\"";
			public const string Quoter = "'";
			public const string BackQuoter = "`";
			public const string Slash = "/";
			public const string BackwardSlash = "\\";
			public const string Equal = "=";
			public const string Period = ".";
			public const string Minus = "-";
			public const string UnderScore = "_";
			public const string Tab = "\t";
			public const string NewLine = "\n";
			public const string CarriageReturn = "\r";
			public const string Question = "?";
			public const string Pipe = "|";

			public static class Char
			{
				public const char Space = ' ';
				public const char At = '@';
				public const char Asterisk = '*';
				public const char Ellipsis = '…';
				public const char Tilde = '~';
				public const char Percent = '%';
				public const char Pound = '#';
				public const char GreaterThan = '>';
				public const char LessThan = '<';
				public const char Exclamation = '!';
				public const char Plus = '+';
				public const char Dollar = '$';
				public const char Caret = '^';
				public const char Ampersand = '&';
				public const char OpenParenthesis = '(';
				public const char CloseParenthesis = ')';
				public const char OpenBracket = '[';
				public const char CloseBracket = ']';
				public const char OpenBrace = '{';
				public const char CloseBrace = '}';
				public const char Colon = ':';
				public const char Semicolon = ';';
				public const char Comma = ',';
				public const char Quotation = '"';
				public const char Quoter = '\'';
				public const char BackQuoter = '`';
				public const char Slash = '/';
				public const char BackwardSlash = '\\';
				public const char Equal = '=';
				public const char Period = '.';
				public const char Minus = '-';
				public const char UnderScore = '_';
				public const char Tab = '\t';
				public const char NewLine = '\n';
				public const char CarriageReturn = '\r';
				public const char Question = '?';
				public const char Pipe = '|';
			}
		}

		public class Json
		{
			public static class Null
			{
				public const string Object = Constants.Null.Json;
				public const string Array = Constants.Null.JsonArray;
			}
		}

		public class Default
		{
			public const string Universal = "K";
			public const string Category = "DefaultCategory";
			public const string Group = "DefaultGroup";
			public const string Path = "DefaultPath";
			public const string Name = "Default";
			public const long Ticks = OneSecondTicks * 60;
			public const string Locale = "en-US";
			public static readonly CultureInfo Culture = new CultureInfo(Locale);
			public const string UserLanguage = "en-us";
			public const string SmtpServer = "localhost";
			public static readonly Guid Guid =
				new Guid("11111111-1111-1111-1111-111111111111");
			public static byte[] ByteArray = new Byte[0];
			public const string SystemDirectoryLetter = "C";
			public const string MachineNameSymbol = ".";
			public const char Separator = ';';
			public const char Connector = '=';
			public const string Joiner = ", ";
		}

		public class File
		{
			public static class Extension
			{
				public const string LOG = ".log";
				public const string XML = ".xml";
				public const string DLL = ".dll";
				public const string CONFIG = ".config";
				public const string DOC = ".doc";
				public const string EXE = ".exe";
				public const string PDF = ".pdf";
				public const string AI = ".ai";
				public const string EPS = ".eps";
				public const string PS = ".ps";
				public const string XLS = ".xls";
				public const string PPT = ".ppt";
				public const string JS = ".js";
				public const string SWF = ".swf";
				public const string XHTML = ".xhtml";
				public const string XHT = ".xht";
				public const string ZIP = ".zip";
				public const string MID = ".mid";
				public const string MIDI = ".midi";
				public const string MPGA = ".mpga";
				public const string MP2 = ".mp2";
				public const string MP3 = ".mp3";
				public const string M3U = ".m3u";
				public const string RAM = ".ram";
				public const string RM = ".rm";
				public const string RA = ".ra";
				public const string WAV = ".wav";
				public const string BMP = ".bmp";
				public const string GIF = ".gif";
				public const string JPEG = ".jpeg";
				public const string JPG = ".jpg";
				public const string JPE = ".jpe";
				public const string PNG = ".png";
				public const string TIFF = ".tiff";
				public const string TIF = ".tif";
				public const string CSS = ".css";
				public const string HTML = ".html";
				public const string HTM = ".htm";
				public const string TXT = ".txt";
				public const string RTX = ".rtx";
				public const string WML = ".wml";
				public const string WMLS = ".wmls";
				public const string XSL = ".xsl";
				public const string MPEG = ".mpeg";
				public const string MPG = ".mpg";
				public const string MPE = ".mpe";
				public const string QT = ".qt";
				public const string MOV = ".mov";
				public const string AVI = ".avi";
				public const string ASP = ".asp";
				public const string ASPX = ".aspx";
				public const string ASHX = ".ashx";
				public const string ASMX = ".asmx";
				public const string SVC = ".svc";
			}
		}

		public class Interval
		{
			public const int Minute = 60 * 1;
			public const int Hour = 60 * 60;
			public const int Day = 60 * 60 * 24;
			public const int Week = 60 * 60 * 24 * 7;
			public const int Month = 60 * 60 * 24 * 30;
			public const int Year = 60 * 60 * 24 * 365;
			public const int Forever = -1;
		}

		public class Html
		{
			public static class Entity
			{
				public const string nbsp = " ";
				public const string amp = "&";
				public const string gt = ">";
				public const string lt = "<";
			}

			public static class Escape
			{
				public const string nbsp = "&nbsp;";
				public const string amp = "&amp";
				public const string gt = "&gt;";
				public const string lt = "&lt;";
			}

			public static class Enctype
			{
				public static string MultiPart = "multipart/form-data";
			}

			public static class ControlPrefix
			{
				public static readonly string[] All = new string[] {
					Checkbox, DropDownList, TextBox
				};

				public const string Checkbox = "chk";     // bool
				public const string DropDownList = "ddl"; // string
				public const string TextBox = "txt";      // string
			}
		}

		public class DotNetFramework
		{
			public static readonly Version Net40 = new Version(4, 0, 30319, 1);
			public static readonly Version Net35 = new Version(3, 5, 21022, 8);
			public static readonly Version Net35SP1 = new Version(3, 5, 30729, 1);
			public static readonly Version Net30 = new Version(3, 0, 4506, 30);
			public static readonly Version Net30SP1 = new Version(3, 0, 4506, 648);
			public static readonly Version Net30SP2 = new Version(3, 0, 4506, 2152);
			public static readonly Version Net20 = new Version(2, 0, 50727, 42);
			public static readonly Version Net20SP1 = new Version(2, 0, 50727, 1433);
			public static readonly Version Net20SP2 = new Version(2, 0, 50727, 3053);
			public static readonly Version Net11 = new Version(1, 1, 4322, 573);
			public static readonly Version Net11SP1 = new Version(1, 1, 4322, 2032);
			public static readonly Version Net11SP1WIN2003 = new Version(1, 1, 4322, 2300);
			public static readonly Version Net10 = new Version(1, 0, 3705, 0);
			public static readonly Version Net10SP1 = new Version(1, 0, 3705, 209);
			public static readonly Version Net10SP2 = new Version(1, 0, 3705, 288);
			public static readonly Version Net10SP3 = new Version(1, 0, 3705, 6018);
		}

		public class ContentType
		{
			public const string Xml = MediaTypeNames.Text.Xml;
			public const string Rss = MediaTypeNames.Text.Xml;
			public const string Json = "application/json";
		}

		public class Ip
		{
			public const string LoopBack = LocalIp;
			public const long LoopBackAsLong = 2130706433;

			public const string Max = "255.255.255.255";
			public const string Min = "0.0.0.0";

			public const long MaxAsLong = 4294967295;
			public const long MinAsLong = 0;

			public static class Private
			{
				public static class ClassA
				{
					public const string Start = "10.0.0.0";
					public const string End = "10.255.255.255";
					public const long StartAsLong = 167772160;
					public const long EndAsLong = 184549375;
				}

				public static class ClassB
				{
					public const string Start = "172.16.0.0";
					public const string End = "172.31.255.255";
					public const long StartAsLong = 2886729728;
					public const long EndAsLong = 2887778303;
				}

				public static class ClassC
				{
					public const string Start = "192.168.0.0";
					public const string End = "192.168.255.255";
					public const long StartAsLong = 3232235520;
					public const long EndAsLong = 3232301055;
				}

				public static class Loopback
				{
					public const string Start = "127.0.0.0";
					public const string End = "127.255.255.255";
					public const long StartAsLong = 2130706432;
					public const long EndAsLong = 2147483647;
				}
			}
		}

		public class Bom
		{
			public static readonly byte[] Utf8 = new byte[] { 0xEF, 0xBB, 0xBF };
			public static readonly byte[] Utf16l = new byte[] { 0xFF, 0xFE };
			public static readonly byte[] Utf16b = new byte[] { 0xFE, 0xFF };
			public static readonly byte[] Utf32l = new byte[] { 0xFF, 0xFE, 0x00, 0x00 };
			public static readonly byte[] Utf32b = new byte[] { 0x00, 0x00, 0xFE, 0xFF };
		}

		public class Folder
		{
			public const string Bin = "bin";
			public const string Log = "log";
			public const string Config = "config";
		}

		public class Authentication
		{
			public const string User = "USER@KUICK";
			public const string UserID = "USERID@KUICK";
		}

		public class Authorization
		{
		}

		public class Client
		{
			public const string Device = "Device@KUICK";
			public const string DeviceID = "DeviceID@KUICK";
		}
	}
}

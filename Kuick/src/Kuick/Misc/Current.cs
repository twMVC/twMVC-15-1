// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Current.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Web;

namespace Kuick
{
	public class Current
	{
		private const string DEFAULT_VOLUMNE_SERIAL = "8A8F0A88";       //  8 digits
		private const string DEFAULT_MAC_ADDRESS = "00:1A:2F:A2:DD:88"; // 12 digits
		private const string DEFAULT_CPU_ID = "AFABFAFF00020688";       // 16 digits
		private static object _Lock = new object();

		private static Int64 _Index = 0;
		public static Int64 Index
		{
			get
			{
				lock(_Lock) {
					return _Index++;
				}
			}
		}

		public static bool Released { get; internal set; } // ?

		public static bool IsWebApplication
		{
			get
			{
				return null != HttpContext.Current;
			}
		}

		private static string _BinFolder;
		public static string BinFolder
		{
			get
			{
				if(string.IsNullOrEmpty(_BinFolder)) {
					_BinFolder = IsWebApplication
						? WebBinFolder
						: AppDomain.CurrentDomain.BaseDirectory;
				}
				return _BinFolder;
			}
		}

		private static string _WebBinFolder;
		public static string WebBinFolder
		{
			get
			{
				if(string.IsNullOrEmpty(_WebBinFolder)) {
					_WebBinFolder = Path.Combine(
						AppDomain.CurrentDomain.BaseDirectory,
						Constants.Folder.Bin
					);
				}
				return _WebBinFolder;
			}
		}

		private static string _EncryptKey;
		public static string EncryptKey
		{
			get
			{
				if(null == _EncryptKey) {
					_EncryptKey = BuildEncryptKey();
				}
				return _EncryptKey;
			}
		}
		public static bool Started
		{
			get
			{
				return Heartbeat.Singleton.Stage == KernelStage.Running;
			}
		}

		/// <summary>
		/// Mode
		/// </summary>
		/// <remarks>
		/// In order to avoid config error only compare the first character.
		/// </remarks>
		public static KernelMode Mode
		{
			get
			{
				string mode = Config.Singleton.Kernel.GetString("Mode");
				string start = string.IsNullOrEmpty(mode) 
					? "d" 
					: mode.Substring(0, 1).ToLower();

				switch(start) {
					case "r":
						return KernelMode.Released;
					case "t":
						return KernelMode.Testing;
					case "d":
						return KernelMode.Developing;
					default:
						return KernelMode.Developing;
				}

				//return Config.Singleton.Kernel.GetEnum<KernelMode>(
				//    "Mode", KernelMode.Developing
				//);
			}
		}

		/// <summary>
		/// AppId
		/// </summary>
		public static string AppID
		{
			get
			{
				return Config.Singleton.Kernel.GetString("AppID");
			}
		}

		/// <summary>
		/// AppName
		/// </summary>
		public static string AppName
		{
			get
			{
				return Config.Singleton.Kernel.GetString("AppName", AppID);
			}
		}

		/// <summary>
		/// Is Front End
		/// </summary>
		public static bool IsFrontEnd
		{
			get
			{
				return Config.Singleton.Kernel.GetBoolean("IsFrontEnd");
			}
		}

		/// <summary>
		/// Language
		/// </summary>
		public static string Language
		{
			get
			{
				return Config.Singleton.Kernel.GetString(
					"Language", "en-US"
				);
			}
		}

		public static string DomainName
		{
			get
			{
				return Environment.UserDomainName;
			}
		}

		public static string UserName
		{
			get
			{
				if(IsWebApplication) {
					if(
						null != HttpContext.Current &&
						null != HttpContext.Current.User &&
						null != HttpContext.Current.User.Identity) {
						return HttpContext.Current.User.Identity.Name;
					} else {
						return "Anonymous";
					}
				} else {
					return Environment.UserName;
				}
			}
		}

		/// <summary>
		/// OperatingSystem Bits: 32/64 bit.
		/// </summary>
		public static Bits OperatingSystemBits
		{
			get
			{
				return Environment.Is64BitOperatingSystem
					? Bits.x64
					: Bits.x86;
			}
		}

		/// <summary>
		/// Processor Bits: 32/64 bit.
		/// </summary>
		public static Bits ProcessorBits
		{
			get
			{
				return Environment.Is64BitProcess
					? Bits.x64
					: Bits.x86;
			}
		}

		/// <summary>
		/// OS Version
		/// </summary>
		public static string OSVersion
		{
			get
			{
				return Environment.OSVersion.VersionString;
			}
		}

		private static string[] _OnlyAssemblies;
		/// <summary>
		/// Only load specific assemblies
		/// </summary>
		/// <example>
		/// web.config / app.config
		/// <![CDATA[
		/// <configuration>
		///     <Kuick>
		///         <kernel>
		///             <add name="OnlyAssemblies" value="Assembly1, Assembly2"/>
		///         </kernel>
		///     </Kuick>
		/// </configuration>
		/// ]]>
		/// </example>
		public static string[] OnlyAssemblies
		{
			get
			{
				if(null == _OnlyAssemblies) {
					string s = Config.Singleton.Kernel.GetString("OnlyAssemblies");
					_OnlyAssemblies = string.IsNullOrEmpty(s)
						? new string[0]
						: new List<string>(s.SplitWith(",")).ToArray();
				}
				return _OnlyAssemblies;
			}
		}

		private static string[] _SkipAssemblies;
		/// <summary>
		/// specific skip assemblies
		/// </summary>
		/// <example>
		/// web.config / app.config
		/// <![CDATA[
		/// <configuration>
		///     <Kuick>
		///         <kernel>
		///             <add name="SkipAssemblies" value="NamePrefix1, NamePrefix2"/>
		///         </kernel>
		///     </Kuick>
		/// </configuration>
		/// ]]>
		/// </example>
		public static string[] SkipAssemblies
		{
			get
			{
				if(null == _SkipAssemblies) {
					string s = Config.Singleton.Kernel.GetString("SkipAssemblies");
					_SkipAssemblies = string.IsNullOrEmpty(s)
						? new string[0]
						: new List<string>(s.SplitWith(",")).ToArray();
				}
				return _SkipAssemblies;
			}
		}

		public class Setting
		{
			private const string Group = "Setting";

			public static bool Enable
			{
				get
				{
					return Config.Singleton.Application.GetBoolean(
						Group, "Enable", true
					);
				}
			}

			public static string Folder
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "Folder", "setting"
					);
				}
			}

			public static string FolderPath
			{
				get;
				internal set;
			}
		}

		public class Impersonator
		{
			private const string Group = "Impersonator";

			public static bool Enable
			{
				get
				{
					return Config.Singleton.Application.GetBoolean(
						Group, "Enable", false
					);
				}
			}
			public static string DomainName
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "DomainName", Environment.UserDomainName
					);
				}
			}
			public static string UserName
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "UserName", Environment.UserName
					);
				}
			}
			public static string Password
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "Password", string.Empty
					);
				}
			}
		}

		public class Smtp
		{
			private const string Group = "Smtp";

			public static string Server
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "Server", "http://127.0.0.1"
					);
				}
			}
			public static string UserName
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "Credential:UserName", Environment.UserName
					);
				}
			}
			public static string Password
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "Credential:Password", string.Empty
					);
				}
			}
		}

		public class Mail
		{
			private const string Group = "Mail";

			public static string From
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "From", string.Empty
					);
				}
			}
		}

		public class Log
		{
			private const string Group = "Log";

			public static bool Enable
			{
				get
				{
					return Config.Singleton.Application.GetBoolean(
						Group, "Enable", true
					);
				}
			}

			public static string Folder
			{
				get
				{
					return Config.Singleton.Application.GetString(
						Group, "Folder", "log"
					);
				}
			}

			public static string FolderPath
			{
				get;
				internal set;
			}

			/// <summary>
			/// Log file split critical size (in KB)
			/// </summary>
			public static int RefreshSize
			{
				get
				{
					return Config.Singleton.Application.GetInteger(
						Group, "RefreshSize", -1
					);
				}
			}

			public static Frequency RefreshFrequency
			{
				get
				{
					return Config.Singleton.Application.GetEnum<Frequency>(
						Group, "RefreshFrequency", Frequency.Hourly
					);
				}
			}
		}

		public static ConfigSection<ConfigSetting> Kernel
		{
			get
			{
				return Config.Singleton.Kernel;
			}
		}

		public static ConfigSection<ConfigSetting> Application
		{
			get
			{
				return Config.Singleton.Application;
			}
		}

		public static ConfigSection<ConfigDatabase> Database
		{
			get
			{
				return Config.Singleton.Database;
			}
		}

		#region Alias
		public static ConfigSection<ConfigSetting> App
		{
			get
			{
				return Application;
			}
		}
		public static ConfigSection<ConfigDatabase> Db
		{
			get
			{
				return Database;
			}
		}
		#endregion

		#region internal
		internal static string BuildEncryptKey()
		{
			return BuildHardwareKey(
				DEFAULT_VOLUMNE_SERIAL,
				DEFAULT_MAC_ADDRESS,
				DEFAULT_CPU_ID
			);
		}

		internal static string BuildHardwareKey()
		{
			try {
				string volumneSerial = GetSystemDirectoryVolumeSerial();
				string macAddress = GetMacAddresses()[0];
				string cpuID = GetCpuIds()[0];
				return BuildHardwareKey(volumneSerial, macAddress, cpuID);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Current.BuildHardwareKey()",
					ex.ToAny()
				);
				return BuildHardwareKey(
					DEFAULT_VOLUMNE_SERIAL,
					DEFAULT_MAC_ADDRESS,
					DEFAULT_CPU_ID
				);
			}
		}

		internal static string BuildHardwareKey(
			string volumneSerial,
			string macAddress,
			string cpuID)
		{
			char[] x = Formator
				.AirBag(volumneSerial, DEFAULT_VOLUMNE_SERIAL)
				.ToCharArray();
			char[] y = Formator
				.AirBag(macAddress, DEFAULT_MAC_ADDRESS)
				.Replace(Constants.Symbol.Colon, string.Empty)
				.ToCharArray();
			char[] z = Formator
				.AirBag(cpuID, DEFAULT_CPU_ID)
				.ToCharArray();

			int xLen = x.Length;
			int yLen = y.Length;
			int zLen = z.Length;

			return String.Format(
				"{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}",
				x[03 % xLen],
				y[09 % yLen],
				z[08 % zLen],
				y[11 % yLen],
				z[13 % zLen],
				z[04 % zLen],
				x[07 % xLen],
				y[03 % yLen],
				z[10 % zLen],
				y[06 % yLen],
				z[02 % zLen],
				z[14 % zLen],
				x[01 % xLen],
				y[10 % yLen],
				z[02 % zLen],
				y[00 % yLen]
			);
		}
		#endregion

		#region private
		private static string GetSystemDirectoryVolumeSerial()
		{
			try {
				string letter = Formator.AirBag(
					Environment.SystemDirectory.Split(
						Constants.Symbol.Char.Colon
					)[0].ToString(),
					Constants.Default.SystemDirectoryLetter
				);

				ManagementObject disk = new ManagementObject(
					String.Format("win32_logicaldisk.deviceid=\"{0}:\"", letter)
				);
				disk.Get();
				return disk["VolumeSerialNumber"].ToString();
			} catch(Exception) {
			}
			return string.Empty;
		}

		private static string[] GetMacAddresses()
		{
			ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection moc = mc.GetInstances();
			List<string> list = new List<string>();
			foreach(ManagementObject mo in moc) {
				if(true == (bool)mo["IPEnabled"]) {
					list.Add(mo["MacAddress"].ToString());
				}
				mo.Dispose();
			}
			return list.ToArray();
		}

		private static string[] GetCpuIds()
		{
			ManagementClass mc = new ManagementClass("Win32_Processor");
			ManagementObjectCollection moc = mc.GetInstances();
			List<string> list = new List<string>();
			foreach(ManagementObject mo in moc) {
				list.Add(mo.Properties["ProcessorId"].Value.ToString());
				mo.Dispose();
			}
			return list.ToArray();
		}
		#endregion
	}
}
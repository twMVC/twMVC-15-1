// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ForceImpersonator.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Kuick
{
	public class ForceImpersonator : IDisposable
	{
		private static Dictionary<string, LogonCache> _LogonCaches =
			new Dictionary<string, LogonCache>();
		private static object _Lock = new object();

		public event EventHandler Dealing;

		private WindowsImpersonationContext _Wic;

		public ForceImpersonator(string userName, string domainName, string password)
		{
			this.UserName = userName;
			this.DomainName = domainName;
			this.Password = password;
			this.Anys = new Anys();
		}

		#region IForceImpersonator
		public string UserName { get; private set; }
		public string DomainName { get; private set; }
		private string Password { get; set; }
		public Anys Anys { get; private set; }

		public bool Deal()
		{
			bool success = false;
			LogonType currentLogonType = LogonType.LOGON32_LOGON_BATCH;
			LogonProvider currentLogonProvider = LogonProvider.LOGON32_PROVIDER_DEFAULT;

			IntPtr logonToken = IntPtr.Zero;
			IntPtr logonTokenDuplicate = IntPtr.Zero;

			LogonCache cache = new LogonCache();
			cache.UserName = UserName;
			cache.Password = Password;
			cache.DomainName = DomainName;

			// from cache
			if(_LogonCaches.TryGetValue(cache.ToString(), out cache)) {
				UndoImpersonation();
				try {
					_Wic = WindowsIdentity.Impersonate(IntPtr.Zero);

					success = DealOne(
						cache.SuccessType,
						cache.SuccessProvider,
						ref logonToken,
						ref logonTokenDuplicate
					);
				} catch(Exception ex) {
					// Remove from _LogonCaches
					if(_LogonCaches.ContainsKey(cache.ToString())) {
						lock(_Lock) {
							if(_LogonCaches.ContainsKey(cache.ToString())) {
								_LogonCaches.Remove(cache.ToString());
							}
						}
					}

					// log
					Logger.Track(
						"ForceImpersonator: Wrong Cache!",
						ex.ToAny(BuildAnys(cache.SuccessType, cache.SuccessProvider))
					);
				} finally {
					CloseHandle(ref logonToken, ref logonTokenDuplicate);
				}
			}

			Array logonTypes = Enum.GetValues(typeof(LogonType));
			Array logonProviders = Enum.GetValues(typeof(LogonProvider));

			// try
			foreach(LogonType logonType in logonTypes) {
				currentLogonType = logonType;
				foreach(LogonProvider logonProvider in logonProviders) {
					currentLogonProvider = logonProvider;

					UndoImpersonation();

					try {
						_Wic = WindowsIdentity.Impersonate(IntPtr.Zero);

							Logger.Track(
								"ForceImpersonator: Already logged!",
								BuildAnys(logonType, logonProvider)
							);

						try {
							success = DealOne(
								logonType,
								logonProvider,
								ref logonToken,
								ref logonTokenDuplicate
							);
						} catch(Exception exInner) {
								Logger.Track(
									"ForceImpersonator: Dealing failed!",
									exInner.ToAny(BuildAnys(logonType, logonProvider))
								);
						}
					} catch(Exception ex) {
							Logger.Track(
								"ForceImpersonator: Attempt failed!",
								ex.ToAny(BuildAnys(logonType, logonProvider))
							);
					} finally {
						CloseHandle(ref logonToken, ref logonTokenDuplicate);
					}
					if(success) { break; }
				}
				if(success) { break; }
			}

			if(success) {
				if(!_LogonCaches.ContainsKey(cache.ToString())) {
					lock(_Lock) {
						if(!_LogonCaches.ContainsKey(cache.ToString())) {
							cache.SuccessType = currentLogonType;
							cache.SuccessProvider = currentLogonProvider;
							_LogonCaches.Add(cache.ToString(), cache);
						}
					}
				}
			} else {
				Logger.Error(
					"ForceImpersonator: Failure!",
					BuildAnys()
				);
			}

			return success;
		}
		#endregion

		private bool DealOne(
			LogonType logonType,
			LogonProvider logonProvider,
			ref IntPtr logonToken,
			ref IntPtr logonTokenDuplicate)
		{
			bool success = false;

			if(Win32NativeMethods.LogonUser(
				UserName,
				DomainName,
				Password,
				(int)logonType,
				(int)logonProvider,
				ref logonToken) != 0) {

				if(Win32NativeMethods.DuplicateToken(
					logonToken,
					(int)ImpersonationLevel.SecurityImpersonation,
					ref logonTokenDuplicate) != 0) {

					var wi = new WindowsIdentity(logonTokenDuplicate);
					wi.Impersonate();

					// delegate
					if(null != Dealing) { Dealing(this, EventArgs.Empty); }
					success = true;

						Logger.Track(
							"ForceImpersonator: Attempt is successful!",
							BuildAnys(logonType, logonProvider)
						);
				} else {
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			} else {
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return success;
		}

		private Any[] BuildAnys()
		{
			return new Any[]{
				new Any("DomainName", DomainName),
				new Any("UserName", UserName),
				MarkPassword(Password)
			};
		}

		private Any[] BuildAnys(LogonType logonType, LogonProvider logonProvider)
		{
			return new Any[]{
				new Any("DomainName", DomainName),
				new Any("UserName", UserName),
				MarkPassword(Password),
				new Any("LogonType", logonType),
				new Any("LogonProvider", logonProvider)
			};
		}

		private void CloseHandle(ref IntPtr logonToken, ref IntPtr logonTokenDuplicate)
		{
			if(logonToken != IntPtr.Zero) {
				Win32NativeMethods.CloseHandle(logonToken);
			}
			if(logonTokenDuplicate != IntPtr.Zero) {
				Win32NativeMethods.CloseHandle(logonTokenDuplicate);
			}
		}

		private Any MarkPassword(string password)
		{
			return new Any(
				"Password",
				Current.Mode == KernelMode.Developing
					? Password
					: Formator.Repeater(Constants.Symbol.Asterisk, Password.Length)
			);
		}

		private void UndoImpersonation()
		{
			if(_Wic != null) { _Wic.Undo(); }
			_Wic = null;
		}

		#region IDisposable
		public void Dispose()
		{
			UndoImpersonation();
		}
		#endregion

		#region inner class or enum
		private class LogonCache : IEquatable<LogonCache>
		{
			public LogonCache() { }
			public string UserName { get; set; }
			public string DomainName { get; set; }
			public string Password { get; set; }
			public LogonType SuccessType { get; set; }
			public LogonProvider SuccessProvider { get; set; }

			public override string ToString()
			{
				return string.Format("{0}, {1}, {2}", DomainName, UserName, Password);
			}

			#region IEquatable<LogonCache>
			public bool Equals(LogonCache other)
			{
				return this.ToString() == other.ToString();
			}
			#endregion
		}

		private enum LogonType
		{
			LOGON32_LOGON_INTERACTIVE = 2,
			LOGON32_LOGON_NETWORK = 3,
			LOGON32_LOGON_BATCH = 4,
			LOGON32_LOGON_SERVICE = 5,
			LOGON32_LOGON_UNLOCK = 7,
			LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // Win2K or higher
			LOGON32_LOGON_NEW_CREDENTIALS = 9 // Win2K or higher
		};

		private enum LogonProvider
		{
			LOGON32_PROVIDER_DEFAULT = 0,
			LOGON32_PROVIDER_WINNT35 = 1,
			LOGON32_PROVIDER_WINNT40 = 2,
			LOGON32_PROVIDER_WINNT50 = 3
		};

		private enum ImpersonationLevel
		{
			SecurityAnonymous = 0,
			SecurityIdentification = 1,
			SecurityImpersonation = 2,
			SecurityDelegation = 3
		}

		private class Win32NativeMethods
		{
			[DllImport("advapi32.dll", SetLastError = true)]
			public static extern int LogonUser(
				string lpszUserName,
				string lpszDomain,
				string lpszPassword,
				int dwLogonType,
				int dwLogonProvider,
				ref IntPtr phToken
			);

			[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern int DuplicateToken(
				IntPtr hToken,
				int impersonationLevel,
				ref IntPtr hNewToken
			);

			[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool RevertToSelf();

			[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
			public static extern bool CloseHandle(IntPtr handle);
		}
		#endregion
	}
}

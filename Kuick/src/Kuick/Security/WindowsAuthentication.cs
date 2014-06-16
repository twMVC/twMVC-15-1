// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WindowsAuthentication.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Kuick
{
	public class WindowsAuthentication
	{
		#region PInvoke
		// const
		internal const int LOGON32_PROVIDER_DEFAULT = 0;
		internal const int LOGON32_LOGON_INTERACTIVE = 2;
		internal const int LOGON32_LOGON_NETWORK = 3;
		internal const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

		// DllImport
		[DllImport("advapi32.Dll", SetLastError = true)]
		internal static extern bool LogonUser(
			String lpszUsername,
			String lpszDomain,
			String lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken
		);

		// DllImport
		[DllImport("kernel32.Dll", CharSet = CharSet.Auto)]
		internal static extern bool CloseHandle(IntPtr handle);

		// DllImport
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private unsafe static extern int FormatMessage(
			int dwFlags,
			ref IntPtr lpSource,
			int dwMessageId,
			int dwLanguageId,
			ref string lpBuffer,
			int nSize,
			IntPtr* Arguments
		);

		// DllImport
		[DllImport("advapi32.dll")]
		public static extern int LogonUserA(
			String lpszUserName,
			String lpszDomain,
			String lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken
		);

		// DllImport
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DuplicateToken(IntPtr hToken,
			int impersonationLevel,
			ref IntPtr hNewToken
		);

		// DllImport
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool RevertToSelf();
		#endregion

		private string[] _Roles;
		private WindowsImpersonationContext _ImpersonationContext;

		public WindowsAuthentication()
		{
		}

		#region IAuthentication
		public bool Login(string userName, string password)
		{
			// email or username check
			if(userName.Contains("@")) {
				DirectoryEntry[] des = WinActiveDirectory.GetEntriesByEmail(userName);
				if(Checker.IsNull(des)) {
					Logger.Error(
						"WindowsAuthentication.Login",
						"Can't find anyone user account by this email.",
						new Any("email", userName)
					);
					return false;
				}

				if(des.Length > 1) {
					Logger.Error(
						"WindowsAuthentication.Login",
						"More than one user account was found by this email.",
						new Any("email", userName)
					);
					return false;
				}

				userName = WinActiveDirectory.GetSAMAccountName(des[0]);
			}

			try {
				WindowsIdentity id = GetIdentity(userName, password);
				if(null == id || !id.IsAuthenticated) { return false; }

				// Roles
				ArrayList list = new ArrayList();
				IdentityReferenceCollection groups = id.Groups;
				foreach(IdentityReference group in groups) {
					list.Add(group.Value);
				}
				_Roles = list.ToArray(typeof(string)) as string[];

				return true;
			} catch(Exception ex) {
				Logger.Error(
					"WindowsAuthentication.Login: " + ex.Message, ex.StackTrace
				);
				return false;
			}
		}

		public string[] GetRoles(string userName)
		{
			return _Roles;
		}
		#endregion

		#region Impersonate
		public bool Impersonate(
			string domain, string userName, string password)
		{
			string msg = string.Empty;
			return Impersonate(domain, userName, password, ref msg);
		}

		public bool Impersonate(
			string domain, string userName, string password, ref string message)
		{
			return Impersonate(domain, userName, password, ref message, 9);
		}

		public bool Impersonate(
			string domain, 
			string userName, 
			string password, 
			ref string message, 
			int way)
		{
			WindowsIdentity tempWindowsIdentity;
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			if(RevertToSelf()) {
				if(LogonUserA(
					userName,
					domain,
					password,
					way,//LOGON32_LOGON_NEW_CREDENTIALS,
					LOGON32_PROVIDER_DEFAULT,
					ref token
					) != 0) {
					if(
						DuplicateToken(token, 2, ref tokenDuplicate)
						!=
						0) {
						tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
						_ImpersonationContext = tempWindowsIdentity.Impersonate();
						if(null != _ImpersonationContext) {
							CloseHandle(token);
							CloseHandle(tokenDuplicate);
							return true;
						}
					}
				}
			}

			int ret = Marshal.GetLastWin32Error();
			if(ret == 0) { return true; }
			message = GetErrorMessage(ret);

			if(token != IntPtr.Zero) { CloseHandle(token); }
			if(tokenDuplicate != IntPtr.Zero) { CloseHandle(tokenDuplicate); }
			return false;
		}

		public void Undo()
		{
			if(null != _ImpersonationContext) { _ImpersonationContext.Undo(); }
		}

		// sample
		private void DoSomeCritical()
		{
			if(Impersonate("username", "domain", "password")) {
				// Insert your code that runs under the security context 
				// of a specific user.
				Undo();
			} else {
				// Your impersonation failed. 
				// Therefore, include a fail-safe mechanism.
			}
		}
		#endregion

		public WindowsIdentity GetIdentity(string userName, string password)
		{
			if(userName.IsNullOrEmpty()) {
				throw new ArgumentException("'userName' is null or empty.");
			}
			if(password.IsNullOrEmpty()) {
				throw new ArgumentException("'password' is null or empty.");
			}

			try {
				string domain = string.Empty;
				string user = string.Empty;
				int pos = userName.IndexOf('\\');
				if(pos == -1) {
					if(Current.DomainName == "NT AUTHORITY") {
						throw new InvalidOperationException(
							"Can't read the value of 'Environment.UserDomainName'."
						);
					}

					domain = Current.DomainName;
					user = userName;
				} else {
					domain = userName.Substring(0, pos);
					user = userName.Substring(pos + 1);
				}

				IntPtr tokenHandle = new IntPtr(0);
				tokenHandle = IntPtr.Zero;

				bool rtn = LogonUser(
					user,
					domain,
					password,
					LOGON32_LOGON_NETWORK,
					LOGON32_PROVIDER_DEFAULT,
					ref tokenHandle
				);

				if(false == rtn) {
					int ret = Marshal.GetLastWin32Error();
					Logger.Error(
						new Exception("LogonUser failed"),
						"WindowsAuthentication.GetIdentity",
						new Any("UserName", userName),
						new Any("Password", "*".Repeat(password.Length)),
						new Any("Error Message", GetErrorMessage(ret))
					);
					return null;
				}

				WindowsIdentity id = new WindowsIdentity(tokenHandle);
				CloseHandle(tokenHandle);

				return id;
			} catch(Exception ex) {
				Logger.Error(
					"WindowsAuthentication.GetIdentity: " + ex.Message, 
					ex.StackTrace
				);
				return null;
			}
		}

		public unsafe static string GetErrorMessage(int errorCode)
		{
			int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
			int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
			int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
			int messageSize = 1024;
			string lpMsgBuf = "";
			int dwFlags =
				FORMAT_MESSAGE_ALLOCATE_BUFFER
				|
				FORMAT_MESSAGE_FROM_SYSTEM
				|
				FORMAT_MESSAGE_IGNORE_INSERTS;

			IntPtr ptrlpSource = IntPtr.Zero;
			IntPtr prtArguments = IntPtr.Zero;

			int retVal = FormatMessage(
				dwFlags,
				ref ptrlpSource,
				errorCode,
				0,
				ref lpMsgBuf,
				messageSize,
				&prtArguments
			);

			if(0 == retVal) {
				lpMsgBuf = String.Format(
					"Failed to format message for error code {0}.",
					errorCode
				);
				Logger.Error( "WindowsAuthentication.GetErrorMessage", lpMsgBuf);
			}

			return lpMsgBuf;
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WinActiveDirectory.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace Kuick
{
	public class WinActiveDirectory
	{
		public const int MAX_SEARCHER_PAGE_SIZE = 10000;
		public const int MAX_SEARCHER_SIZE_LIMIT = 10000;

		private const string ROOT_DSE = Constants.Protocol.Ldap + "RootDSE";
		private const string DEFAULT_NAMING_CONTEXT = "defaultNamingContext";

		#region public
		public static string GetDefaultNamingContextPath()
		{
			DirectoryEntry de = GetDefaultNamingContextEntry();
			if(null == de) {
				throw new ApplicationException(
					"Can't get the default naming context entry."
				);
			}
			string ldap = CollToString(de.Properties, DEFAULT_NAMING_CONTEXT);
			return CorrectADsPath(ldap);
		}

		public static DirectoryEntry GetDefaultNamingContextEntry()
		{
			DirectoryEntry de = GetEntry();
			return de;
		}

		public static DirectoryEntry[] GetEntriesByEmail(string email)
		{
			DirectoryEntry de = GetDefaultNamingContextEntry();
			List<DirectoryEntry> al = new List<DirectoryEntry>();
			if(null == de) { return al.ToArray(); }
			using(DirectorySearcher searcher = new DirectorySearcher(de)) {
				searcher.ServerTimeLimit = new TimeSpan(1, 0, 0);
				searcher.PageSize = MAX_SEARCHER_PAGE_SIZE;
				searcher.SizeLimit = MAX_SEARCHER_SIZE_LIMIT;
				searcher.Filter = String.Format(
					"(&(&(objectCategory=person)(objectClass=user))(mail={0}))",
					email
				);

				var all = searcher.FindAll();
				if(null != all) {
					foreach(SearchResult searcherEntity in all) {
						DirectoryEntry entry = searcherEntity.GetDirectoryEntry();
						al.Add(entry);
					}
				}
				return al.ToArray();
			}
		}

		public static string GetSAMAccountName(DirectoryEntry entry)
		{
			if(null == entry) { return null; }
			string userName = CollToString(entry.Properties, "sAMAccountName");
			if(Checker.IsNull(userName)) { return null; }
			userName = userName.Replace("$", "");
			return userName;
		}

		public static DirectoryEntry GetEntry()
		{
			return GetEntry(string.Empty, string.Empty, string.Empty);
		}

		public static DirectoryEntry GetEntry(string adsPath)
		{
			return GetEntry(adsPath, string.Empty, string.Empty);
		}

		public static DirectoryEntry GetEntry(
			string adsPath, string userName, string password)
		{
			DirectoryEntry de = null;
			adsPath = Checker.IsNull(adsPath) ? ROOT_DSE : CorrectADsPath(adsPath);

			de = String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password)
				? new DirectoryEntry(adsPath)
				: new DirectoryEntry(adsPath, userName, password);

			if(adsPath == ROOT_DSE) {
				de = GetEntry(
					Constants.Protocol.Ldap +
						CollToString(de.Properties, DEFAULT_NAMING_CONTEXT),
					userName,
					password
				);
			}

			return de;
		}

		public static string CollToString(
			ResultPropertyCollection propColl, string property)
		{
			string rtn = string.Empty;

			try {
				foreach(object oColl in propColl[property]) {
					if(rtn.Length > 0) { rtn += "; "; }
					rtn += oColl.ToString();
				}
				return rtn;
			} catch {
				return string.Empty;
			}
		}

		public static string CollToString(
			PropertyCollection propColl, string property)
		{
			string rtn = string.Empty;

			try {
				foreach(object oColl in propColl[property]) {
					if(rtn.Length > 0) { rtn += "; "; }
					rtn += oColl.ToString();
				}
				return rtn;
			} catch {
				return string.Empty;
			}
		}
		#endregion

		#region private
		private static string CorrectADsPath(string adsPath)
		{
			if(adsPath.StartsWith(Constants.Protocol.Ldap)) { return adsPath; }
			return Constants.Protocol.Ldap + adsPath;
		}
		#endregion
	}
}

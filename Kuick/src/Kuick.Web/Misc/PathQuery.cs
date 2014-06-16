// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PathQuery.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;
using System.Web;
using System.Web.UI;

namespace Kuick.Web
{
	public class PathQuery
	{
		public PathQuery(Page page)
			: this(page.Request.RawUrl)
		{
		}

		public PathQuery(string pathAndQuery)
		{
			pathAndQuery = WebTools.ResolveUrl(pathAndQuery);
			int index = pathAndQuery.IndexOf(WebConstants.Symbol.Char.Question);
			this.Path = (index == -1) ? pathAndQuery : pathAndQuery.Substring(0, index);
			this.Anys = new Anys();
			string query = (index == -1) ? string.Empty : pathAndQuery.Substring(index + 1);
			if(!WebChecker.IsNull(query)) {
				Anys anys = new Anys(
					query,
					WebConstants.Symbol.Char.Ampersand,
					WebConstants.Symbol.Char.Equal
				);
				foreach(Any any in anys.ToArray()) {
					Anys.Add(any.Name, HttpContext.Current.Server.UrlDecode(any.ToString()));
				}
			}
		}

		public string Path { get; private set; }
		public Anys Anys { get; private set; }

		public bool SamePath(string path)
		{
			return Path == WebTools.ResolveUrl(path);
		}

		public static PathQuery operator +(PathQuery pathQuery, Any any)
		{
			pathQuery.Anys.Add(any);
			return pathQuery;
		}

		public static PathQuery operator -(PathQuery pathQuery, string queryName)
		{
			pathQuery.Anys.Remove(queryName);
			return pathQuery;
		}

		public static bool operator !=(PathQuery pathQuery, PathQuery pathQueryTo)
		{
			return pathQuery != pathQueryTo;
		}

		public static bool operator ==(PathQuery pathQuery, PathQuery pathQueryTo)
		{
			if(!pathQuery.Path.Equals(pathQueryTo.Path, StringComparison.OrdinalIgnoreCase)) {
				return false;
			}
			if(pathQuery.Anys.Count != pathQueryTo.Anys.Count) {
				return false;
			}
			foreach(Any any in pathQuery.Anys) {
				Any anyTo = pathQueryTo.Anys.Find(any.Name);
				if(WebChecker.IsNull(anyTo)) {
					return false;
				}
				if(!any.ToString().Equals(anyTo.ToString(), StringComparison.OrdinalIgnoreCase)) {
					return false;
				}
			}
			return true;
		}

		public static bool operator <=(PathQuery pathQuery, PathQuery pathQueryTo)
		{
			return pathQuery == pathQueryTo
				? true
				: pathQueryTo >= pathQuery;
		}

		public static bool operator >=(PathQuery pathQuery, PathQuery pathQueryTo)
		{
			if(!pathQuery.Path.Equals(pathQueryTo.Path, StringComparison.OrdinalIgnoreCase)) {
				return false;
			}
			foreach(Any anyTo in pathQueryTo.Anys) {
				Any any = pathQuery.Anys.Find(anyTo.Name);
				if(WebChecker.IsNull(any)) { return false; }
				if(!anyTo.ToString().Equals(any.ToString(), StringComparison.OrdinalIgnoreCase)) {
					return false;
				}
			}
			return true;
		}

		#region override: todo
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		#endregion
	}
}

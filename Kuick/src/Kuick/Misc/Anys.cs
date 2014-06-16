// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Any.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Kuick
{
	[Serializable]
	public class Anys : List<Any>
	{
		public Anys()
			: base()
		{
		}

		public Anys(params Any[] anys)
			: base()
		{
			AddRange(anys);
		}

		public Anys(string setting, char separator, string name)
		{
			if(Checker.IsNull(setting)) { return; }

			var sections = setting.Split(separator);
			foreach(string x in sections) {
				this.Add(new Any(name, x));
			}
		}

		public Anys(string setting, string separator, string connector)
		{
			if(Checker.IsNull(setting)) { return; }

			var sections = setting.SplitWith(separator);
			foreach(string one in sections) {
				string[] parts = one.Trim().SplitWith(connector);
				switch(parts.Length) {
					case 1:
						this.Add(new Any(parts[0].Trim(), parts[0].Trim()));
						break;
					case 2:
						this.Add(new Any(parts[0].Trim(), parts[1].Trim()));
						break;
					default:
						this.Add(new Any(
							parts[0].Trim(), 
							one.Substring(parts[0].Length).Trim()
						));
						break;
				}
			}
		}

		public Anys(string setting, char separator, char connector)
			: base()
		{
			AddRange(Formator.ToAnys(setting, separator, connector));
		}

		public Anys(params NameValueCollection[] nvcs)
			: base()
		{
			AddRange(nvcs);
		}

		public Any this[string name]
		{
			get
			{
				return Find(name);
			}
		}

		public Anys Add(string name, object val)
		{
			var any = new Any(name, val);
			this.Add(any);
			return this;
		}

		public Anys AddRange(params Any[] anys)
		{
			if(Checker.IsNull(anys)) { return this; }
			foreach(Any any in anys) {
				base.Add(any);
			}
			return this;
		}

		public Anys AddRange(params NameValueCollection[] nvcs)
		{
			if(Checker.IsNull(nvcs)) { return this; }
			foreach(NameValueCollection nvc in nvcs) {
				AddRange(Formator.NVc2NVs(nvc));
			}
			return this;
		}

		public bool Exists(string name)
		{
			if(name.IsNullOrEmpty()) { return false; }
			return Find(name) != null;
		}

		public Any Find(string name)
		{
			if(name.IsNullOrEmpty()) { return null; }
			return base.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public Any FindByValue(object value)
		{
			if(value == null) { return null; }
			return Find(x => x == null ? false : x.Value.Equals(value));
		}

		public bool Remove(string name)
		{
			if(name.IsNullOrEmpty()) { return false; }
			Any any = Find(name);
			return Checker.IsNull(any) ? false : Remove(any);
		}

		public string ToString(string name)
		{
			if(name.IsNullOrEmpty()) { return string.Empty; }
			return ToString(name, string.Empty);
		}

		public string ToString(string name, string airBag)
		{
			if(name.IsNullOrEmpty()) { return airBag; }
			Any any = Find(name);
			return Checker.IsNull(any) ? airBag : any.ToString();
		}

		public bool ToBoolean(string name)
		{
			if(name.IsNullOrEmpty()) { return false; }
			return ToBoolean(name, false);
		}
		public bool ToBoolean(string name, bool airBag)
		{
			if(name.IsNullOrEmpty()) { return airBag; }
			Any any = Find(name);
			return Checker.IsNull(any) ? airBag : any.ToBoolean();
		}

		public DateTime ToDateTime(string name)
		{
			return ToDateTime(name, Constants.Date.Min);
		}
		public DateTime ToDateTime(string name, DateTime airBag)
		{
			if(name.IsNullOrEmpty()) { return airBag; }
			Any any = Find(name);
			return Checker.IsNull(any) ? airBag : any.ToDateTime();
		}

		public int ToInteger(string name)
		{
			return ToInteger(name, 0);
		}
		public int ToInteger(string name, int airBag)
		{
			if(name.IsNullOrEmpty()) { return airBag; }
			Any any = Find(name);
			return Checker.IsNull(any) ? airBag : any.ToInteger();
		}

		public object GetValue(string name)
		{
			return GetValue(name, null);
		}
		public object GetValue(string name, object airBag)
		{
			if(name.IsNullOrEmpty()) { return airBag; }
			Any any = Find(name);
			return Checker.IsNull(any) ? airBag : any.Value;
		}

		public Anys SetValue(Any any)
		{
			if(null == any) { return this; }
			SetValue(any.Name, any.Value);
			return this;
		}
		public Anys SetValue(string name, object value)
		{
			if(name.IsNullOrEmpty()) { return this; }
			if(Exists(name)) { Remove(name); }
			Add(name, value);
			return this;
		}

		public object[] GetValues(string name)
		{
			if(name.IsNullOrEmpty()) { return new Object[0]; }
			Any any = Find(name);
			return Checker.IsNull(any) ? new Object[0] : any.Values;
		}

		public string[] AllNames
		{
			get
			{
				var list = new List<string>(Count);
				ForEach(x => list.Add(x.Name));
				return list.ToArray();
			}
		}

		public string[] AllValues
		{
			get
			{
				var list = new List<string>(Count);
				ForEach(x => list.Add(x.ToString()));
				return list.ToArray();
			}
		}

		public string Join(string separator, string connector)
		{
			var sb = new StringBuilder();
			foreach(var any in this) {
				if(sb.Length > 0) { sb.Append(separator); }
				sb.AppendFormat(string.Concat(
					any.Name, connector, any.ToString()
				));
			}
			return sb.ToString();
		}

		public override string ToString()
		{
			string str = Formator.ListAnys(ToArray());
			return str;
		}

		public string ToHtml()
		{
			string html = Formator.ListAnysAsHtml(ToArray());
			return html;
		}
	}
}

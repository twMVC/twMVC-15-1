// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// All.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-12-30 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class All<T> : Any where T : class, IEntity, new()
	{
		#region constructor
		public All()
			: base()
		{
			this.Alls = new List<All<T>>();
		}

		public All(string name)
			: base(name)
		{
			this.Alls = new List<All<T>>();
		}

		public All(string name, params object[] vals)
			: base(name, vals)
		{
			this.Alls = new List<All<T>>();
		}

		public All(string name, Func<T, string> func, params All<T>[] alls)
			: base(name)
		{
			this.Alls = new List<All<T>>();
			this.Delegate = func;
			if(null != alls) {
				foreach(All<T> all in alls) {
					this.Alls.Add(all);
				}
			}
		}

		public All(string name, object val, Func<T, string> func, params All<T>[] alls)
			: base(name, val)
		{
			this.Alls = new List<All<T>>();
			this.Delegate = func;
			if(null != alls) {
				foreach(All<T> all in alls) {
					this.Alls.Add(all);
				}
			}
		}
		#endregion

		public Func<T, string> Delegate { get; set; }

		public List<All<T>> Alls { get; set; }
	}
}

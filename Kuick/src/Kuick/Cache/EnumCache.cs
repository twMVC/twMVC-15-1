// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EnumCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-11-20 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class EnumCache
	{
		protected static Dictionary<string, object> _Cache = new Dictionary<string, object>();
		protected static object _Lock = new object();

		public static EnumReference<T> Get<T>()
		{
			Type type = typeof(T);
			EnumReference<T> er;
			object value;

			lock(_Lock) {
				if(_Cache.TryGetValue(type.Name, out value)) {
					try {
						er = (EnumReference<T>)value;
					} catch {
						er = new EnumReference<T>();
						_Cache.SafeAdd(type.Name, er);
					}
				} else {
					er = new EnumReference<T>();
					_Cache.Add(type.Name, er);
				}
			}
			return er;
		}

		public static EnumReference Get(Type type)
		{
			EnumReference er;
			object value;

			lock(_Lock) {
				if(_Cache.TryGetValue(type.Name, out value)) {
					er = (EnumReference)value;
				} else {
					er = new EnumReference(type);
					_Cache.Add(type.Name, er);
				}
			}
			return er;
		}
	}
}

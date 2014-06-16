// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// StaticCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public abstract class StaticCache<T>
	{
		protected static Dictionary<string, T> _Cache = new Dictionary<string, T>();
		protected static object _Lock = new object();

		// public
		public static int Count
		{
			get
			{
				return _Cache.Count;
			}
		}

		public static Dictionary<string, T>.KeyCollection Keys
		{
			get
			{
				return _Cache.Keys;
			}
		}

		public static Dictionary<string, T>.ValueCollection Values
		{
			get
			{
				return _Cache.Values;
			}
		}

		// protected
		internal static bool Add(string key, T value)
		{
			if(!_Cache.ContainsKey(key.ToLower())) {
				lock(_Lock) {
					if(!_Cache.ContainsKey(key.ToLower())) {
						_Cache.Add(key.ToLower(), value);
						return true;
					}
				}
			}

			return false;
		}

		#region Exists Get, TryGet, GetFirst, Find
		public static bool Exists(string key)
		{
			return _Cache.ContainsKey(key);
		}

		public static T Get(string key)
		{
			T value;
			if(_Cache.TryGetValue(key.ToLower(), out value)) {
				return value;
			} else {
				return default(T);
			}
		}

		public static I[] Get<I>() where I : T // interface
		{
			List<I> list = new List<I>();
			foreach(T x in Values) {
				if(x.IsDerived<I>()) { list.Add((I)x); }
			}
			return list.ToArray();
		}

		public static T[] Get(Type type)
		{
			List<T> list = new List<T>();
			foreach(T x in Values) {
				if(x.IsDerived(type)) { list.Add(x); }
			}
			return list.ToArray();
		}

		public static bool TryGet(string key, out T value)
		{
			return _Cache.TryGetValue(key.ToLower(), out value);
		}

		public static bool TryGet<I>(out I[] values) where I : T
		{
			List<I> list = new List<I>();
			foreach(T x in Values) {
				if(x.IsDerived<I>()) { list.Add((I)x); }
			}
			values = list.ToArray();
			return list.Count > 0;
		}

		public static bool TryGet(Type type, out T[] values)
		{
			List<T> list = new List<T>();
			foreach(T x in Values) {
				if(x.IsDerived(type)) { list.Add(x); }
			}
			values = list.ToArray();
			return list.Count > 0;
		}

		public static I GetFirst<I>() where I :  T
		{
			foreach(T x in Values) {
				if(x.GetType().Equals(typeof(I))) { return (I)x; }
			}

			foreach(T x in Values) {
				if(x.IsDerived<I>()) { return (I)x; }
			}

			return default(I);
		}

		public static T GetFirst(Type type)
		{
			foreach(T x in Values) {
				if(x.GetType().Equals(type)) { return x; }
			}

			foreach(T x in Values) {
				if(x.IsDerived(type)) { return x; }
			}

			return default(T);
		}

		public static I Find<I>() where I : T
		{
			Type type = typeof(I);
			foreach(T x in Values) {
				if(x.GetType().Equals(type)) { return (I)x; }
			}
			return default(I);
		}
		#endregion
	}
}

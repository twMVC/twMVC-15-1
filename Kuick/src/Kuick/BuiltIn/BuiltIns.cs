// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Builtins.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation
// Kevin Jong       2012-03-18 - Rename from Builtin to Builtins


using System.Collections.Generic;

namespace Kuick
{
	public class Builtins
	{
		private static KeyedByTypeCollection<IBuiltin> _DefaultCache =
			new KeyedByTypeCollection<IBuiltin>();
		private static KeyedByTypeCollection<IBuiltin> _CustomCache =
			new KeyedByTypeCollection<IBuiltin>();

		public static void Add(IBuiltin builtIn)
		{
			if(builtIn.IsNull) { return; }
			if(builtIn.Default) {
				if(!_DefaultCache.Contains(builtIn)) { _DefaultCache.Add(builtIn); }
			} else {
				if(!_CustomCache.Contains(builtIn)) { _CustomCache.Add(builtIn); }
			}
		}

		public static T Get<T, TNull>()
			where T : IBuiltin
			where TNull : class, T, new()
		{
			T builtIn = _CustomCache.Find<T>();
			if(null == builtIn) {
				builtIn = _DefaultCache.Find<T>();
			}
			return null == builtIn ? new TNull() : builtIn;
		}

		public static IEnumerator<IBuiltin> AllDefault
		{
			get
			{
				return _DefaultCache.GetEnumerator();
			}
		}

		public static IEnumerator<IBuiltin> AllCustom
		{
			get
			{
				return _CustomCache.GetEnumerator();
			}
		}

		#region Builtin
		public static IAuditor Auditor
		{
			get
			{
				return Get<IAuditor, NullAuditor>();
			}
		}

		public static ICache Cache
		{
			get
			{
				return Get<ICache, NullCache>();
			}
		}

		public static IConfig Config
		{
			get
			{
				return Get<IConfig, NullConfig>();
			}
		}

		public static IDirectory Directory
		{
			get
			{
				return Get<IDirectory, NullDirectory>();
			}
		}

		public static IIdentity Identity
		{
			get
			{
				return Get<IIdentity, NullIdentity>();
			}
		}

		public static IMultilingual Multilingual
		{
			get
			{
				return Get<IMultilingual, NullMultilingual>();
			}
		}

		public static IAuthenticate Authenticate
		{
			get
			{
				return Get<IAuthenticate, NullAuthenticate>();
			}
		}

		public static IAuthorize Authorize
		{
			get
			{
				return Get<IAuthorize, NullAuthorize>();
			}
		}
		#endregion
	}
}

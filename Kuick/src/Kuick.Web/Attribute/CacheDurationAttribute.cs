// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CacheDurationAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick.Web
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class CacheDurationAttribute : Attribute, ICloneable<CacheDurationAttribute>
	{
		public const string DEFAULT_PARAM_CLEAN_CACHE = "clean";
		public const string DEFAULT_PARAM_NO_CACHE = "nocache";

		#region constructor
		/// <summary>
		/// Default is cache forever.
		/// </summary>
		public CacheDurationAttribute() : this(int.MaxValue) { }

		/// <param name="Seconds">Cache duration in second.</param>
		public CacheDurationAttribute(int Seconds)
			: this(Seconds, String.Empty, String.Empty) { }

		/// <param name="Seconds">Cache duration in second.</param>
		/// <param name="clientOnly">Only cache on client, no serve cache.</param>
		public CacheDurationAttribute(int Seconds, bool clientOnly)
			: this(Seconds, clientOnly, String.Empty, String.Empty) { }

		/// <param name="Seconds">Cache duration in second.</param>
		/// <param name="cleanCacheParam">Request parameter Name for 
		/// clean output cache</param>
		/// <param name="noCacheParam">Request parameter name for response 
		/// no cache output</param>
		public CacheDurationAttribute(
			int Seconds,
			string cleanCacheParam,
			string noCacheParam)
			: this(Seconds, false, cleanCacheParam, noCacheParam) { }

		/// <param name="Seconds">Cache duration in second.</param>
		/// <param name="clientOnly">Only cache on client, no serve cache.</param>
		/// <param name="cleanCacheParam">Request parameter Name for 
		/// clean output cache</param>
		/// <param name="noCacheParam">Request parameter name for response 
		/// no cache output</param>
		public CacheDurationAttribute(
			int Seconds,
			bool clientOnly,
			string cleanCacheParam,
			string noCacheParam)
		{
			this.ClientOnly = clientOnly;
			this.Duration = Seconds > 0 ? Seconds : 0;
			this.CleanCacheParameter = String.IsNullOrEmpty(cleanCacheParam)
				? DEFAULT_PARAM_CLEAN_CACHE
				: cleanCacheParam;
			this.NoCacheParameter = String.IsNullOrEmpty(noCacheParam)
				? DEFAULT_PARAM_NO_CACHE
				: noCacheParam;
		}

		public CacheDurationAttribute(string entityName)
		{
			SetAssociatedWithEntity(entityName);
		}
		#endregion

		#region ICloneable<T>
		public CacheDurationAttribute Clone()
		{
			return WebChecker.IsNull(this.EntityName)
				? new CacheDurationAttribute(this.Duration)
				: new CacheDurationAttribute(this.EntityName);
		}
		#endregion

		#region property
		/// <summary>
		/// Cache duration , in second.
		/// </summary>
		public int Duration { get; private set; }
		public string NoCacheParameter { get; private set; }
		public string CleanCacheParameter { get; private set; }
		private string EntityName { get; set; }
		private string EntityKeyValue { get; set; }

		/// <summary>
		/// No server cache, only client!
		/// </summary>
		public bool ClientOnly { get; private set; }
		public bool Forever { get { return Duration == int.MaxValue; } }
		#endregion

		#region method
		public DateTime ExpireTime(DateTime baseTime)
		{
			return WebChecker.IsNull(EntityName)
				? Forever
					? DateTime.MaxValue
					: baseTime.AddSeconds(Duration)
				: Builtins.Cache.NeedUpdate("", EntityName, baseTime)
					? DateTime.Now.AddDays(-1)
					: DateTime.MaxValue;
		}

		public override string ToString()
		{
			return Forever
				? "Cached forever"
				: Duration > 0
					? String.Format("Cached for {0} second(s)", Duration)
					: "Never cached";
		}
		#endregion

		#region internal method
		internal void SetAssociatedWithEntity(string entityName)
		{
			SetAssociatedWithEntity(entityName, string.Empty);
		}

		internal void SetAssociatedWithEntity(string entityName, string entityKeyValue)
		{
			if(Builtins.Cache.IsNull) {
				ClientOnly = false;
				Duration = 0;
				CleanCacheParameter = DEFAULT_PARAM_CLEAN_CACHE;
				NoCacheParameter = DEFAULT_PARAM_NO_CACHE;
			} else {
				EntityName = entityName;
				ClientOnly = false;
				Duration = Utility.GetRandom(3600, 86400);
				CleanCacheParameter = DEFAULT_PARAM_CLEAN_CACHE;
				NoCacheParameter = DEFAULT_PARAM_NO_CACHE;
			}

			EntityKeyValue = entityKeyValue;
		}
		#endregion
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ObjectEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Kuick.Data
{
	[DataContract]
	[EntityIndex(false, FLAG, CREATE_DATE)]
	public abstract class ObjectEntity<T>
		: Entity<T>, IObjectEntity
		where T : ObjectEntity<T>, new()
	{
		#region Schema
		public const string CREATE_DATE = "CREATE_DATE";
		public const string LAST_MODIFIED_DATE = "LAST_MODIFIED_DATE";
		public const string FLAG = "FLAG";
		#endregion

		#region constructor
		public ObjectEntity()
			: base()
		{
			base.AfterAdd += new EntityEventHandler(AddCache);
			base.AfterModify += new EntityEventHandler(ModifyCache);
			base.AfterRemove += new EntityEventHandler(RemoveCache);
		}
		#endregion

		#region property
		[DataMember]
		[Category(DataConstants.Entity.Category)]
		[ColumnSpec(CREATE_DATE, SpecFlag.ReadOnly)]
		[ColumnInitiate(InitiateValue.Date17)]
		[ColumnVisual(VisualFlag.SystemColumn)]
		[IgnoreDiff]
		public DateTime CreateDate { get; set; }

		[DataMember]
		[Category(DataConstants.Entity.Category)]
		[ColumnSpec(LAST_MODIFIED_DATE)]
		[ColumnInitiate(InitiateValue.Date17AutoUpdate)]
		[ColumnVisual(VisualFlag.SystemColumn)]
		[IgnoreDiff]
		public DateTime LastModifiedDate { get; set; }

		[DataMember]
		[Category(DataConstants.Entity.Category)]
		[ColumnSpec(FLAG)]
		[ColumnInitiate(true)]
		[ColumnVisual(VisualFlag.SystemColumn)]
		[IgnoreDiff]
		public bool Flag { get; set; }
		#endregion
		
		#region IEntity
		public override void FrontEndInterceptor(Sql<T> sql)
		{
			sql.Where(x => x.Flag == true);
			base.FrontEndInterceptor(sql);
		}

		public override Flag Concurrency
		{
			get
			{
				return Kuick.Flag.Enable;
			}
		}
		#endregion

		#region IObjectEntity
		public void ResetCache()
		{
			ClearCache();
		}
		#endregion

		#region class level
		protected static int _CacheCheckSeconds = 30;
		protected static object _CachedLocking = new object();
		protected static DateTime _CachedTime = Constants.Date.Min;
		protected static DateTime _CheckTime = Constants.Date.Min;
		protected static int _CachedCount = 0;
		protected static IQueryable<T> _Cache = null;
		public static IQueryable<T> Cache
		{
			get
			{
				T one = EntityCache.Find<T>();
				if(!one.EnableCache) {
					throw new KException(string.Format(
						"{0} is not caching enabled.",
						one.GetType().Name
					));
				}
				if(CachedNeedUpdate) {
					lock (_CachedLocking) {
						if (CachedNeedUpdate) {
							_CheckTime = DateTime.Now;

							T last = Sql()
								.Descending(x => x.LastModifiedDate)
								.QueryFirst();

							bool needUpdate = false;
							if(null != last) {
								needUpdate = _CachedTime < last.LastModifiedDate;
							}
							if(needUpdate) {
								_CachedTime = last.LastModifiedDate;
							}else{
								int count = Sql().Count();
								needUpdate = count != _CachedCount;
								if(needUpdate) {
									_CachedTime = DateTime.Now;
								}
							}

							if (needUpdate) {
								using (ILogger il = 
									new ILogger("Update Cache", LogLevel.Track)) {
									_CachedCount = Count();
									List<T> all = GetAll();
									if (null == all) { all = new List<T>(); }
									_Cache = all.AsQueryable();
									DictionaryCache = _Cache.ToDictionary(x => 
										x.KeyValue
									);

									Int64 size = 0;
									foreach (var x in _Cache) {
										size += x.SizeOf();
									}

									il.Add("Entity Name", EntityName);
									il.Add(
										"Cached Time", 
										_CachedTime.yyyyMMddHHmmssfff()
									);
									il.Add("Total Count", _CachedCount);
									il.Add("Cache Count", _Cache.Count());
									il.Add(
										"Memory Size(bytes)", 
										size.ToThousandPoint()
									);
								}
							}
						}
					}
				}
				IQueryable<T> newCache = one.CacheInterceptor(_Cache);
				return newCache;
			}
		}
		public static bool CachedNeedUpdate
		{
			get
			{
				return
					null == _Cache
					||
					_CachedTime == DateTime.MinValue
					||
					_CachedTime.AddSeconds(_CacheCheckSeconds) < DateTime.Now
					||
					Checker.Overdue(
						_CheckTime, 
						DataCurrent.Data.CacheFrequency<T>()
					);
			}
		}
		public static void ClearCache()
		{
			_Cache = null;
			_CachedTime = DateTime.MinValue;
		}
		public static bool CacheExpired(DateTime dt)
		{
			T latest = Sql()
				.Descending(x => x.LastModifiedDate)
				.QueryFirst();
			return null != latest && latest.LastModifiedDate > dt;
		}

		public static IQueryable<T> EmptyQueryable
		{
			get
			{
				return Enumerable.Empty<T>().AsQueryable();
			}
		}

		private static Dictionary<string, T> _DictionaryCache = 
			new Dictionary<string, T>();
		public static Dictionary<string, T> DictionaryCache
		{
			get { return _DictionaryCache; }
			private set { _DictionaryCache = value; }
		}

		public static T GetFromCache(string keyValue)
		{
			T one = default(T);
			DictionaryCache.TryGetValue(keyValue, out one);
			return one;
		}
		#endregion

		#region event handler
		private void AddCache(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == _Cache) { return; }
			List<T> list = new List<T>();
			list.Add((T)this);
			_Cache = _Cache.Concat(list);
		}
		private void ModifyCache(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == _Cache) { return; }
			List<T> list = new List<T>();
			list.Add((T)this);
			_Cache = _Cache.Where(x => x.KeyValue != this.KeyValue).Concat(list);
		}
		private void RemoveCache(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == _Cache) { return; }
			_Cache = _Cache.Where(x => x.KeyValue != this.KeyValue);
		}
		#endregion

		#region IObjectEntity
		public bool SkipUpdateModifiedDate { get; set; }
		public DataResult UpdateModifiedDate()
		{
			SkipDirtyColumnCheck = true;
			DataResult result = Modify();
			SkipDirtyColumnCheck = false;
			return result;
		}
		#endregion
	}
}

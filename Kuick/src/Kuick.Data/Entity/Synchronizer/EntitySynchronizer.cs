// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntitySynchronizer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class EntitySynchronizer<TSource, TDestination>
		where TSource : class, IObjectEntity, new()
		where TDestination : class, IObjectEntity, new()
	{
		private static TSource _Source;
		private static TDestination _Destination;
		private static object _Lock = new object();

		public EntitySynchronizer()
		{
			// property
			this.EnumeratorPageSize = 100;

			// Func
			this.FromSourceFunc = this.FromSource;
			this.CombineFunc = this.Combine;
			this.EqualsFunc = this.Equals;
			this.SkipInsertOneFunc = this.SkipInsertOne;
			this.SkipDeleteOneFunc = this.SkipDeleteOne;

			// Action
			this.BeforeInsertNewDataAction = this.BeforeInsertNewData;
			this.BeforeUpdateDataAction = this.BeforeUpdateData;
			this.AfterInsertNewDataAction = this.AfterInsertNewData;
			this.AfterUpdateDataAction = this.AfterUpdateData;
		}

		#region Entity
		protected TSource Source
		{
			get
			{
				if(null == _Source) {
					lock(_Lock) {
						if(null == _Source) {
							_Source = EntityCache.Find<TSource>();
						}
					}
				}
				return _Source;
			}
		}

		protected TDestination Destination
		{
			get
			{
				if(null == _Destination) {
					lock(_Lock) {
						if(null == _Destination) {
							_Destination = EntityCache.Find<TDestination>();
						}
					}
				}
				return _Destination;
			}
		}
		#endregion

		#region property
		public virtual int EnumeratorPageSize { get; private set; }
		#endregion

		#region Skip
		public virtual bool SkipInsert { get { return false; } }
		public virtual bool SkipUpdate { get { return false; } }
		public virtual bool SkipDelete { get { return false; } }
		public virtual bool SkipDeleteWhenSourceEmpty { get { return true; } }
		public virtual bool DisableWhenDelete { get { return true; } }
		#endregion

		#region Func
		public Func<TSource, TDestination> FromSourceFunc { private get; set; }
		public Func<TSource, TDestination, TDestination> CombineFunc 
		{ private get; set; }
		public Func<TSource, TDestination, bool> EqualsFunc 
		{ private get; set; }
		public Func<TSource, bool> SkipInsertOneFunc { private get; set; }
		public Func<TDestination, bool> SkipDeleteOneFunc { private get; set; }
		public Action<TSource> BeforeInsertNewDataAction { private get; set; }
		public Action<TSource, TDestination> BeforeUpdateDataAction 
		{ private get; set; }
		public Action<Result, TSource> AfterInsertNewDataAction 
		{ private get; set; }
		public Action<Result, TSource, TDestination> AfterUpdateDataAction 
		{ private get; set; }
		#endregion

		public virtual TDestination FromSource(TSource source)
		{
			TDestination destination = new TDestination();
			return CombineFunc(source, destination);
		}

		public virtual TDestination Combine(
			TSource source, TDestination destination)
		{
			Reflector.Copy(source, destination);
			return destination;
		}

		public virtual bool Equals(TSource source, TDestination destination)
		{
			return source.Equals(destination);
		}

		public virtual bool SkipInsertOne(TSource source)
		{
			return false;
		}

		public virtual bool SkipUpdateOne(
			TSource source, TDestination destination)
		{
			return false;
		}

		public virtual bool SkipDeleteOne(TDestination destination)
		{
			return false;
		}

		public virtual void BeforeInsertNewData(TSource source)
		{
			// do nothoing
		}
		public virtual void BeforeUpdateData(
			TSource source, TDestination destination)
		{
			// do nothoing
		}
		public virtual void AfterInsertNewData(Result result, TSource source)
		{
			// do nothoing
		}
		public virtual void AfterUpdateData(
			Result result, TSource source, TDestination destination)
		{
			// do nothoing
		}

		private Result InsertNewData(TSource source)
		{
			// SkipInsertOne
			if(SkipInsertOne(source)) {
				return Result.BuildSuccess();
			}

			// BeforeInsertNewDataAction
			BeforeInsertNewDataAction(source);

			// 
			TDestination destination = FromSourceFunc(source);
			Result result = destination.Add();

			// AfterInsertNewDataAction
			AfterInsertNewDataAction(result, source);

			if(result.Success) {
				Logger.Track(
					"EntitySynchronizer.InsertNewData",
					destination.EntityName,
					destination.ToAny()
				);
			} else {
				Logger.Error(
					"EntitySynchronizer.InsertNewData",
					string.Format(
						"{0}: {1}", 
						destination.EntityName, 
						result.Message
					),
					null == result.Exception
						? destination.ToAny()
						: result.Exception.ToAny(destination.ToAny().ToArray())
				);
			}
			return result;
		}

		private Result UpdateData(TSource source, TDestination destination)
		{
			// SkipUpdateOne
			if(SkipUpdateOne(source, destination)) {
				return Result.BuildSuccess();
			}

			if(EqualsFunc(source, destination)) {
				return Result.BuildSuccess();
			}

			TDestination newDestination = CombineFunc(source, destination);

			// BeforeUpdateDataAction
			BeforeUpdateDataAction(source, destination);

			// 
			Result result = newDestination.Modify();

			// AfterUpdateDataAction
			AfterUpdateDataAction(result, source, destination);

			if(result.Success) {
				Logger.Track(
					"EntitySynchronizer.UpdateData",
					newDestination.EntityName,
					newDestination.ToAny()
				);
			} else {
				Logger.Error(
					"EntitySynchronizer.UpdateData",
					string.Format(
						"{0}: {1}",
						newDestination.EntityName,
						result.Message
					),
					null == result.Exception
						? newDestination.ToAny()
						: result.Exception.ToAny(
							newDestination.ToAny().ToArray()
						)
				);
			}

			return new Result();
		}

		public virtual void Sync()
		{
			#region delete
			if(
				SkipDelete
				||
				(
					SkipDeleteWhenSourceEmpty
					&&
					Entity.Count(Source.EntityName) == 0
			)) {
				Logger.Track(
					"EntitySynchronizer.Sync",
					"Skip delete",
					new Any("Source Entity Name", Source.EntityName),
					new Any("Destination Entity Name", Destination.EntityName)
				);
			} else {
				List<string> deleteList = new List<string>();
				using(EntityEnumerator<TDestination> destinationEnumerator =
				new EntityEnumerator<TDestination>(EnumeratorPageSize)
				) {
					while(destinationEnumerator.MoveNext()) {
						List<TDestination> destinations = 
							destinationEnumerator.Current;
						foreach(TDestination destination in destinations) {
							if(!Entity.Exists(
								Source.EntityName,
								new Any(Source.KeyName, destination.KeyValue))) {
								deleteList.Add(destination.KeyValue);
							}
						}
					}
				}
				foreach(string keyValue in deleteList) {
					TDestination destination = 
						Entity<TDestination>.Get(keyValue);
					if(!SkipDeleteOne(destination)) {
						destination.Flag = false;
						destination.Modify();
					}
				}
			}
			#endregion

			#region insert & update
			using(EntityEnumerator<TSource> sourceEnumerator =
				new EntityEnumerator<TSource>(EnumeratorPageSize)
			) {
				while(sourceEnumerator.MoveNext()) {
					List<TSource> sources = sourceEnumerator.Current;
					foreach(TSource source in sources) {
						TDestination destination = Entity<TDestination>.Get(
							source.KeyValue
						);
						if(Checker.IsNull(destination)) {
							// Insert
							if(!SkipInsert) { InsertNewData(source); }
						} else {
							// Update
							if(!SkipUpdate) { UpdateData(source, destination); }
						}
					}
				}
			}
			#endregion
		}
	}
}

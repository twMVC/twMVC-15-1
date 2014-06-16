// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Interceptor.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation
// Kevin Jong       2012-09-28 - Fix Action Casting


using System;
using System.Collections.Generic;
using Kuick;
using System.Data.Common;
using System.Data;

namespace Kuick.Data
{
	// Interceptor.AttachBeforeSelect<UserEntity>((x, y) => { 
	//     // ... 
	// });
	public class Interceptor
	{
		#region BeforeSelect
		private static InterceptorCache<BeforeRAction> _BeforeSelectCache =
			new InterceptorCache<BeforeRAction>();
		public static void AttachBeforeSelect(
			string entityName, 
			Action<string, Sql> action)
		{
			ExecutingCheck();
			_BeforeSelectCache.Add(new BeforeRAction(
				entityName, action
			));
		}
		public static void AttachBeforeSelect<T>(
			Action<string, Sql> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeSelectCache.Add(new BeforeRAction(
				typeof(T).Name, action
			));
		}
		public static void DetachBeforeSelect<T>(
			Action<string, Sql> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeSelectCache.Remove(new BeforeRAction(
				typeof(T).Name, action
			));
		}
		internal static void OnBeforeSelect(
			string entityName, 
			string token, Sql sql)
		{
			_BeforeSelectCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, sql));
		}
		#endregion

		#region BeforeInsert
		private static InterceptorCache<BeforeCUDAction<IEntity>> _BeforeInsertCache =
			new InterceptorCache<BeforeCUDAction<IEntity>>();
		public static void AttachBeforeInsert(
			string entityName, 
			Action<string, IEntity> action)
		{
			ExecutingCheck();
			_BeforeInsertCache.Add(new BeforeCUDAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachBeforeInsert<T>(
			Action<string, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeInsertCache.Add(new BeforeCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachBeforeInsert<T>(
			Action<string, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeInsertCache.Remove(new BeforeCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnBeforeInsert(
			string entityName, 
			string token, 
			IEntity instance)
		{
			_BeforeInsertCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, instance));
		}
		#endregion

		#region BeforeUpdate
		private static InterceptorCache<BeforeCUDAction<IEntity>> _BeforeUpdateCache =
			new InterceptorCache<BeforeCUDAction<IEntity>>();
		public static void AttachBeforeUpdate(
			string entityName, 
			Action<string, IEntity> action)
		{
			ExecutingCheck();
			_BeforeUpdateCache.Add(new BeforeCUDAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachBeforeUpdate<T>(
			Action<string, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeUpdateCache.Add(new BeforeCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachBeforeUpdate<T>(
			Action<string, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeUpdateCache.Remove(new BeforeCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnBeforeUpdate(
			string entityName, 
			string token, 
			IEntity instance)
		{
			_BeforeUpdateCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, instance));
		}
		#endregion

		#region BeforeDelete
		private static InterceptorCache<BeforeCUDAction<IEntity>> _BeforeDeleteCache =
			new InterceptorCache<BeforeCUDAction<IEntity>>();
		public static void AttachBeforeDelete(
			string entityName, 
			Action<string, IEntity> action)
		{
			ExecutingCheck();
			_BeforeDeleteCache.Add(new BeforeCUDAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachBeforeDelete<T>(
			Action<string, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeDeleteCache.Add(new BeforeCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachBeforeDelete<T>(
			Action<string, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeDeleteCache.Remove(new BeforeCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnBeforeDelete(
			string entityName, 
			string token, 
			IEntity instance)
		{
			_BeforeDeleteCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, instance));
		}
		#endregion

		#region BeforeExecuteNonQuery
		private static InterceptorCache<BeforeExecuteAction> _BeforeExecuteNonQueryCache =
			new InterceptorCache<BeforeExecuteAction>();
		public static void AttachBeforeExecuteNonQuery(
			string entityName, 
			Action<string, string, IDbDataParameter[]> action)
		{
			ExecutingCheck();
			_BeforeExecuteNonQueryCache.Add(new BeforeExecuteAction(
				entityName, action
			));
		}
		public static void AttachBeforeExecuteNonQuery<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteNonQueryCache.Add(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		public static void DetachBeforeExecuteNonQuery<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteNonQueryCache.Remove(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		internal static void OnBeforeExecuteNonQuery(
			string entityName, 
			string token, 
			string commandText, 
			IDbDataParameter[] parameters)
		{
			_BeforeExecuteNonQueryCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, commandText, parameters));
		}
		#endregion

		#region BeforeExecuteQuery
		private static InterceptorCache<BeforeExecuteAction> _BeforeExecuteQueryCache =
			new InterceptorCache<BeforeExecuteAction>();
		public static void AttachBeforeExecuteQuery(
			string entityName, 
			Action<string, string, IDbDataParameter[]> action)
		{
			ExecutingCheck();
			_BeforeExecuteQueryCache.Add(new BeforeExecuteAction(
				entityName, action
			));
		}
		public static void AttachBeforeExecuteQuery<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteQueryCache.Add(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		public static void DetachBeforeExecuteQuery<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteQueryCache.Remove(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		internal static void OnBeforeExecuteQuery(
			string entityName, 
			string token, 
			string commandText, IDbDataParameter[] parameters)
		{
			_BeforeExecuteQueryCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, commandText, parameters));
		}
		#endregion

		#region BeforeExecuteScalar
		private static InterceptorCache<BeforeExecuteAction> _BeforeExecuteScalarCache =
			new InterceptorCache<BeforeExecuteAction>();
		public static void AttachBeforeExecuteScalar(
			string entityName, 
			Action<string, string, IDbDataParameter[]> action)
		{
			ExecutingCheck();
			_BeforeExecuteScalarCache.Add(new BeforeExecuteAction(
				entityName, action
			));
		}
		public static void AttachBeforeExecuteScalar<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteScalarCache.Add(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		public static void DetachBeforeExecuteScalar<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteScalarCache.Remove(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		internal static void OnBeforeExecuteScalar(
			string entityName, 
			string token, 
			string commandText, 
			IDbDataParameter[] parameters)
		{
			_BeforeExecuteScalarCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, commandText, parameters));
		}
		#endregion

		#region BeforeExecuteStoredProcedure
		private static InterceptorCache<BeforeExecuteAction> _BeforeExecuteStoredProcedure =
			new InterceptorCache<BeforeExecuteAction>();
		public static void AttachBeforeExecuteStoredProcedure(
			string entityName, 
			Action<string, string, IDbDataParameter[]> action)
		{
			ExecutingCheck();
			_BeforeExecuteStoredProcedure.Add(new BeforeExecuteAction(
				entityName, action
			));
		}
		public static void AttachBeforeExecuteStoredProcedure<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteStoredProcedure.Add(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		public static void DetachBeforeExecuteStoredProcedure<T>(
			Action<string, string, IDbDataParameter[]> action)
			where T : IEntity
		{
			ExecutingCheck();
			_BeforeExecuteStoredProcedure.Remove(new BeforeExecuteAction(
				typeof(T).Name, action
			));
		}
		internal static void OnBeforeExecuteStoredProcedure(
			string entityName, 
			string token, 
			string commandText, 
			IDbDataParameter[] parameters)
		{
			_BeforeExecuteStoredProcedure
				.FindAll(entityName)
				.ForEach(x => x.Action(token, commandText, parameters));
		}
		#endregion

		#region AfterSelect

		private static InterceptorCache<AfterRAction<IEntity>> _AfterSelectCache =
			new InterceptorCache<AfterRAction<IEntity>>();
		public static void AttachAfterSelect(
			string entityName, 
			Action<string, Sql, IEntity> action)
		{
			ExecutingCheck();
			_AfterSelectCache.Add(new AfterRAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachAfterSelect<T>(
			Action<string, Sql, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterSelectCache.Add(new AfterRAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachAfterSelect<T>(
			Action<string, Sql, T> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterSelectCache.Remove(new AfterRAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnAfterSelect(
			string entityName, 
			string token, 
			Sql sql, 
			IEntity instance)
		{
			_AfterSelectCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, sql, instance));
		}
		#endregion

		#region AfterInsert
		private static InterceptorCache<AfterCUDAction<IEntity>> _AfterInsertCache =
			new InterceptorCache<AfterCUDAction<IEntity>>();
		public static void AttachAfterInsert(
			string entityName, 
			Action<string, IEntity, Result> action)
		{
			ExecutingCheck();
			_AfterInsertCache.Add(new AfterCUDAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachAfterInsert<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterInsertCache.Add(new AfterCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachAfterInsert<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterInsertCache.Remove(new AfterCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnAfterInsert(
			string entityName, 
			string token, 
			IEntity instance, 
			Result result)
		{
			_AfterInsertCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, instance, result));
		}
		#endregion

		#region AfterUpdate
		private static InterceptorCache<AfterCUDAction<IEntity>> _AfterUpdateCache =
			new InterceptorCache<AfterCUDAction<IEntity>>();
		public static void AttachAfterUpdate(
			string entityName, 
			Action<string, IEntity, Result> action)
		{
			ExecutingCheck();
			_AfterUpdateCache.Add(new AfterCUDAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachAfterUpdate<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterUpdateCache.Add(new AfterCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachAfterUpdate<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterUpdateCache.Remove(new AfterCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnAfterUpdate(
			string entityName, 
			string token, 
			IEntity instance, 
			Result result)
		{
			_AfterUpdateCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, instance, result));
		}
		#endregion

		#region AfterDelete
		private static InterceptorCache<AfterCUDAction<IEntity>> _AfterDeleteCache =
			new InterceptorCache<AfterCUDAction<IEntity>>();
		public static void AttachAfterDelete(
			string entityName, 
			Action<string, IEntity, Result> action)
		{
			ExecutingCheck();
			_AfterDeleteCache.Add(new AfterCUDAction<IEntity>(
				entityName, action
			));
		}
		public static void AttachAfterDelete<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterDeleteCache.Add(new AfterCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		public static void DetachAfterDelete<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			ExecutingCheck();
			_AfterDeleteCache.Remove(new AfterCUDAction<IEntity>(
				typeof(T).Name, ActionTransfer<T>(action)
			));
		}
		internal static void OnAfterDelete(
			string entityName, 
			string token, 
			IEntity instance, 
			Result result)
		{
			_AfterDeleteCache
				.FindAll(entityName)
				.ForEach(x => x.Action(token, instance, result));
		}
		#endregion


		#region AfterQuery
		//private static InterceptorCache<AfterRAction<IEntity>> _AfterSelectCache =
		//    new InterceptorCache<AfterRAction<IEntity>>();
		//public static void AttachAfterSelect(
		//    string entityName,
		//    Action<string, Sql, IEntity> action)
		//{
		//    ExecutingCheck();
		//    _AfterSelectCache.Add(new AfterRAction<IEntity>(
		//        entityName, action
		//    ));
		//}
		//public static void AttachAfterSelect<T>(
		//    Action<string, Sql, T> action)
		//    where T : IEntity
		//{
		//    ExecutingCheck();
		//    _AfterSelectCache.Add(new AfterRAction<IEntity>(
		//        typeof(T).Name, ActionTransfer<T>(action)
		//    ));
		//}
		//public static void DetachAfterSelect<T>(
		//    Action<string, Sql, T> action)
		//    where T : IEntity
		//{
		//    ExecutingCheck();
		//    _AfterSelectCache.Remove(new AfterRAction<IEntity>(
		//        typeof(T).Name, ActionTransfer<T>(action)
		//    ));
		//}
		//internal static void OnAfterSelect(
		//    string entityName,
		//    string token,
		//    Sql sql,
		//    IEntity instance)
		//{
		//    _AfterSelectCache
		//        .FindAll(entityName)
		//        .ForEach(x => x.Action(token, sql, instance));
		//}
		#endregion

		#region SkipFrontEnd
		private static Func< bool> _SkipFrontEnd;
		public static Func<bool> SkipFrontEnd
		{
			get
			{
				if(null == _SkipFrontEnd) {
					_SkipFrontEnd = () => false;
				}
				return _SkipFrontEnd;
			}
			set
			{
				_SkipFrontEnd = value;
			}
		}
		#endregion

		#region private
		private static Action<string, IEntity> ActionTransfer<T>(
			Action<string, T> action)
			where T : IEntity
		{
			if(null == action) { throw new NullReferenceException(); }
			return new Action<string, IEntity>((token, instance) =>
				action(token, (T)instance)
			);
		}
		private static Action<string, Sql, IEntity> ActionTransfer<T>(
			Action<string, Sql, T> action)
			where T : IEntity
		{
			if(null == action) { throw new NullReferenceException(); }
			return new Action<string, Sql, IEntity>((token, sql, instance) => 
				action(token, sql, (T)instance)
			);
		}
		private static Action<string, IEntity, Result> ActionTransfer<T>(
			Action<string, T, Result> action)
			where T : IEntity
		{
			if(null == action) { throw new NullReferenceException(); }
			return new Action<string, IEntity, Result>((token, instance, result) =>
				action(token, (T)instance, result)
			);
		}
		private static void ExecutingCheck()
		{
			if(Heartbeat.Singleton.PreStartFinished) {
				throw new KernelException("Interceptor attaching or detaching operation only can do in the section of IStart.DoPreStart.");
			}
		}
		#endregion

	}
}

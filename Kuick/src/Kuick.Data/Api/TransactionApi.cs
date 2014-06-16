// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// TransactionApi.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation
// Kevin Jong       2012-09-25 - Brand new change implementation


using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Threading;

namespace Kuick.Data
{
	public class TApi : TransactionApi
	{
		public TApi()
			: base()
		{
		}
		public TApi(string entityName)
			: base(entityName)
		{
		}
		public TApi(IntervalLogger il, string entityName)
			: base(il, entityName)
		{
		}
	}

	/// <summary>
	/// According to the different request session and entity 
	/// to create a different transaction connection. It means 
	/// each session can create more than one transaction connection 
	/// in the same time.
	/// </summary>
	public class TransactionApi : Api, IDisposable
	{
		private static object _Lock = new object();
		private static Dictionary<string, TransactionApi> _Transactions =
			new Dictionary<string, TransactionApi>();

		public IntervalLogger IntervalLogger;
		private bool _SelfLogger;

		#region constructor
		public TransactionApi()
			: this(DataConstants.Entity.Name)
		{
		}
		public TransactionApi(string entityName)
			: this(null, entityName)
		{
		}
		public TransactionApi(IntervalLogger il, string entityName)
			: base(entityName, IsolationLevel.ReadCommitted)
		{
			this._SelfLogger = null == il;
			this.IntervalLogger = null == il
				? new IntervalLogger("In TransactionApi")
				: il;
			base.DoBegin(IntervalLogger);
			_Transactions.Add(Heartbeat.ScopeID, this);
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			lock(_Lock) {
				base.DoEnd(IntervalLogger);
				if(_Transactions.ContainsKey(Heartbeat.ScopeID)) {
					_Transactions.SafeRemove(Heartbeat.ScopeID);
				}
				if(_SelfLogger) {
					IntervalLogger.Dispose();
					IntervalLogger = null;
				}
			}
		}
		#endregion

		#region Singleton
		public static TransactionApi Scope<T>() where T : IEntity
		{
			return Scope(null, typeof(T).Name);
		}

		public static TransactionApi Scope<T>(IntervalLogger il) where T : IEntity
		{
			return Scope(il, typeof(T).Name);
		}

		public static TransactionApi Scope(string entityName)
		{
			return Scope(null, entityName);
		}

		public static TransactionApi Scope(IntervalLogger il, string entityName)
		{
			lock(_Lock) {
				TransactionApi ts;
				if(!_Transactions.TryGetValue(Heartbeat.ScopeID, out ts)) {
					ts = new TransactionApi(il, entityName);
				}
				return ts;
			}
		}

		public static bool InTransaction
		{
			get
			{
				return _Transactions.ContainsKey(Heartbeat.ScopeID);
			}
		}
		#endregion

		#region instance
		public void Rollback()
		{
			base.DoRollback(IntervalLogger);
		}

		public void Commit()
		{
			base.DoCommit(IntervalLogger);
		}
		#endregion
	}
}

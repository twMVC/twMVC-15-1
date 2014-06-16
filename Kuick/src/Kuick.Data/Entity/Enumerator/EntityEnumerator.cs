// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntityEnumerator.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Collections;

namespace Kuick.Data
{
	public class EntityEnumerator<T> 
		: IEnumerator<List<T>> 
		where T : class, IEntity, new()
	{
		private const int DEFAULT_PAGE_SIZE = 100;
		private T _Instance;
		private Sql<T> _Sql;
		private int _Count = -1;
		private Paginator _Paginator;

		#region constructor
		public EntityEnumerator()
			: this(DEFAULT_PAGE_SIZE)
		{
		}

		public EntityEnumerator(int pageSize)
			: this(null, pageSize)
		{
		}

		public EntityEnumerator(Sql<T> sql)
			: this(sql, DEFAULT_PAGE_SIZE)
		{
		}

		public EntityEnumerator(Sql<T> sql, int pageSize)
		{
			this.PageSize = (pageSize <= 0)
				? DEFAULT_PAGE_SIZE
				: pageSize;
			this.Sql = sql;
			this.PageIndex = 0;
		}
		#endregion

		#region property
		public T Instance
		{
			get
			{
				if(Checker.IsNull(_Instance)) {
					_Instance = new T();
				}
				return _Instance;
			}
		}

		public int PageSize { get; private set; }

		public Sql<T> Sql
		{
			get
			{
				if(Checker.IsNull(_Sql)) {
					_Sql = new Sql<T>();
				}
				return _Sql;
			}
			private set
			{
				_Sql = value;
			}
		}

		public int Count
		{
			get
			{
				if(_Count == -1) {
					_Count = Sql.Count();
				}
				return _Count;
			}
		}

		public Paginator Paginator
		{
			get
			{
				if(Checker.IsNull(_Paginator)) {
					_Paginator = new Paginator(PageSize, 1, Count);
				}
				return _Paginator;
			}
		}

		public int PageIndex { get; private set; }
		#endregion

		#region IEnumerator<IEntity>
		public List<T> Current { get; private set; }
		#endregion

		#region IDisposable
		public void Dispose()
		{
			_Paginator = null;
			_Instance = default(T);
		}
		#endregion

		#region IEnumerator
		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool MoveNext()
		{
			if(Paginator.LastPageIndex < ++PageIndex) {
				return false;
			}

			Sql.Paging(PageSize, PageIndex);
			Current = Sql.Query();
			return !Checker.IsNull(Current);
		}

		public void Reset()
		{
			PageIndex = 0;
		}
		#endregion
	}
}

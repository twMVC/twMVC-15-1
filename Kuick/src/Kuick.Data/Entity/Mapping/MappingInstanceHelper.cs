// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MappingInstanceHelper.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class MappingInstanceHelper<TContainer, TMember>
		where TContainer : class, IEntity, new()
		where TMember : class, IEntity, new()
	{
		public MappingInstanceHelper(TContainer container)
		{
			this.Container = container;
		}

		public TContainer Container { get; private set; }

		public int Count()
		{
			return Helper.Count(Container.KeyValue);
		}

		public bool Exists(string memberID)
		{
			return Helper.Exists(Container.KeyValue, memberID);
		}

		public Result Add(params string[] memberIDs)
		{
			return Helper.Add(Container.KeyValue, memberIDs);
		}

		public Result Remove(params string[] memberIDs)
		{
			return Helper.Remove(Container.KeyValue, memberIDs);
		}

		public Result Clear()
		{
			return Helper.Clear(Container.KeyValue);
		}

		public void Sorting(params string[] ids)
		{
			Helper.Sorting(ids);
		}

		public void Sorting(string containerID, string[] memberIDs)
		{
			Helper.Sorting(containerID, memberIDs);
		}

		public List<TMember> Get()
		{
			return Helper.Get(Container.KeyValue);
		}

		public List<TMember> Get(Func<Sql<TMember>, Sql<TMember>> sqlIntercepter)
		{
			return Helper.Get(sqlIntercepter, Container.KeyValue);
		}

		public List<string> AllMemberIDs()
		{
			return Helper.AllMemberIDs(Container.KeyValue);
		}

		public Sql<TMember> GetSql()
		{
			return Helper.GetSql(Container.KeyValue);
		}

		private MappingHelper<TContainer, TMember> _Helper;
		private MappingHelper<TContainer, TMember> Helper
		{
			get
			{
				if(null == _Helper) {
					_Helper = new MappingHelper<TContainer, TMember>();
				}
				return _Helper;
			}
		}
	}

	public class MappingInstanceHelper
	{
		public MappingInstanceHelper(IEntity container, string memberEntityName)
		{
			this.Container = container;
			this.MemberEntityName = memberEntityName;
		}

		public IEntity Container { get; private set; }
		public string MemberEntityName { get; private set; }

		public int Count()
		{
			return Helper.Count(Container.KeyValue);
		}

		public bool Exists(string memberID)
		{
			return Helper.Exists(Container.KeyValue, memberID);
		}

		public Result Add(params string[] memberIDs)
		{
			return Helper.Add(Container.KeyValue, memberIDs);
		}

		public Result Remove(params string[] memberIDs)
		{
			return Helper.Remove(Container.KeyValue, memberIDs);
		}

		public Result Clear()
		{
			return Helper.Clear(Container.KeyValue);
		}

		public void Sorting(
			params string[] ids)
		{
			Helper.Sorting(Container.EntityName, ids);
		}

		public void Sorting(string containerID, string[] memberIDs)
		{
			Helper.Sorting(containerID, memberIDs);
		}

		public List<IEntity> Get()
		{
			return Helper.Get(Container.KeyValue);
		}

		public List<IEntity> Get(Func<Sql, Sql> sqlIntercepter)
		{
			return Helper.Get(sqlIntercepter, Container.KeyValue);
		}

		public List<string> AllMemberIDs()
		{
			return Helper.AllMemberIDs(Container.KeyValue);
		}

		private MappingHelper _Helper;
		private MappingHelper Helper
		{
			get
			{
				if(null == _Helper) {
					_Helper = new MappingHelper(Container.EntityName, MemberEntityName);
				}
				return _Helper;
			}
		}
	}
}

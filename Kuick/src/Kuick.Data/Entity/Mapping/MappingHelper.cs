// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MappingHelper.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class MappingHelper<TContainer, TMember>
		where TContainer : class, IEntity, new()
		where TMember : class, IEntity, new()
	{
		public int Count(string containerID)
		{
			return DataCurrent.Mapping.Count<TContainer, TMember>(
				containerID
			);
		}

		public bool Exists(string containerID, string memberID)
		{
			return DataCurrent.Mapping.Exists<TContainer, TMember>(
				containerID, memberID
			);
		}

		public Result Add(string containerID, params string[] memberIDs)
		{
			return DataCurrent.Mapping.Add<TContainer, TMember>(
				containerID, memberIDs
			);
		}

		public Result Remove(string containerID, params string[] memberIDs)
		{
			return DataCurrent.Mapping.Remove<TContainer, TMember>(
				containerID, memberIDs
			);
		}

		public Result Clear(string containerID)
		{
			return DataCurrent.Mapping.Clear<TContainer, TMember>(containerID);
		}

		public void Sorting(params string[] ids)
		{
			DataCurrent.Mapping.Sorting<TContainer>(ids);
		}

		public void Sorting(string containerID, params string[] memberIDs)
		{
			DataCurrent.Mapping.Sorting<TContainer, TMember>(containerID, memberIDs);
		}

		public List<TMember> Get(params string[] containerIDs)
		{
			return DataCurrent.Mapping.Get<TContainer, TMember>(containerIDs);
		}

		public List<TMember> Get(
			Func<Sql<TMember>, Sql<TMember>> sqlIntercepter, 
			params string[] containerIDs)
		{
			return DataCurrent.Mapping.Get<TContainer, TMember>(
				sqlIntercepter, containerIDs
			);
		}

		public List<string> AllMemberIDs(
			string containerID)
		{
			return DataCurrent.Mapping.AllMemberIDs<TContainer, TMember>(
				containerID
			);
		}

		public Sql<TMember> GetSql(params string[] containerIDs)
		{
			return DataCurrent.Mapping.GetSql<TContainer, TMember>(containerIDs);
		}
	}

	public class MappingHelper
	{
		public MappingHelper(string containerEntityName, string memberEntityName)
		{
			this.ContainerEntityName = containerEntityName;
			this.MemberEntityName = memberEntityName;
		}

		public string ContainerEntityName { get; private set; }
		public string MemberEntityName { get; private set; }

		public int Count(string containerID)
		{
			return DataCurrent.Mapping.Count(
				ContainerEntityName, MemberEntityName, containerID
			);
		}

		public bool Exists(string containerID, string memberID)
		{
			return DataCurrent.Mapping.Exists(
				ContainerEntityName, MemberEntityName, containerID, memberID
			);
		}

		public Result Add(string containerID, params string[] memberIDs)
		{
			return DataCurrent.Mapping.Add(
				ContainerEntityName, MemberEntityName, containerID, memberIDs
			);
		}

		public Result Remove(string containerID, params string[] memberIDs)
		{
			return DataCurrent.Mapping.Remove(
				ContainerEntityName, MemberEntityName, containerID, memberIDs
			);
		}

		public Result Clear(string containerID)
		{
			return DataCurrent.Mapping.Clear(
				ContainerEntityName, MemberEntityName, containerID
			);
		}

		public void Sorting(
			string containerEntityName,
			params string[] ids)
		{
			DataCurrent.Mapping.Sorting(containerEntityName, ids);
		}

		public void Sorting(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			params string[] memberIDs)
		{
			DataCurrent.Mapping.Sorting(
				containerEntityName, memberEntityName, containerID, memberIDs
			);
		}

		public List<IEntity> Get(params string[] containerIDs)
		{
			return DataCurrent.Mapping.Get(
				ContainerEntityName, MemberEntityName, containerIDs
			);
		}

		public List<IEntity> Get(
			Func<Sql, Sql> sqlIntercepter, 
			params string[] containerIDs)
		{
			return DataCurrent.Mapping.Get(
				ContainerEntityName, MemberEntityName, sqlIntercepter, containerIDs
			);
		}

		public List<string> AllMemberIDs(
			string containerID)
		{
			return DataCurrent.Mapping.AllMemberIDs(
				ContainerEntityName, MemberEntityName, containerID
			);
		}
	}
}

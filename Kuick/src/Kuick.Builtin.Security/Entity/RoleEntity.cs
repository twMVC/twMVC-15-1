// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// RoleEntity.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-13 - Creation


using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Kuick.Data;
using Kuick.Web;
using System.Collections.Generic;

namespace Kuick.Builtin.Security
{
	[DataContract]
	[EntitySpec]
	public class RoleEntity
		: ObjectEntity<RoleEntity>, IRole
	{
		#region Constants
		public class Schema
		{
			public const string TableName = "KUICK_ROLE";

			public const string RoleID = "ROLE_ID";
			public const string Title = "TITLE";
		}
		#endregion

		#region Constructors
		public RoleEntity()
			: base()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string RoleID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Title { get; set; }
		#endregion

		#region IEntity
		#endregion

		#region Class Level
		#endregion

		#region Instance Level
		private List<IUser> _Users;
		public List<IUser> Users 
		{
			get {
				if(null == _Users) {
					_Users = Mapping<UserEntity>().Get()
						.ConvertAll<IUser>(x => x as IUser);
				}
				return _Users;
			}
		}
		#endregion

		#region Event Handler
		#endregion
	}
}

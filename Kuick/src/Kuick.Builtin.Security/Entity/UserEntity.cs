// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// UserEntity.cs
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
	public class UserEntity
		: ObjectEntity<UserEntity>, IUser
	{
		#region Constants
		public class Schema
		{
			public const string TableName = "KUICK_USER";

			public const string UserID= "USER_ID";
			public const string Name = "NAME";
			public const string Email= "EMAIL";
			public const string Password= "PASSWORD";
		}
		#endregion

		#region Constructors
		public UserEntity()
			: base()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		public string UserID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Name { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Email { get; set; }

		[DataMember]
		[ColumnSpec(50)]
		public string Password { get; set; }
		#endregion

		#region IEntity
		#endregion

		#region Class Level
		#endregion

		#region Instance Level
		private List<IRole> _Roles;
		public List<IRole> Roles 
		{
			get {
				if(null == _Roles) {
					_Roles = Mapping<RoleEntity>()
						.Get()
						.ConvertAll<IRole>(x => x as IRole);
				}
				return _Roles;
			}
		}

		public Result Signin(string password)
		{
			if(string.IsNullOrEmpty(password) && string.IsNullOrEmpty(Password)) {
				return new Result(true);
			}
			if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(Password)) {
				return new Result(
					false,
					"Password can't be empty."
				);
			}
			return new Result(
				Password.Equals(password, StringComparison.Ordinal)
			);
		}
		#endregion

		#region Event Handler
		#endregion
	}
}

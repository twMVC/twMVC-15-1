using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Kuick.Data;
using Kuick;

namespace KuickSample
{
	[EntitySpec]
	public class MEntity : ObjectEntity<MEntity>
	{
		public class Schema {
			public const string TableName = "T_M";

			public const string Key1 = "KEY_1";
			public const string Key2 = "KEY_2";
			public const string Name = "NAME";
		}

		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		public string Key1 { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		public int Key2 { get; set; }

		[DataMember]
		[ColumnSpec]
		public string Name { get; set; }
	}
}

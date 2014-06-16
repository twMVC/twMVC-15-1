using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Kuick.Data;
using Kuick;

namespace KuickSample
{
	[EntitySpec]
	public class UserEntity : ObjectEntity<UserEntity>
	{
		// UserName 長度至少 4 碼
		[DataMember]
		[Description("姓名")]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ValidationString(BoundaryString.LengthGreaterThan, 3)]
		public string UserName { get; set; }

		// 生日限制小於 2012-11-05
		[DataMember]
		[Description("生日")]
		[ColumnSpec]
		[ColumnVisual(VisualInput.Date)]
		public DateTime Birthday { get; set; }

		[DataMember]
		[Description("就寢時間")]
		[ColumnSpec]
		[ColumnVisual(VisualInput.Time)]
		public string BedTime { get; set; }

		// Level 值限定在 0 ~ 9 之間
		[DataMember]
		[Description("階級")]
		[ColumnSpec]
		//[ValidationInteger(BoundaryInteger.Between, 0, 9)]
		public int Level { get; set; }

		// Email 格式驗證
		[DataMember]
		[Description("電郵")]
		[ColumnSpec]
		//[ValidationString(BoundaryString.IsEmailAddress)]
		public string Email { get; set; }

		[DataMember]
		[Description("備註")]
		[ColumnSpec]
		[ColumnVisual(VisualInput.HtmlEditor)]
		public string Note { get; set; }

		[DataMember]
		[Description("全名")]
		[ColumnSpec]
		[ColumnVisual(VisualSize.XXLarge)]
		public string FullName { get; set; }

		[DataMember]
		[Description("性別")]
		[ColumnSpec]
		public Gender Gender { get; set; }

		[DataMember]
		[Description("啟用")]
		[ColumnSpec]
		[ColumnVisual("啟用", "停用")]
		public bool Actived { get; set; }

		public override bool EnableDynamic
		{
			get
			{
				return false;
			}
		}
	}
}

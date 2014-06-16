// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Table.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class Table
	{
		public Table()
		{
		}

		#region property
		public string EntityName { get; internal set; }
		public string TableName { get; internal set; }

		public CategoryAttribute Category { get; internal set; }
		public DescriptionAttribute Description { get; internal set; }
		public Type Class { get; internal set; }

		// Spec
		public EntitySpec Spec { get; internal set; }

		// Logic
		public List<EntityIndex> Indexes { get; internal set; }
		public List<EntityMapping> Mappings { get; internal set; }

		// Visual
		public EntityVisual Visual { get; internal set; }

		public List<IEntity> Refers { get; internal set; }

		// Parent
		public IEntity Schema { get; internal set; }

		// Diff
		public FollowDiff FollowDiff { get; internal set; }

		// Title
		public string Title
		{
			get
			{
				return null == Description 
					? EntityName 
					: Description.Description;
			}
		}
		#endregion

		#region method
		public string BuildIndexName(EntityIndex index)
		{
			int i = 0;
			foreach(EntityIndex x in Indexes) {
				i++;
				if(x.Equals(index)) {
					return string.Format("{0}_Index_{1}", TableName, i);
				}
			}

			throw new ArgumentException(
				"{0}: This index doesnot belongs to current table.",
				index.ToString()
			);
		}
		#endregion
	}
}

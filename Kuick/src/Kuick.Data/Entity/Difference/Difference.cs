// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Difference.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-15 - Creation


using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Kuick.Data
{
	public class Difference
	{
		public Difference() {
			this.Values = new List<DiffValue>();
		}

		public string EntityName { get; set; }
		public List<DiffValue> Values { get; set; }

		private IEntity _Schema;
		[XmlIgnore]
		public IEntity Schema {
			get
			{
				if(null == _Schema) {
					_Schema = EntityCache.Get(EntityName);
				}
				return _Schema;
			}
		}

		public static Action<DiffMethod, IEntity, IEntity> Handler { get; set; }
	}
}

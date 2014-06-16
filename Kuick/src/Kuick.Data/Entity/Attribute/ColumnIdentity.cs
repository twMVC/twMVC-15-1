// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnIdentity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-12-10 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ColumnIdentity : Attribute, ICloneable<ColumnIdentity>
	{
		#region constructor
		public ColumnIdentity()
			: this(Frequency.Once, 1, 1)
		{
		}
		public ColumnIdentity(Frequency frequency, int start, int incremental)
			: this(false, 0, frequency, start, incremental)
		{
		}
		public ColumnIdentity(
			bool addPrefix, int fixedLength, Frequency frequency, int start, int incremental)
		{
			this.AddPrefix = addPrefix;
			this.FixedLength = fixedLength;
			this.Frequency = frequency;
			this.Start = start;
			this.Incremental = incremental;
		}
		#endregion

		#region ICloneable<T>
		public ColumnIdentity Clone()
		{
			ColumnIdentity clone = new ColumnIdentity();
			clone.IdentityID = IdentityID;
			clone.FixedLength = FixedLength;
			clone.AddPrefix = AddPrefix;
			clone.Frequency = Frequency;
			clone.Start = Start;
			clone.Incremental = Incremental;
			clone.Column = Column;
			return clone;
		}
		#endregion

		#region property
		public string IdentityID { get; internal set; }
		public bool AddPrefix { get; internal set; }
		public int FixedLength { get; internal set; }
		public Frequency Frequency { get; internal set; }
		public int Start { get; internal set; }
		public int Incremental { get; internal set; }

		// Parent
		public Column Column { get; internal set; }
		#endregion
	}
}

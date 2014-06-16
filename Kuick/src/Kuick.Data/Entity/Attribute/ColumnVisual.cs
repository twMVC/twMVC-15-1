// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnVisual.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnVisual : Attribute, ICloneable<ColumnVisual>
	{
		#region constructor
		public ColumnVisual()
			: this(VisualFlag.Normal, VisualInput.TextBox, VisualSize.Medium)
		{
		}

		public ColumnVisual(VisualFlag flag)
			: this(flag, VisualInput.TextBox, VisualSize.Medium)
		{
		}

		public ColumnVisual(VisualInput input)
			: this(VisualFlag.Normal, input, VisualSize.Medium)
		{
		}

		public ColumnVisual(VisualSize size)
			: this(VisualFlag.Normal, VisualInput.TextBox, size)
		{
		}

		public ColumnVisual(VisualFlag flag, VisualInput input)
			: this(flag, input, VisualSize.Medium)
		{
		}

		public ColumnVisual(VisualFlag flag, VisualSize size)
			: this(flag, VisualInput.TextBox, size)
		{
		}

		public ColumnVisual(VisualFlag flag, VisualInput input, VisualSize size)
			: this(flag, input, size, "True", "False")
		{
		}

		public ColumnVisual(string trueText, string falseText)
			: this(
				VisualFlag.Normal, 
				VisualInput.TextBox, 
				VisualSize.Medium, 
				trueText, 
				falseText)
		{
		}

		public ColumnVisual(VisualFlag flag, string trueText, string falseText)
			: this(flag, VisualInput.TextBox, VisualSize.Medium, trueText, falseText)
		{
		}

		public ColumnVisual(VisualInput input, string trueText, string falseText)
			: this(VisualFlag.Normal, input, VisualSize.Medium, trueText, falseText)
		{
		}

		public ColumnVisual(
			VisualFlag flag, 
			VisualInput input, 
			VisualSize size, 
			string trueText, 
			string falseText)
		{
			this.Flag = flag;
			this.Input = input;
			this.Size = size;
			this.TrueText = trueText;
			this.FalseText = falseText;

			this.ShowBulkEdit = Checker.Flag.Check((int)flag, (int)VisualFlag.ShowBulkEdit);
			this.HideInAdd = Checker.Flag.Check((int)flag, (int)VisualFlag.HideInAdd);
			this.HideInModify = Checker.Flag.Check((int)flag, (int)VisualFlag.HideInModify);
			this.HideInList = Checker.Flag.Check((int)flag, (int)VisualFlag.HideInList);
			this.SystemColumn = Checker.Flag.Check((int)flag, (int)VisualFlag.SystemColumn);

			// correcting
			if(this.ShowBulkEdit) {
				this.HideInAdd = false;
				this.HideInModify = false;
			}
		}
		#endregion

		#region ICloneable<T>
		public ColumnVisual Clone()
		{
			return new ColumnVisual(Flag, Input, Size, TrueText, FalseText);
		}
		#endregion

		#region property
		public VisualFlag Flag { get; internal set; }
		public VisualInput Input { get; internal set; }
		public VisualSize Size { get; internal set; }

		public string TrueText { get; private set; }
		public string FalseText { get; private set; }

		// Parent
		public Column Column { get; internal set; }
		#endregion

		#region assistance
		public bool ShowBulkEdit { get; private set; }
		public bool HideInAdd { get; private set; }
		public bool HideInModify { get; private set; }
		public bool HideInList { get; private set; }
		public bool SystemColumn { get; private set; }
		#endregion
	}
}

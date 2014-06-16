// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Tf.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-27 - Creation


using System;
using Kuick.Data;
using System.Text;
using System.Linq.Expressions;

namespace Kuick.Web.UI
{
	public class Tf<T> where T : class ,IEntity, new()
	{
		public Tf()
			: this(null)
		{
		}
		public Tf(Expression<Func<T, object>> expression)
		{
			this.Th = new Th<T>();
			this.TTop = new Td<T>();
			this.Td = new Td<T>();
			this.TBottom = new Td<T>();

			if(null == expression) { return; }
			Column column = DataUtility.ToColumn<T>(expression);
			if(null == column) { return; }
			Th.TitleValue = column.Title;
			Td.ColumnName = column.Spec.ColumnName;
		}


		private Th<T> _Th;
		public Th<T> Th
		{
			get
			{
				return _Th;
			}
			set
			{
				Th<T> th = value;
				if(null != _Th) {
					if(th.CssClass.IsNullOrEmpty()) {
						th.CssClass = _Th.CssClass;
					}
					if(th.TitleValue.IsNullOrEmpty()) {
						th.TitleValue = _Th.TitleValue;
					}
				}
				_Th = th;
			}
		}

		private Td<T> _TTop;
		public Td<T> TTop
		{
			get
			{
				return _TTop;
			}
			set
			{
				Td<T> tTop = value;
				if(null != _TTop) {
					if(tTop.ColumnName.IsNullOrEmpty()) {
						tTop.ColumnName = _TTop.ColumnName;
					}
					if(tTop.CssClass.IsNullOrEmpty()) {
						tTop.CssClass = _TTop.CssClass;
					}
					if(null == tTop.GetValue) {
						tTop.GetValue = _TTop.GetValue;
					}
					if(null == tTop.GetClass) {
						tTop.GetClass = _TTop.GetClass;
					}
				}
				_TTop = tTop;
			}
		}

		private Td<T> _Td;
		public Td<T> Td
		{
			get
			{
				return _Td;
			}
			set
			{
				Td<T> td = value;
				if(null != _Td) {
					if(td.ColumnName.IsNullOrEmpty()) {
						td.ColumnName = _Td.ColumnName;
					}
					if(td.CssClass.IsNullOrEmpty()) {
						td.CssClass = _Td.CssClass;
					}
					if(null == td.GetValue) {
						td.GetValue = _Td.GetValue;
					}
					if(null == td.GetClass) {
						td.GetClass = _Td.GetClass;
					}
				}
				_Td = td;
			}
		}

		private Td<T> _TBottom;
		public Td<T> TBottom
		{
			get
			{
				return _TBottom;
			}
			set
			{
				Td<T> tBottom = value;
				if(null != _TBottom) {
					if(tBottom.ColumnName.IsNullOrEmpty()) {
						tBottom.ColumnName = _TBottom.ColumnName;
					}
					if(tBottom.CssClass.IsNullOrEmpty()) {
						tBottom.CssClass = _TBottom.CssClass;
					}
					if(null == tBottom.GetValue) {
						tBottom.GetValue = _TBottom.GetValue;
					}
					if(null == tBottom.GetClass) {
						tBottom.GetClass = _TBottom.GetClass;
					}
				}
				_TBottom = tBottom;

			}
		}
	}
}

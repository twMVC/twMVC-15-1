// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Attr.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-05 - Creation


using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kuick.Web.UI.Bootstrap
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class Attr : Attribute
	{
		public Attr()
		{
		}

		public Attr(AttributeName name)
		{
			this.AttributeName = name;
		}

		#region property
		// internal
		public TagName TagName { get; internal set; }
		public PropertyInfo Property { get; internal set; }

		private Nullable<AttributeName> _AttributeName = AttributeName.Non;
		public AttributeName AttributeName
		{
			get
			{
				if(_AttributeName.Value == AttributeName.Non) {
					try {
						AttributeName name = (AttributeName)Enum.Parse(
							typeof(AttributeName), Property.Name, true
						);
						_AttributeName = name;
					} catch(Exception ex) {
						Logger.Error(
							"Attr setting error",
							ex
						);
					}
				}
				return _AttributeName.Value;
			}
			private set
			{
				_AttributeName = value;
			}
		}

		public AttributeStyle AttributeStyle
		{
			get
			{
				switch(AttributeName) {
					// Non
					case AttributeName.Checked:
					case AttributeName.Disabled:
					case AttributeName.Nowrap:
					case AttributeName.Selected:
					case AttributeName.BtnCheckbox:
						return AttributeStyle.Non;
					// Multiple
					case AttributeName.Class:
					case AttributeName.Style:
						return AttributeStyle.Multiple;
					// Single
					case AttributeName.Alt:
					case AttributeName.Cols:
					case AttributeName.Colspan:
					case AttributeName.For:
					case AttributeName.Href:
					case AttributeName.Id:
					case AttributeName.Maxlength:
					case AttributeName.Multiple:
					case AttributeName.Name:
					case AttributeName.Placeholder:
					case AttributeName.Role:
					case AttributeName.Rows:
					case AttributeName.Rowspan:
					case AttributeName.Size:
					case AttributeName.Src:
					case AttributeName.TabIndex:
					case AttributeName.Target:
					case AttributeName.Title:
					case AttributeName.Type:
					case AttributeName.Value:
					case AttributeName.Width:
					//case AttributeName.NgModel:
					//case AttributeName.BtnCheckboxTrue:
					//case AttributeName.BtnCheckboxFalse:
					//case AttributeName.BtnRadio:
					//case AttributeName.AriaLabelledBy:
					//case AttributeName.AriaHidden:
					//case AttributeName.DataToggle:
					//case AttributeName.DataTarget:
					//case AttributeName.DataSrc:
					//case AttributeName.DataDismiss:
					//case AttributeName.DataParent:
					//case AttributeName.DataLoadingText:
					//case AttributeName.DataCompleteText:
					//case AttributeName.DataSlideTo:
					//case AttributeName.DataSlide:
					//case AttributeName.DataProvide:
					//case AttributeName.KuickEntityName:
					//case AttributeName.KuickKeyValue:
					//case AttributeName.KuickColumnName:
					//case AttributeName.KuickOriginalValue:
					default:
						return AttributeStyle.Single;
				}
			}
		}
		public string Separator
		{
			get
			{
				switch(AttributeName) {
					// Multiple
					case AttributeName.Class:
						return " ";
					case AttributeName.Style:
						return "; ";
					case AttributeName.Checked:
					case AttributeName.Disabled:
					case AttributeName.Multiple:
					case AttributeName.Nowrap:
					case AttributeName.Selected:
					case AttributeName.Alt:
					case AttributeName.Cols:
					case AttributeName.Colspan:
					case AttributeName.For:
					case AttributeName.Href:
					case AttributeName.Id:
					case AttributeName.Maxlength:
					case AttributeName.Name:
					case AttributeName.Placeholder:
					case AttributeName.Role:
					case AttributeName.Rows:
					case AttributeName.Rowspan:
					case AttributeName.Size:
					case AttributeName.Src:
					case AttributeName.TabIndex:
					case AttributeName.Target:
					case AttributeName.Title:
					case AttributeName.Type:
					case AttributeName.Value:
					case AttributeName.Width:
					//case AttributeName.NgModel:
					//case AttributeName.BtnCheckbox:
					//case AttributeName.BtnCheckboxTrue:
					//case AttributeName.BtnCheckboxFalse:
					//case AttributeName.BtnRadio
					//case AttributeName.AriaLabelledBy:
					//case AttributeName.AriaHidden:
					//case AttributeName.DataToggle:
					//case AttributeName.DataTarget:
					//case AttributeName.DataSrc:
					//case AttributeName.DataDismiss:
					//case AttributeName.DataParent:
					//case AttributeName.DataLoadingText:
					//case AttributeName.DataCompleteText:
					//case AttributeName.DataSlideTo:
					//case AttributeName.DataSlide:
					//case AttributeName.DataProvide:
					//case AttributeName.KuickEntityName:
					//case AttributeName.KuickKeyValue:
					//case AttributeName.KuickColumnName:
					//case AttributeName.KuickOriginalValue:
					default:
						return string.Empty;
				}
			}
		}
		#endregion

		#region function
		internal void Foolproof()
		{
			if(null == Property) { throw new ArgumentException("Need PropertyInfo!"); }
			bool error = false;
			switch(AttributeName) {
				// int
				case AttributeName.Cols:
				case AttributeName.Colspan:
				case AttributeName.Maxlength:
				case AttributeName.Rows:
				case AttributeName.Rowspan:
				case AttributeName.Size:
				case AttributeName.TabIndex:
				case AttributeName.DataSlideTo:
					if(!Property.PropertyType.IsInteger()) {
						error = true;
					}
					break;
				// List<string>
				case AttributeName.Class:
				case AttributeName.Style:
					if(!Property.PropertyType.Equals(typeof(List<string>))) {
						error = true;
					}
					break;
				// bool
				case AttributeName.Checked:
				case AttributeName.Disabled:
				case AttributeName.Multiple:
				case AttributeName.Nowrap:
				case AttributeName.Selected:
				case AttributeName.AriaHidden:
				case AttributeName.BtnCheckbox:
				case AttributeName.Required:
					if(!Property.PropertyType.IsBoolean()) {
						error = true;
					}
					break;
				// string
				case AttributeName.Alt:
				case AttributeName.For:
				case AttributeName.Href:
				case AttributeName.Id:
				case AttributeName.Name:
				case AttributeName.Placeholder:
				case AttributeName.Role:
				case AttributeName.Src:
				case AttributeName.Target:
				case AttributeName.Title:
				case AttributeName.Type:
				case AttributeName.Value:
				case AttributeName.Width:
				//case AttributeName.NgModel:
				//case AttributeName.BtnCheckboxTrue:
				//case AttributeName.BtnCheckboxFalse:
				//case AttributeName.BtnRadio
				//case AttributeName.AriaLabelledBy:
				//case AttributeName.DataToggle:
				//case AttributeName.DataTarget:
				//case AttributeName.DataSrc:
				//case AttributeName.DataDismiss:
				//case AttributeName.DataParent:
				//case AttributeName.DataLoadingText:
				//case AttributeName.DataCompleteText:
				//case AttributeName.DataSlide:
				//case AttributeName.DataProvide:
				//case AttributeName.KuickEntityName:
				//case AttributeName.KuickKeyValue:
				//case AttributeName.KuickColumnName:
				//case AttributeName.KuickOriginalValue:
				default:
					if(!Property.PropertyType.IsString()) {
						error = true;
					}
					break;
			}

			if(error) {
				throw new KException(string.Format(
					"{0}.{1} property type doesn't match the attr of '{2}'.",
					TagName,
					Property.Name,
					AttributeName
				));
			}
		}
		#endregion
	}
}

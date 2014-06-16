// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-03 - Creation


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Kuick.Web.UI.Bootstrap
{
	public abstract class HtmlTag : IHtmlTag
	{
		private static object _Lock = new object();

		#region constructor
		public HtmlTag()
		{
		}
		#endregion

		#region property
		// virtual
		private Nullable<TagName> _TagName = TagName.Non;
		public virtual TagName TagName
		{
			get
			{
				if(_TagName.Value == TagName.Non) {
					try {
						TagName tagName = (TagName)Enum.Parse(
							typeof(TagName), this.GetType().Name.TrimEnd("Tag"), true
						);
						_TagName = tagName;
					} catch(Exception ex) {
						Logger.Error(
							"HtmlTag class name naming error.",
							ex.ToAny(
								new Any("Class Name", this.GetType().Name)
							)
						);
					}
				}
				return _TagName.Value;
			}
		}

		public virtual bool WithCloseTag { get { return false; } }
		public virtual bool NeedClose { get { return true; } }
		private bool Closed { get; set; }

		//
		protected StringBuilder Builder { get; set; }

		private List<Attr> _Attrs;
		[XmlIgnore]
		public List<Attr> Attrs
		{
			get
			{
				if(null == _Attrs) {
					lock(_Lock) {
						if(null == _Attrs) {
							if(Heartbeat.Singleton.PostStartFinished) {
								IHtmlTag tag = HtmlTagCache.Get(TagName);
								if(null == tag) {
									throw new NullReferenceException();
								}
								_Attrs = tag.Attrs;
							} else {
								_Attrs = new List<Attr>();
								PropertyInfo[] infos = this.GetType().GetProperties();
								foreach(PropertyInfo info in infos) {
									object[] objs = info.GetCustomAttributes(true);
									Attr attr = null;
									foreach(object x in objs) {
										if(x is Attr) {
											attr = (Attr)x;
											break;
										}
									}

									if(null == attr) {
										continue;
									} else {
										attr.TagName = TagName;
										attr.Property = info;
										attr.Foolproof();
										_Attrs.Add(attr);
									}
								}
							}
						}
					}
				}
				return _Attrs;
			}
		}
		#endregion

		#region function
		public Attr GetAttr(string propertyName)
		{
			Attr attr = Attrs.Find(x => x.Property.Name == propertyName);
			return attr;
		}

		public object GetAttrValue(string propertyName)
		{
			Attr attr = GetAttr(propertyName);
			return GetAttrValue(attr);
		}

		public object GetAttrValue(Attr attr)
		{
			object val = Reflector.GetValue(attr.Property, this);
			return val;
		}

		protected string ParseTagName()
		{
			string name = TagName.ToString().ToLower();
			return name;
		}

		protected string ParseAttributeName(Attr attr)
		{
			string name = attr.AttributeName.ToString().ToLower();
			if(attr.AttributeName == AttributeName.DataLoadingText) {
				name = "data-loading-text";
			} else if(attr.AttributeName == AttributeName.DataCompleteText) {
				name = "data-complete-text";
			} else if(attr.AttributeName == AttributeName.DataSlideTo) {
				name = "data-slide-to";
			} else if(attr.AttributeName == AttributeName.BtnRadio) {
				name = "btn-radio";
			} else if(attr.AttributeName == AttributeName.BtnCheckbox) {
				name = "btn-checkbox";
			} else if(attr.AttributeName == AttributeName.BtnCheckboxTrue) {
				name = "btn-checkbox-true";
			} else if(attr.AttributeName == AttributeName.BtnCheckboxFalse) {
				name = "btn-checkbox-false";
			} else if(name.StartWith("ng")) {
				name.Insert(2, "-");
			} else if(name.StartWith("aria")) {
				name.Insert(4, "-");
			} else if(name.StartWith("data")) {
				name.Insert(4, "-");
			} else if(name.StartWith("kuick")) {
				name.Insert(5, "-");
			} else if(name.StartWith("btn")) {
				name.Insert(3, "-");
			}
			return name;
		}

		public virtual void Render(StringBuilder sb)
		{
			if(null == sb) {
				throw new ArgumentNullException("StringBuilder");
			}
			Builder = sb;

			sb.AppendFormat("<{0}", ParseTagName());

			if(WithCloseTag) {
				sb.Append(">");
				RenderCloseTag();
			} else {
				if(NeedClose) {
					sb.Append("/>");
				} else {
					sb.Append(">");
				}
			}
		}

		internal void RenderCloseTag()
		{
			if(Closed) { return; }

			if(null == Builder) {
				throw new KException(
					"Invoke Render before RenderCloseTag."
				);
			}

			Builder.AppendFormat("</{0}>", ParseTagName());
			Closed = true;
		}

		internal void RenderAttr(Attr attr)
		{
			if(null == Builder) {
				throw new KException(
					"Invoke Render before RenderCloseTag."
				);
			}

			object value = GetAttrValue(attr);
			switch(attr.AttributeName) {
				case AttributeName.Checked:
				case AttributeName.Disabled:
				case AttributeName.Nowrap:
				case AttributeName.Selected:
				case AttributeName.BtnCheckbox:
					if(!(bool)value) { break; }
					RenderAttrMain(attr);
					break;
				case AttributeName.AriaHidden:
					if(!(bool)value) { break; }
					RenderAttrMain(attr, "true");
					break;
				case AttributeName.Class:
				case AttributeName.Style:
					if(null == value) { break; }
					List<string> values = value as List<string>;
					if(null == values) {
						throw new ArrayTypeMismatchException();
					}
					if(values.Count == 0) { break; }
					RenderAttrMain(attr, values);
					break;
				case AttributeName.Cols:
				case AttributeName.Colspan:
				case AttributeName.Maxlength:
				case AttributeName.Rows:
				case AttributeName.Rowspan:
				case AttributeName.Size:
				case AttributeName.Width:
					if((int)value < 1) { break; }
					RenderAttrMain(attr, value);
					break;
				case AttributeName.DataSlideTo:
					if((int)value < 0) { break; }
					RenderAttrMain(attr, value);
					break;
				case AttributeName.Multiple:
					if(!(bool)value) { break; }
					RenderAttrMain(attr, "multiple");
					break;
				case AttributeName.Alt:
				case AttributeName.For:
				case AttributeName.Href:
				case AttributeName.Id:
				case AttributeName.Name:
				case AttributeName.Placeholder:
				case AttributeName.Src:
				case AttributeName.Target:
				case AttributeName.Title:
				case AttributeName.Type:
				case AttributeName.Value:
				case AttributeName.NgModel:
				case AttributeName.BtnCheckboxTrue:
				case AttributeName.BtnCheckboxFalse:
				case AttributeName.AriaLabelledBy:
				case AttributeName.DataToggle:
				case AttributeName.DataTarget:
				case AttributeName.DataSrc:
				case AttributeName.DataDismiss:
				case AttributeName.DataParent:
				case AttributeName.DataLoadingText:
				case AttributeName.DataCompleteText:
				case AttributeName.DataSlide:
				case AttributeName.DataProvide:
				case AttributeName.KuickEntityName:
				case AttributeName.KuickKeyValue:
				case AttributeName.KuickColumnName:
				case AttributeName.KuickOriginalValue:
				default:
					if(null == value) { break; }
					if(string.IsNullOrEmpty(value.ToString())) { break; }
					RenderAttrMain(attr, value);
					break;
			}
		}

		private const string ATTR_PATTERN = "{0}=\"{1}\"";
		private void RenderAttrMain(Attr attr)
		{
			Builder.Append(ParseAttributeName(attr));
		}
		private void RenderAttrMain(Attr attr, object value)
		{
			Builder.AppendFormat(
				ATTR_PATTERN,
				ParseAttributeName(attr),
				value
			);
		}
		private void RenderAttrMain(Attr attr, List<string> values)
		{
			Builder.AppendFormat(
				ATTR_PATTERN,
				ParseAttributeName(attr),
				values.Distinct().Join(attr.Separator)
			);
		}
		#endregion
	}
}

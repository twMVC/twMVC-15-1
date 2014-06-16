// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EditPage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-23 - Creation


using System;
using Kuick.Data;

namespace Kuick.Web.UI
{
	public class EditPage : PageBase
	{
		#region const
		public const string ENTITY_NAME = "EntityName";
		public const string KEY_VALUE = "KeyValue";
		public const string EDIT_MODE = "EditMode";
		public const string DEFAULT_VALUES = "DefaultValues";
		#endregion

		#region properties
		public override bool RenderJQueryFile { get { return false; } }

		public virtual string EntityName { get { return string.Empty; } }

		[RequestParameter(KEY_VALUE)]
		public string KeyValue { get; set; }

		[RequestParameter(EDIT_MODE)]
		public EditMode Mode { get; set; }

		[RequestParameter(DEFAULT_VALUES)]
		public string DefaultValues { get; set; }

		private Any[] _CurrentAnys;
		protected Any[] CurrentAnys
		{
			get
			{
				if(null == _CurrentAnys) {
					Anys anys = new Anys();
					anys.Add(ENTITY_NAME, EntityName);
					anys.Add(KEY_VALUE, KeyValue);
					anys.Add(EDIT_MODE, Mode);
					_CurrentAnys = anys.ToArray();
				}
				return _CurrentAnys;
			}
		}
		#endregion

		#region virtual
		public virtual Anys Defaults
		{
			get
			{
				if(Mode == EditMode.Add && !string.IsNullOrEmpty(DefaultValues)) {
					// UrlEncode, UrlDecode
					return new Anys(
						DefaultValues,              // DefaultValues=Name1=Value1&Name2=Value2
						Constants.Symbol.Ampersand, // &
						Constants.Symbol.Equal      // =
					);
				}
				return null;
			}
		}
		#endregion
	}

	public class SemiEditPage<T>
		: EditPage
		where T : class, IEntity, new()
	{
		public override string EntityName { get { return typeof(T).Name; } }

		public T Instance { get; private set; }

		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);

			Instance = Entity<T>.Get(KeyValue);
		}
	}

	public class EditPage<T>
		: EditPage
		where T : class, IEntity, new()
	{
		public override string EntityName { get { return typeof(T).Name; } }

		public T Instance { get; set; }

		protected virtual void btnSubmit_Click(object sender, EventArgs e)
		{
			T one;
			DataResult result;

			switch(Mode) {
				case EditMode.Add:
					// new
					one = new T();

					// set
					Parameter.RequestEntity<T>(one);

					// event
					BeforeAdd(one);

					// deal
					result = one.Add();
					if(!result.Success) {
						Alert(result.Message);
						return;
					}

					// event
					AfterAdd(one);

					break;
				case EditMode.Edit:
					// assign
					one = Entity<T>.Get(KeyValue);

					// set
					Parameter.RequestEntity<T>(one);

					// event
					BeforeEdit(one);

					// deal
					result = one.Modify();
					if(!result.Success) {
						Alert(result.Message);
						return;
					}

					// event
					AfterEdit(one);

					break;
				case EditMode.Remove:
					// event
					BeforeRemove();

					// deal
					result = Entity<T>.Remove(KeyValue);
					if(!result.Success) {
						Alert(result.Message);
						return;
					}

					// event
					AfterRemove();

					break;
				case EditMode.Clone:
					Instance = Entity<T>.Get(KeyValue);

					// event
					BeforeClone();

					// deal

					// event
					AfterClone();

					break;
				case EditMode.Move:
					Instance = Entity<T>.Get(KeyValue);

					// event
					BeforeMove();

					// deal

					// event
					AfterMove();

					break;
				default:
					break;
			}
		}

		#region event handler
		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);

			if(!IsPostBack) {
				switch(Mode) {
					case EditMode.Add:
						Instance = new T();
						BeforeRenderAdd(Instance); // event
						break;
					case EditMode.Edit:
					case EditMode.Sort:
					case EditMode.Remove:
						Instance = Entity<T>.Get(KeyValue);
						if(null == Instance) {
							string msg = "Can not find this record";
							Logger.Error(
								"EditPage",
								msg,
								CurrentAnys
							);
							throw new Exception(msg);
						}
						BeforeRenderEdit(Instance); // event
						break;
					case EditMode.Clone:
						if(null == Instance) { Instance = Entity<T>.Get(KeyValue); }
						BeforeRenderClone(Instance);
						break;
					case EditMode.Move:
						if(null == Instance) { Instance = Entity<T>.Get(KeyValue); }
						BeforeRenderMove(Instance);
						break;
					default:
						throw new NotImplementedException("Not implemented!");
				}
				WebTools.Bind(this, Instance);

				// default values
				if(Mode == EditMode.Add) {
					Anys anys = Defaults;
					if(null != anys) { Instance.Bind(anys.ToArray()); }
				}
			}
		}

		// Add
		public virtual void BeforeRenderAdd(T one)
		{
			// do nothing
		}
		public virtual void BeforeAdd(T one)
		{
			// do nothing
		}
		public virtual void AfterAdd(T one)
		{
			JsParentReload();
		}

		// Edit
		public virtual void BeforeRenderEdit(T one)
		{
			// do nothing
		}
		public virtual void BeforeEdit(T one)
		{
			// do nothing
		}
		public virtual void AfterEdit(T one)
		{
			JsParentReload();
		}

		// Remove
		public virtual void BeforeRemove()
		{
			// do nothing
		}
		public virtual void AfterRemove()
		{
			JsParentReload();
		}

		// Clone
		public virtual void BeforeRenderClone(T one)
		{
			// do nothing
		}
		public virtual void BeforeClone()
		{
			// do nothing
		}
		public virtual void AfterClone()
		{
			JsParentReload();
		}

		// Move
		public virtual void BeforeRenderMove(T one)
		{
			// do nothing
		}
		public virtual void BeforeMove()
		{
			// do nothing
		}
		public virtual void AfterMove()
		{
			JsParentReload();
		}
		#endregion
	}
}

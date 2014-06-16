// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HierarchyEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Kuick.Data
{
	//[EntityIndex(true, PARENT_ID, TITLE)]
	[DataContract]
	[EntityIndex(false, PARENT_ID)]
	public class HierarchyEntity<T>
		: ObjectEntity<T>, IHierarchyEntity<T>
		where T : HierarchyEntity<T>, new()
	{
		#region Schema
		public const string PARENT_ID = "PARENT_ID";
		public const string TITLE = "TITLE";
		public const string LEFT = "LEFT";
		public const string RIGHT = "RIGHT";
		#endregion

		#region constructor
		public HierarchyEntity()
			: base()
		{
		}
		#endregion

		#region property
		[DataMember]
		[ColumnSpec(256)]
		[ColumnRefer(ReferValue.Self)]
		public string ParentID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 512)]
		public string Title { get; set; }

		[DataMember]
		[Category(DataConstants.Entity.Category)]
		[ColumnSpec]
		[ColumnVisual(VisualFlag.SystemColumn)]
		[IgnoreDiff]
		public int Left { get; set; }

		[DataMember]
		[Category(DataConstants.Entity.Category)]
		[ColumnSpec]
		[ColumnVisual(VisualFlag.SystemColumn)]
		[IgnoreDiff]
		public int Right { get; set; }
		#endregion

		#region IHierarchy
		public bool IsRoot { get { return string.IsNullOrEmpty(ParentID); } }
		public int Level { get { return Parents.Count; } }
		public bool HasChild { get { return Children.Count > 0; } }

		public bool HasNext
		{
			get
			{
				bool find = false;
				foreach(T one in Siblings) {
					if(find) { return true; }
					if(one.KeyValue == KeyValue) { find = true; }
				}
				return false;
			}
		}

		public List<string> Path
		{
			get
			{
				List<string> list = new List<string>();
				foreach(T one in Parents) {
					list.Add(one.KeyValue);
				}
				return list;
			}
		}

		public void Regulating()
		{
			int index = 0;
			Regulating(null, ref index);
		}

		private void Regulating(T instance, ref int index)
		{
			if(Heartbeat.Singleton.DatabaseStartFinished) { return; }
			List<T> list = null == instance ? Roots : instance.Children;

			if(null != instance) {
				instance.Left = ++index;
			}
			foreach(T one in list) {
				Regulating(one, ref index);
			}
			if(null != instance) {
				instance.Right = ++index;
				instance.SkipUpdateModifiedDate = true;
				instance.Modify();
			}
		}

		private T _Parent;
		public T Parent
		{
			get
			{
				if(Checker.IsNull(_Parent)) {
					if(ParentID != KeyValue) {
						if(null == _Nodes) {
							_Parent = Get(ParentID);
						} else {
							_Parent = Nodes.Find(delegate(T x) {
								return x.KeyValue == ParentID;
							});
						}
					}
				}
				return _Parent;
			}
		}

		private T _Next;
		public T Next
		{
			get
			{
				if(null == _Next) {
					bool find = false;
					foreach(T one in Siblings) {
						if(find) {
							_Next = one;
							break;
						}
						if(one.KeyValue == KeyValue) { find = true; }
					}
				}
				return _Next;
			}
		}

		private T _Previous;
		public T Previous
		{
			get
			{
				if(null == _Previous) {
					bool find = false;
					T previous = null;
					foreach(T one in Siblings) {
						if(one.KeyValue == KeyValue) { find = true; break; }
						previous = one;
					}
					_Previous = find ? previous : null;
				}
				return _Previous;
			}
		}

		private List<T> _Nodes;
		public List<T> Nodes
		{
			get
			{
				if(Checker.IsNull(_Nodes)) {
					// Cache ...
					_Nodes = Sql().Query();
					_Nodes = SortAll(_Nodes);
				}
				return _Nodes;
			}
			private set
			{
				_Nodes = value;
			}
		}

		private List<T> _Roots;
		public List<T> Roots
		{
			get
			{
				if(null == _Roots) {
					if(null == _Nodes) {
						_Roots = Query(x =>
							x.ParentID == ""
							|
							x.ParentID == null
						);
					} else {
						_Roots = Nodes.FindAll(delegate(T x) {
							return Checker.IsNull(x.ParentID);
						});
					}
				}
				return _Roots;
			}
		}

		private List<T> _Parents;
		public List<T> Parents
		{
			get
			{
				if(null == _Parents) {
					lock(_Lock) {
						if(null == _Parents) {
							_Parents = new List<T>();
							T parent = Parent;
							while(null != parent) {
								_Parents.Add(parent);
								parent = parent.Parent;
							}
							_Parents.Reverse();
						}
					}
				}
				return _Parents;
			}
		}

		private List<T> _ParentsWithSelf;
		public List<T> ParentsWithSelf
		{
			get
			{
				if(null == _ParentsWithSelf) {
					lock(_Lock) {
						if(null == _ParentsWithSelf) {
							_ParentsWithSelf = new List<T>();
							T parent = (T)this;
							while(null != parent) {
								_ParentsWithSelf.Add(parent);
								parent = parent.Parent;
							}
							_ParentsWithSelf.Reverse();
						}
					}
				}
				return _ParentsWithSelf;
			}
		}

		private List<T> _Siblings;
		public List<T> Siblings
		{
			get
			{
				if(null == _Siblings) {
					lock(_Lock) {
						if(null == _Siblings) {
							if(null == Nodes) {
								_Siblings = Query(x =>
									x.ParentID == ParentID
								);
							} else {
								_Siblings = Nodes.FindAll(delegate(T x) {
									return x.ParentID == ParentID;
								});
							}
						}
					}
				}
				return _Siblings;
			}
		}

		private List<T> _Children;
		public List<T> Children
		{
			get
			{
				if(null == _Children) {
					lock(_Lock) {
						if(null == _Children) {
							if(null == Nodes) {
								_Children = Query(x =>
									x.ParentID == KeyValue
								);
							} else {
								_Children = Nodes.FindAll(delegate(T x) {
									return x.ParentID == KeyValue;
								});
							}
						}
					}
				}
				return _Children;
			}
		}

		private List<T> _Descendant;
		public List<T> Descendant
		{
			get
			{
				if(Checker.IsNull(_Descendant)) {
					List<T> list = new List<T>();
					List<T> children = Children;
					while(!Checker.IsNull(children)) {
						children = null;
						foreach(T one in children) {
							if(!list.Contains(one)) {
								one.Nodes = Nodes;
								list.Add(one);
								children.AddRange(one.Children);
							}
						}
					}
					_Descendant = list;
				}

				return _Descendant;
			}
		}
		#endregion

		#region IEntity
		public override string TitleValue
		{
			get
			{
				return Title;
			}
		}

		public override Flag Concurrency
		{
			get
			{
				return Kuick.Flag.Enable;
			}
		}

		public override void Interceptor(Sql sql)
		{
			sql.OrderBy(new SqlOrderBy(LEFT));
			base.Interceptor(sql);
		}
		#endregion

		#region class level
		public static List<T> AllRoots
		{
			get
			{
				List<T> roots = Query(x =>
					x.ParentID == ""
					|
					x.ParentID == null
				);
				return roots;
			}
		}

		public static List<T> SortAll(List<T> all)
		{
			if(Checker.IsNull(all)) { return new List<T>(); }

			List<T> list = new List<T>();
			SortAllMain(null, all, list);
			return list;
		}

		private static void SortAllMain(T current, List<T> all, List<T> list)
		{
			if(!Checker.IsNull(current)) { list.Add(current); }

			var children = Checker.IsNull(current)
				? from child in all
				  where Checker.IsNull(child.ParentID)
				  select child
				: from child in all
				  where child.ParentID == current.KeyValue
				  select child;
			foreach(T one in children) {
				SortAllMain(one, all, list);
			}
		}

		private bool Predicate(T x)
		{
			return x.KeyValue == KeyValue;
		}
		#endregion

		#region instance level
		#endregion

		#region Event Handler
		#endregion
	}
}

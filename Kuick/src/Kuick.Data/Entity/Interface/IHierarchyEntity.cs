// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IHierarchyEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System.Collections.Generic;

namespace Kuick.Data
{
	public interface IHierarchyEntity : IEntity
	{
		string ParentID { get; set; }
		string Title { get; set; }
		int Left { get; set; }
		int Right { get; set; }

		bool IsRoot { get; }
		int Level { get; }
		bool HasChild { get; }
		bool HasNext { get; }
		List<string> Path { get; }

		void Regulating();
	}

	public interface IHierarchyEntity<T>
		: IHierarchyEntity
		 where T : IHierarchyEntity<T>
	{
		T Parent { get; }
		T Previous { get; }
		T Next { get; }
		List<T> Nodes { get; }
		List<T> Roots { get; }
		List<T> Parents { get; }
		List<T> Siblings { get; }
		List<T> Children { get; }
		List<T> Descendant { get; }
	}
}

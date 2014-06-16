// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Randomizer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-07 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class Randomizer<T>
	{
		private List<T> _List;
		private int _Index = 0;

		public Randomizer(List<T> list) {
			if (list == null || list.Count == 0) {
				throw new ArgumentException("null or zero count", "list");
			}
			_List = new List<T>(list);
			Init();
		}

		private void Init()
		{
			_List.Randomize();
			_Index = _List.Count - 1;
		}

		public T Next()
		{
			var s = _List[_Index--];
			if (_Index < 0) { Init(); }
			return s;
		}
	}
}

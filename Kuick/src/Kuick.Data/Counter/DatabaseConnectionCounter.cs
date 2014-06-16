// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DatabaseConnectionCounter.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class DatabaseConnectionCounter
	{
		private static object _Lock = new object();
		private static int _Present = 0;
		private static int _Peak = 0;
		private static Dictionary<string, Pointer> _Dictionary = 
			new Dictionary<string, Pointer>();

		#region operation
		public static Pointer Open(ConfigDatabase config)
		{
			lock(_Lock) {
				// all
				_Present++;
				if(_Present > _Peak) {
					Logger.Message(string.Format(
						"All Entities: max database concurrent connection count so far = {1}",
						config.Key,
						_Present
					));
					_Peak = _Present;
				}

				// single
				Pointer pointer = _Dictionary.ContainsKey(config.Key) 
					? _Dictionary[config.Key] 
					: new Pointer();
				pointer.Present++;
				if(pointer.Present > pointer.Peak) {
					pointer.Peak = pointer.Present;
					Logger.Track(string.Format(
						"{0}: max database concurrent connection count so far = {1}",
						config.Key,
						pointer.Peak
					));
				}
				if(!_Dictionary.ContainsKey(config.Key)) {
					_Dictionary.Add(config.Key, pointer);
				}
				return pointer;
			}
		}

		public static Pointer Close(ConfigDatabase config)
		{
			lock(_Lock) {
				// all
				_Present--;

				// single
				Pointer pointer;
				if(_Dictionary.TryGetValue(config.Key, out pointer)) {
					if(pointer.Present > 0) { pointer.Present--; }
				} else {
					Logger.Error(
						"DatabaseConnectionCounter.Close",
						"Always open before colse, the executing order was wrong!",
						config.ToAny().ToArray()
					);
				}
				return pointer;
			}
		}
		#endregion
	}
}

// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Diagnostics;

namespace Kuicker
{
	// Acronyms
	public class ExHelper : ExceptionHelper
	{ }

	public class ExceptionHelper
	{
		public static NotImplementedException NotImplementedEnum<T>(T value)
		{
			var callee = CalleeFullName();
			//Logger.Error(
			//	callee,
			//	new Any("Type", typeof(T).FullName),
			//	new Any("Value", value)
			//);
			return new NotImplementedException(
				string.Concat(
					callee, " >> ", typeof(T).FullName, "::", value
				)
			);
		}

		#region private
		private static string CalleeFullName()
		{
			var stackTrace = new StackTrace();
			var method = stackTrace.GetFrame(1).GetMethod();
			var methodName = method.Name;
			var className = method.ReflectedType.Name;
			return string.Concat(methodName, ".", className);
		}
		#endregion
	}
}

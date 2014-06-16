// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullBuiltin.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation
// Kevin Jong       2012-03-18 - Rename from NullService to NullBuiltin


namespace Kuick
{
	public abstract class NullBuiltin : BuiltinBase
	{
		#region IIsNull
		public override bool IsNull
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}

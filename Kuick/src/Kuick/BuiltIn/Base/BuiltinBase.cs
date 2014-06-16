// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BuiltinBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation
// Kevin Jong       2012-03-18 - Rename from ServiceBase to BuiltinBase


namespace Kuick
{
	public abstract class BuiltinBase : NameBase, IBuiltin
	{
		#region IIsNull
		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region IBuiltin
		public virtual bool Default
		{
			get
			{
				return false;
			}
		}

		public virtual void Initiate()
		{
		}

		public virtual void Terminate()
		{
		}
		#endregion
	}
}

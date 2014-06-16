// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntityEventArgs.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation



namespace Kuick.Data
{
	public class EntityEventArgs
	{
		public EntityEventArgs()
			: this(Kuick.Result.BuildSuccess())
		{
		}

		public EntityEventArgs(Result result)
		{
			this.Result = result;
		}

		public Result Result { get; private set; }
		public bool Success { get { return Result.Success; } }

		public void Add(Result result)
		{
			Result.InnerResults.Add(result);
		}
	}
}

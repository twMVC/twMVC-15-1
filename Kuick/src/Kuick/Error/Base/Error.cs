// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Error.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-18 - Creation



namespace Kuick
{
	public abstract class Error : IError
	{
		#region constants
		private const string CAUSE = "Cause";
		private const string ACTION = "Action";
		#endregion

		#region IError
		public string ErrorID { get { return string.Format("{0}-{1}", Scope, Code); } }
		public abstract string Code { get; }
		public string Cause { get { return Read(CAUSE).AirBag(DefaultCause); } }
		public string Action { get { return Read(ACTION).AirBag(DefaultAction); } }
		#endregion

		#region abstract
		protected abstract Scope Scope { get; }
		protected abstract string DefaultCause { get; }
		protected abstract string DefaultAction { get; }
		#endregion

		#region private
		private string Namespace { get { return this.GetType().Namespace; } }

		private string Path 
		{ 
			get 
			{ 
				return string.Format(
					"Error/{0}/{1}",
					Scope.ToString(),
					Namespace
				); 
			} 
		}

		private string Read(string name)
		{
			string value = Builtins.Multilingual.Read(
				Current.AppID, Current.Language, Path, name
			);
			return value;
		}
		#endregion
	}
}

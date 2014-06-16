// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Impersonator.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class Impersonator : IDisposable
	{
		private bool _Disposed = false;
		private WindowsAuthentication _Auth = null;
		private string _ErrorMessage = string.Empty;

		public Impersonator(string domain, string userName, string password)
		{
			_Auth = new WindowsAuthentication();
			if(!_Auth.Impersonate(domain, userName, password, ref _ErrorMessage)) {
				throw new ApplicationException(_ErrorMessage);
			}
		}

		public Impersonator(string domain, string userName, string password, int way)
		{
			_Auth = new WindowsAuthentication();
			if(!_Auth.Impersonate(domain, userName, password, ref _ErrorMessage, way)) {
				throw new ApplicationException(_ErrorMessage);
			}
		}

		~Impersonator()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if(!_Disposed) {
				if(disposing) {
				}

				if(_Auth != null) {
					_Auth.Undo();
					_Auth = null;
				}

				_Disposed = true;
			}
		}

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}

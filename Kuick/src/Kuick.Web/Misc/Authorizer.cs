// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Authorizer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-07 - Creation


using System;

namespace Kuick.Web
{
	public sealed class Authorizer
	{
		private static Func<bool> _AppAuthorize;
		public static Func<bool> AppAuthorize
		{
			get
			{
				if(null == _AppAuthorize) {
					return () => true;
				}
				return _AppAuthorize;
			}
			set
			{
				_AppAuthorize = value;
			}
		}

		private static Func<PageBase, bool> _FeatureAuthorize;
		public static Func<PageBase, bool> FeatureAuthorize
		{
			get
			{
				if(null == _FeatureAuthorize) {
					return page => true;
				}
				return _FeatureAuthorize;
			}
			set
			{
				_FeatureAuthorize = value;
			}
		}

		private static Func<PageBase, string, string, bool> _FragmentAuthorize;
		public static Func<PageBase, string, string, bool> FragmentAuthorize
		{
			get
			{
				if(null == _FragmentAuthorize) {
					return (page, title, description) => true;
				}
				return _FragmentAuthorize;
			}
			set
			{
				_FragmentAuthorize = value;
			}
		}
	}
}

// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebConfig.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick.Web
{
	/// <summary>
	/// All about web application configuration.
	/// </summary>
	/// <remarks>
	/// <![CDATA[
	/// <Kuick>
	///     <project>
	///         <add group="Web" name="SecurePolicy" value="http|https|non"/>
	///         <add group="Web" name="SecureDomain" value="ssl.kuick.com"/>
	///         <add group="Web" name="SecureArea" value="~/Permission, Logon.aspx"/>
	///         <add group="Web" name="NonSecureArea" value="~/Data/, Default.aspx"/>
	///     </project>
	/// </Kuick>
	/// ]]>
	/// </remarks>
	public class WebConfig
	{
		#region constants
		private const string GROUP = "Web";
		private const string SECURE_POLICY = "SecurePolicy";
		private const string SECURE_DOMAIN = "SecureDomain";
		private const string SECURE_AREA = "SecureArea";
		private const string NON_SECURE_AREA = "NonSecureArea";
		#endregion

		#region property
		public static SecurePolicy SecurePolicy { get; private set; }
		public static string SecureDomain { get; private set; }
		public static string[] SecureArea { get; private set; }
		public static string[] NonSecureArea { get; private set; }
		#endregion

		#region method
		internal static void Initialize()
		{
			WebConfig.SecurePolicy = Formator.AirBagToEnum<SecurePolicy>(
				WebCurrent.Application.GetString(GROUP, SECURE_POLICY, string.Empty),
				SecurePolicy.Any
			);

			WebConfig.SecureDomain = Formator.AirBagToString(
				WebCurrent.Application.GetString(GROUP, SECURE_DOMAIN, string.Empty)
			);

			WebConfig.SecureArea = Formator.AirBagToString(
				WebCurrent.Application.GetString(GROUP, SECURE_AREA, string.Empty)
			).SplitWith(WebConstants.Symbol.Comma);

			WebConfig.NonSecureArea = Formator.AirBagToString(
				WebCurrent.Application.GetString(GROUP, NON_SECURE_AREA, string.Empty)
			).SplitWith(WebConstants.Symbol.Comma);
		}
		#endregion
	}
}

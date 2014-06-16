// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AcceptFilterAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;
using System.Web;

namespace Kuick.Web
{
	/// <summary>
	/// Filter the request, especially for secure reason.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class AcceptFilterAttribute : Attribute
	{
		private List<string> AcceptVerbs = new List<string>(5);

		public AcceptFilterAttribute()
			: this(HttpVerbs.All)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="verbs">which Http verbs are accepted.</param>
		public AcceptFilterAttribute(HttpVerbs verbs)
		{
			Verbs = verbs;
			Add(HttpVerbs.Delete, "DELETE");
			Add(HttpVerbs.Get, "GET");
			Add(HttpVerbs.Head, "HEAD");
			Add(HttpVerbs.Post, "POST");
			Add(HttpVerbs.Put, "PUT");
		}

		private void Add(HttpVerbs verb, string VERB)
		{
			if((Verbs & verb) == verb) { AcceptVerbs.Add(VERB); }
		}

		/// <summary>
		/// Valid the request is accepted or not.
		/// </summary>
		/// <returns>valid or invalid, if invalid, reason set to message property.</returns>
		public Result Valid(HttpRequest Request)
		{
			Result r = new Result(false);

			if(RequireSecure && !Request.IsSecureConnection) {
				r.Message = "Require Secure connection.";
				return r;
			}

			if(RequireSameDomain) {
				if(WebChecker.IsNull(Request.UrlReferrer)) {
					r.Message = "Request must from same domain.";
					return r;
				}

				// trim dot character, for fiddler to monitor "localhost."
				else if(!Request.Url.Host.Equals(
					Request.UrlReferrer.Host.Trim('.'),
					StringComparison.OrdinalIgnoreCase)
					) {
					r.Message = "Request must from same domain.";
					return r;
				}
			}

			if(Verbs != HttpVerbs.All) {

				if(!AcceptVerbs.Contains(Request.HttpMethod)) {
					r.Message = String.Format(
						"Http verb {0} is denied.",
						Request.HttpMethod
					);
					return r;
				}
			}

			r.Success = true;
			return r;
		}

		/// <summary>
		/// Which verbs are accepted.
		/// </summary>
		public HttpVerbs Verbs { get; private set; }

		/// <summary>
		/// The request must from same domain. 
		/// i.e. Referrer and request url must have the same host name.
		/// </summary>
		public bool RequireSameDomain { get; set; }

		/// <summary>
		/// The request must use https.
		/// </summary>
		public bool RequireSecure { get; set; }

	}
}

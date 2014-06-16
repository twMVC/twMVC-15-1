// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Emailer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Security.Principal;

namespace Kuick
{
	public sealed class Emailer
	{
		#region const
		private static readonly string REGEX_HREF_ABSOLUTE = string.Format(
			@"(\s(href|src)\s*=\s*[""'])((?!#|\/|{0}:\/\/|{1}:\/\/|{2}:)[^""']+)([""'])",
			Uri.UriSchemeHttp,
			Uri.UriSchemeHttps,
			Uri.UriSchemeMailto
		);
		private const string REGEX_HREF_ABSOLUTE_SLASH =
			@"(\s(href|src)\s*=\s*[""'])(\/[^""']+)([""'])";
		#endregion

		#region Constructor
		public Emailer()
			: this(Current.Mail.From, string.Empty, string.Empty, string.Empty) { }

		public Emailer(string sender, string receiver, string subject, string message)
			: this(sender, receiver, subject, message, Encoding.Default) { }

		public Emailer(
			string sender,
			string receiver,
			string subject,
			string message,
			Encoding encode)
		{
			this.SendSuccess = true;

			Sender = sender;
			Encode = encode;

			ReceiverList = new List<string>();
			BCCList = new List<string>();
			CCList = new List<string>();
			Params = new Dictionary<string, string>();

			if(!string.IsNullOrEmpty(receiver)) {
				string[] ls = receiver.Split(';');
				foreach(string s in ls) {
					if(!Checker.IsNull(s)) { ReceiverList.Add(s.Trim()); }
				}
			}

			Subject = subject;
			Body = message;
		}
		#endregion

		#region property
		public List<string> ReceiverList { get; set; }
		public List<string> CCList { get; set; }
		public List<string> BCCList { get; set; }
		public string Sender { get; set; }
		public Dictionary<string, string> Params { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string BodyFile { get; set; }
		public bool SendSuccess { get; set; }
		public string ErrorMessage { get; set; }
		public Encoding Encode { get; set; }
		public Action<object, AsyncCompletedEventArgs> CustomSendCompleted {get; set;}
		#endregion

		#region static
		public static Emailer SendMail(
			string subject, string theMsg, List<string> tos)
		{
			Emailer emailer = new Emailer();
			emailer.ReceiverList.AddRange(tos);
			emailer.Subject = subject;
			emailer.Body = theMsg;
			return emailer.Send();
		}
		public static Emailer SendMail(
			string fromWho, string toWho, string subject, string theMsg)
		{
			return SendMail(fromWho, toWho, subject, theMsg, Encoding.Default);
		}
		public static Emailer SendMail(
			string fromWho,
			string toWho,
			string subject,
			string theMsg,
			Encoding encode)
		{
			return new Emailer(fromWho, toWho, subject, theMsg, encode).Send();
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Send mail, use config default smtp server, add base href to header
		/// </summary>
		/// <returns>this</returns>
		public Emailer Send()
		{
			return Send(true);
		}

		private string ToAbsoluteUrl(string url)
		{
			string ap = Current.IsWebApplication
				? HttpContext.Current.Request.ApplicationPath
				: string.Empty;
			StringBuilder sb = new StringBuilder(url);
			if(!ap.EndsWith(Constants.Symbol.Slash)) {
				ap += Constants.Symbol.Slash;
			}
			return sb.Replace("~/", ap).Replace("\\", "/").ToString();
		}

		/// <summary>
		/// Send mail, use config default smtp server
		/// </summary>
		/// <param name="addBaseHref">add request host as base href to header or not</param>
		/// <returns>this</returns>
		public Emailer Send(bool addBaseHref)
		{
			UriBuilder ub = Current.IsWebApplication ?
				new UriBuilder(
					Uri.UriSchemeHttp,
					HttpContext.Current.Request.Url.Host,
					80,
					ToAbsoluteUrl("~/")
				)
				: new UriBuilder();

			return Send(null, addBaseHref ? ub.Uri : null);
		}

		public static string RelativeToAbsolute(string html, Uri baseHref)
		{
			// ensure path end with slash
			var path = baseHref.AbsolutePath;
			if(!path.EndsWith(Constants.Symbol.Slash)) {
				path += Constants.Symbol.Slash;
			}

			// change relative path to absolute (begin with slash)
			html = Regex.Replace(
				html,
				REGEX_HREF_ABSOLUTE,
				String.Format("$1{0}$3$4", path),
				RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled
			);

			// change path to absolute (begin with protocal(http,https))
			UriBuilder ub = new UriBuilder(baseHref.Scheme, baseHref.Host, baseHref.Port);
			html = Regex.Replace(
				html,
				REGEX_HREF_ABSOLUTE_SLASH,
				String.Format("$1{0}$3$4", ub.Uri.ToString().TrimEnd(
					Constants.Symbol.Char.Slash
				)),
				RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled
			);
			return html;
		}
		/// <summary>
		/// Send mail
		/// </summary>
		/// <param name="smtpServer">SMTP Server</param>
		/// <param name="baseHref">add base href to header, 
		/// null means not to add. Uri scheme is required.</param>
		/// <returns>this</returns>
		public Emailer Send(string smtpServer, Uri baseHref)
		{
			// MailMessage
			using(MailMessage email = new MailMessage()) {
				email.BodyEncoding = Encode;
				email.SubjectEncoding = Encode;

				email.From = new MailAddress(string.IsNullOrEmpty(Sender) 
					? Current.Mail.From : Sender
				);

				if(ReceiverList != null) {
					foreach(string receiver in ReceiverList) {
						email.To.Add(new MailAddress(receiver));
					}
				}

				if(CCList != null) {
					foreach(string receiver in CCList) {
						email.CC.Add(new MailAddress(receiver));
					}
				}

				if(BCCList != null) {
					foreach(string receiver in BCCList) {
						email.Bcc.Add(new MailAddress(receiver));
					}
				}

				email.Subject = Subject;

				StringBuilder sb = new StringBuilder(
					(!String.IsNullOrEmpty(BodyFile) && File.Exists(BodyFile)) ?
						File.ReadAllText(BodyFile) :
						Body
				);

				if(Params != null) {
					foreach(KeyValuePair<string, string> p in Params) {
						sb.Replace(p.Key, p.Value);
					}
				}

				string body = sb.ToString();

				if(null != baseHref) {
					// body = RelativeToAbsolute(body, baseHref);
					body = body.Replace(
						"<head>",
						String.Format(
							"<head><base href=\"{0}\"></base>",
							baseHref
						)
					);
				}

				// Email Body: HTML
				// 1
				email.Body = body;
				email.IsBodyHtml = true;
				//// 2
				//email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
				//    body, null, "text/html"
				//));

				smtpServer = string.IsNullOrEmpty(smtpServer) 
					? Current.Smtp.Server : smtpServer;

				using(SmtpClient smtpClient = Checker.IsNull(smtpServer)
					? new SmtpClient()
					: new SmtpClient(smtpServer)
				) {
					// Credentials
					if(
						string.IsNullOrEmpty(Current.Smtp.UserName)
						&&
						string.IsNullOrEmpty(Current.Smtp.Password)) {
						smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
					} else {
						smtpClient.Credentials = new NetworkCredential(
							Current.Smtp.UserName, Current.Smtp.Password
						);
					}

					// EventHandler
					smtpClient.SendCompleted += new SendCompletedEventHandler(
						this.SendCompleted
					);

					// Send
					try {
						smtpClient.Send(email);
						this.SendSuccess = true;
					} catch(Exception e) {
						Logger.Error(
							"Kuick.Emailer.Send",
							e,
							new Any("ConnectionName", smtpClient.ServicePoint.ConnectionName)
						);
						this.SendSuccess = false;
						this.ErrorMessage = e.Message;
					}
				}
			}
			return this;
		}


		private void SendCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if(null != CustomSendCompleted) {
				CustomSendCompleted(sender, e);
			}

			MailMessage mailMessage = (MailMessage)e.UserState;
			string subject = mailMessage.Subject;

			if(e.Error == null) {
				// Not Error
				if(e.Cancelled) {
					// Cancelled
					Logger.Error(
						"Emailer.SendCompleted",
						"Send canceled",
						new Any("Subject", subject)
					);
				} else {
					// Correct
					Logger.Message(
						"Emailer.SendCompleted",
						"email send",
						new Any("Subject", subject)
					);
				}
			} else {
				// Error
				Logger.Error(
					"Emailer.SendCompleted",
					"Send error",
					e.Error,
					new Any("Subject", subject)
				);
			}
		}
		#endregion
	}
}

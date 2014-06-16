// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ApiHandlerBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MSA = Microsoft.Security.Application;
using System.Web.SessionState;

namespace Kuick.Web
{
	public abstract class ApiHandlerBase 
		: HandlerBase, IRequiresSessionState //IReadOnlySessionState
	{
		[RequestParameter]
		public ApiMode KuickApiMode { get; set; }

		/// <summary>
		/// The api result in json format
		/// </summary>
		/// <param name="r"></param>
		public void Result(Result r)
		{
			Response.ContentType = ContentType.JSON;
			Response.Write(r.SerializeJson("Message", "Success"));
		}
		/// <summary>
		/// The api result in json format
		/// </summary>
		public void Result(bool success)
		{
			Result(new Result(success));
		}
		/// <summary>
		/// The api result in json format
		/// </summary>
		public void Result(bool success, string message)
		{
			Result((new Result(success) { Message = message }));
		}

		public ApiHandlerBase()
		{
			this.OnValidRequestEnd += new EventHandler<HandlerEventArgs>(
				ApiHandlerBase_OnValidRequestEnd
			);
		}

		private void ApiHandlerBase_OnValidRequestEnd(object sender, HandlerEventArgs e)
		{
			if(WebCurrent.Released
				||
				!KuickApiMode.EnumIn(ApiMode.Develop, ApiMode.Document)
			) {
				return;
			}

			bool isDoc = KuickApiMode == ApiMode.Document;
			StringBuilder jsConst = GetJson(isDoc);
			bool notHasParameter = jsConst.Length == 0;
			if(notHasParameter) { return; }

			var accept = AttributeHelper.GetAll<AcceptFilterAttribute>(this.GetType());
			var ls = new List<string>();
			foreach(var item in accept) {
				ls.Add(item.Verbs.ToString());
			}

			jsConst
				.Insert(0, "var CONST = {Data:")
				.Append(",Action:")
				.Append(MSA.Encoder.JavaScriptEncode(
					Regex.Replace(
						Request.RawUrl,
						"Mode=Develop",
						"",
						RegexOptions.IgnoreCase | RegexOptions.Compiled
					)
				))
				.Append(",Verbs:")
				.Append(WebTools.JsString(String.Join(" / ", ls.ToArray())))
				.Append("};");

			if(isDoc) {
				Response.Write(GetDocumentHtml(jsConst.ToString()));
			} else {
				jsConst.Insert(0, GetCommonFunction());
				Response.Write(GetDevelopHtml(jsConst.ToString()));
			}

			Response.End();
		}

		/// <summary>
		/// descript what this api to do.
		/// </summary>
		public abstract string Title { get; }

		#region private
		private bool hasUpload = false;

		private string GetCommonFunction()
		{
			List<Type> d = new List<Type>();
			StringBuilder sb = new StringBuilder();

			List<RequestParameterAttribute> parameters = new List<RequestParameterAttribute>(
				AttributeHelper.GetRequestParameterAttributes(this.GetType())
			);
			parameters.RemoveAll(x => x.Title == "KuickApiMode");

			foreach(var param in parameters) {
				foreach(var validator in param.Validations) {
					if(!d.Contains(validator.GetType())) {
						sb.Append(validator.JsFunction);
					}
				}
			}
			return sb.ToString();
		}

		private StringBuilder GetJson(bool forDoc)
		{

			List<RequestParameterAttribute> parameters = new List<RequestParameterAttribute>(
				AttributeHelper.GetRequestParameterAttributes(this.GetType())
			);
			parameters.RemoveAll(x => x.Title == "KuickApiMode");

			StringBuilder json = new StringBuilder();

			foreach(var param in parameters) {
				Type t = param.PropertyInfo.PropertyType;
				bool isUpload = t.Equals((typeof(Stream))) || t.Equals((typeof(Byte[])));
				if(isUpload) { hasUpload = true; }
				json.Append("{")
				.AppendFormat(
					"name:{0},title:{1},type:{2},val:{3},",
					MSA.Encoder.JavaScriptEncode(param.Title),
					MSA.Encoder.JavaScriptEncode(param.Description),
					isUpload ? "'file'" : "'text'",
					MSA.Encoder.JavaScriptEncode(Request[param.Title] ?? "")
				);

				if(param.Validations.Length == 0) {
					json.Append("docs:[''], valid:[function(){return'';}]},");
				} else {
					json.Append("docs:[");
					foreach(var validator in param.Validations) {
						json.Append(MSA.Encoder.JavaScriptEncode(validator.ToString()))
							.Append(",");
					}

					if(!forDoc) {
						json.RemoveLast(1).Append("], valid:[");
						foreach(var validator in param.Validations) {
							json.Append(validator.JsValidate()).Append(",");
						}
					}
					json.RemoveLast(1).Append("]},");
				}
			} // parameters

			json.RemoveLast(1).Insert(0, '[').Append("]");

			return json;
		}

		private string GetDevelopHtml(string JsConst)
		{
			return new StringBuilder(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
	<title>{Title}</title>
	<style type=""text/css"">
		html,body,ul,li,div,span,label,h1,h2,h3,h4,h5{padding:0;margin:0;color:#333;font-family: Consolas;}
		ul.forms{list-style:none}
		li.items{padding:10px;border-bottom:1px dotted gray}
		.left{float:left;width:49%}
		.right{float:right;width:49%;border:1px dashed gray}
		.label{float:left;padding-right:10px;width:100px}
		.warning{color:#d00;margin-left:100px;padding:5px 0}
		.verb-warning{color:#d00;padding:5px 0}
		.invalid{background-color:#fcc}
		.form-foot{text-align:center}
		.response .time{color:#888;background-color:#ccc;padding:5px 10px}
		.response .text{padding:5px 10px}
		.expand{font-size:0.5em;margin-left:3px}
		a.file{display:none}
	</style>
</head>
<body>
	<div class=""left"">
		<h1 class=""title"">{Title}</h1>
		<div class=""verbs"">
			<label><input name=""method"" type=""radio"" value=""get"">GET</label>
			<label><input name=""method"" type=""radio"" value=""post"" checked=""checked"">POST</label>
		</div>
		<form action="""" method=""post""{enctype}>
		<ul class=""forms"">
		</ul>
		<div class=""form-foot"">
		<input type=""submit"" />
		<label><input id=""use-ajax"" type=""checkbox"" checked=""checked"">AJAX</label>
		</div>
		</form>
	</div>
	<div class=""right"">
		<h1 class=""title"">Output</h1>
		<div class=""output-area"">
		</div>
	</div>
	<script type=""text/javascript"" src=""//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js""></script>
	<script type=""text/javascript"">//<![CDATA[
")
				.Replace(
					"{Title}",
					WebTools.HtmlEncode(Title)
				)
				.Replace(
					"{enctype}", 
					hasUpload 
						? " enctype=\"" + WebConstants.Html.Enctype.MultiPart + "\"" 
						: String.Empty
				)
				.Append(
					new JavaScriptMinifier().Minify(@"
$(function(){
	$.bind = function(template, data){
		var m;
		while ( m = /\{\s*([^\}\s]+)\s*\}/.exec(template) ) {
			template = template.replace( m[0], data[m[1]] || '' );
		}
		return template;
	};
	$.log = function(d){
		$('<div class=""response""><div class=""time"">'+new Date+'</div><div class=""text""><pre></pre></div></div>').find('pre').text(JSON.stringify(d)).end().hide()
		.prependTo('.output-area').slideDown();
	};

	$('.expand').on('click', function(){
		var t = $(this).hide().prev();
		if (!t.attr('readonly')){
			t.replaceWith('<textarea name=""'+t.attr('name')+'"" rows=""5"" cols=""30"">'+t.val()+'</textarea>');
		}
		return !1;
	});

	$.each(CONST.Data, function(i,o){
		$($.bind('<li class=""items""><span class=""label"">{title}</span><input name=""{name}"" type=""{type}""/><a class=""expand {type}"" href=""#"">&gt;&gt;</a></li>',o))
		.data('valid', o).find('input').val(o.val).attr('readonly', o.val!='').end().appendTo('ul.forms');
	});

	if (CONST.Verbs) {
		$('<span class=""verb-warning""/>').text('Only accept:' + CONST.Verbs).appendTo('.verbs');
	}

	$('input[name=method]').click(function(){
		frm.attr('method',$(this).val());
	});

	var frm = $('form').ajaxError(function(evt,req,opts,err) {
		var d = req.status + ' ' + req.statusText;
		$.log(d);
	})
	.attr('action',CONST.Action)
	.submit(function(){

		var jq = $(this).find('.warning').remove().end()
		.find('.forms input,.forms textarea').removeClass('invalid').each(function(){
			var t = $(this);
			var v = t.val();
			var valid = t.parents('li').data('valid').valid;
			for(var i=0;i<valid.length;i++) {
				r = valid[i](v);
				if (r) { $('<div class=""warning""></div>').text(r).insertAfter(t.addClass('invalid').next('.expand')); }
			}
		}).end();
		if (jq.find('.warning').size()!=0) return !1;

		if ($('#use-ajax').attr('checked')) {
			var act = jq.attr('action');
			$.post(act, jq.serialize(), $.log);
			return !1;
		} else {
			var now = new Date;
			jq.attr('target','ifrm'+now.getTime());
			$('<div class=""response""><div class=""time"">'+now+'</div><iframe name=""'+jq.attr('target')+'"" src=""about:blank"" width=""100%"" height=""250""/></div>')
			.hide().prependTo('.output-area').slideDown();
		}
	});

	if (frm.find('input:file').size()>0) {
		frm.attr('enctype','multipart/form-data');
		$('#use-ajax').attr('checked',!1).parent().hide();
		$('.verbs label').hide();
	}
});
"))
				.Append(JsConst)
				.Append(@"
					//]]></script>
					</body></html>
				")
				 .ToString();
		}

		private string GetDocumentHtml(string JsConst)
		{
			return new StringBuilder(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
	<title>{Title}</title>
	<style type=""text/css"">
	html,body,ul,li,div,span,label,h1,h2,h3,h4,h5{padding:0;margin:0;color:#333;font-family: Consolas;}
	/*Blue on blue written by Glenn Slaven http://slaven.net.au */
	table,td,th{border-collapse:collapse;margin:0;padding:0}
	table{border:none;font-family:Consolas}
	table caption{background-color:transparent;background-image:url(cap_bg.gif);background-repeat:repeat-x;background-position:bottom left;text-align:left;font-size:150%;text-transform:uppercase;line-height:30px;letter-spacing:0}
	td,th{vertical-align:top;font-weight:normal}
	thead{border-left:1px solid #2293FF}
	thead th{background:#2293FF;color:#FFF;font-size:125%;border-top:1px solid #2293FF;border-right:1px solid #2293FF;padding:4px 0.4em 4px 0.4em}
	tfoot td,tfoot th{border-top:1px solid #2293FF;border-right:none;font-weight:bolder;font-size:110%;padding:0.4em 0.5em 0.4em 0.5em}
	tbody td,tbody th{background-color:#D9ECFF;border-right:1px solid #2293FF;font-size:110%;padding:0.4em 0.5em 0.4em 0.5em}
	tbody tr.odd th,tbody tr.odd td{background-color:#BDDFFF}
	tbody th{font-weight:bold;border-left:1px solid #2293FF}
	table a:link,table a:visited,table a:active{color:#444F66;background-color:transparent;text-decoration:underline}
	table a:hover{text-decoration:none;background-color:#1E90FF;color:#FFF}
	</style>
</head>
<body>
	<h1>{Title}</h1>
		<table class=""doc"">
			<thead><tr><th>Request Name</th><th>Description</th><th>Rules</th></tr></thead>
			<tbody></tbody>
			<tfoot><tr><th colspan=""3""></th></tr></tfoot>
		</table>
	<script type=""text/javascript"" src=""//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js""></script>
	<script type=""text/javascript"">//<![CDATA[
")
				.Replace("{Title}", WebTools.HtmlEncode(Title))
				.Append(new JavaScriptMinifier().Minify(@"
	$(function(){
		$.bind = function(template, data){
			var m;
			while ( m = /\{\s*([^\}\s]+)\s*\}/.exec(template) ) {
				template = template.replace( m[0], data[m[1]] || '' );
			}
			return template;
		};
		$.each(CONST.Data, function(i,o){
			o.docs = o.docs.join('<br>');
			$($.bind('<tr><td>{name}</td><td>{title}</td><td>{docs}</td></tr>',o)).appendTo('.doc>tbody');
			$('.doc tr:odd').addClass('odd')
		});
		if (CONST.Verbs) {
			$('.doc tfoot th').text('ONLY ACCEPT: ' + CONST.Verbs);
		}
	});
"))
				.Append(JsConst)
				.Append(@"
					//]]></script>
				</body></html>
				")
				.ToString();
		}
		#endregion
	}
}

<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
	CodeFile="Edit.aspx.cs" Inherits="Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="__Head__" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="__Main__" runat="Server">
	<%=Editor.RenderForm()%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="__Footer__" runat="Server">
	<%=Editor.RenderJavaScript()%>
</asp:Content>

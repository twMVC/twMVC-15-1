<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="__Head__" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="__Main__" Runat="Server">
	<%=Render()%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="__Footer__" Runat="Server">
	<%=RenderJavaScript()%>
</asp:Content>


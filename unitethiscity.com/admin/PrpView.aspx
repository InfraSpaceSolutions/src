<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PrpView.aspx.cs" Inherits="admin_PrpView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="PrpMenu" runat="server" Name="Properties" Prefix="Prp" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="PrpIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="PrpNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField" valign="top">Description</td>
			<td class="formValue"><asp:Literal ID="PrpDescriptionLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this property?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
</asp:Content>


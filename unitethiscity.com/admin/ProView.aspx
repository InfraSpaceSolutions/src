<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="ProView.aspx.cs" Inherits="admin_ProView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="ProMenu" runat="server" Name="Promotions" Prefix="Pro" Type="View" />
	<table class="formTable" style="width:100%" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="ProIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="ProNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Title</td>
			<td class="formValue"><asp:Literal ID="ProTitleLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Text</td>
			<td class="formValue"><asp:Literal ID="ProTextLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Enabled?</td>
			<td class="formValue"><asp:Literal ID="ProEnabledLiteral" runat="server"/></td>
		</tr>
	</table>
    <br clear="right" />
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this promotion?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="MsjView.aspx.cs" Inherits="admin_MsjView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="MsjMenu" runat="server" Name="Message Jobs" Prefix="Msj" Type="View" />
	<table class="formTable" style="width:99%" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="MsjIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Account</td>
			<td class="formValue"><asp:Literal ID="AccNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Message Job State</td>
			<td class="formValue"><asp:Literal ID="MjsNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Send Time</td>
			<td class="formValue"><asp:Literal ID="MsjSendTSLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">To</td>
			<td class="formValue"><asp:Literal ID="MsjToLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">From Name</td>
			<td class="formValue"><asp:Literal ID="MsgFromNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Timestamp</td>
			<td class="formValue"><asp:Literal ID="MsgTSLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Expires</td>
			<td class="formValue"><asp:Literal ID="MsgExpiresLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Summary</td>
			<td class="formValue"><asp:Literal ID="MsgSummaryLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField" colspan="2">Body</td>
		</tr>
		<tr>
			<td class="formValue" colspan="2"><asp:Literal ID="MsgBodyLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this message job?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
</asp:Content>


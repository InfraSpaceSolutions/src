<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PerDelView.aspx.cs" Inherits="admin_PerDelView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="PerMenu" runat="server" Name="Periods" Prefix="Per" Type="Deal View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="PerIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:HyperLink ID="PerNameHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Start Date</td>
			<td class="formValue"><asp:Literal ID="PerStartDateLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">End Date</td>
			<td class="formValue"><asp:Literal ID="PerEndDateLiteral" runat="server" /></td>
		</tr>
	</table>
    
    <br />
    <br />
	<h3>Deal</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="DelNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Amount</td>
			<td class="formValue"><asp:Literal ID="DelAmountLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField" valign="top">Description</td>
			<td class="formValue"><asp:Literal ID="DelDescriptionLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField" valign="top">Custom Terms</td>
			<td class="formValue"><asp:Literal ID="DelCustomTermsLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Assigned Business</td>
			<td class="formValue"><asp:HyperLink ID="BusNameHyperLink" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this deal?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
</asp:Content>


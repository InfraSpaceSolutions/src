<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusLocLprEdit.aspx.cs" Inherits="admin_BusLocLprEdit" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="BusIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue"><asp:Literal ID="BusGuidLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:HyperLink ID="BusNameHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Formal Name</td>
			<td class="formValue"><asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Location Name</td>
			<td class="formValue"><asp:HyperLink ID="LocNameHyperLink" runat="server" /></td>
		</tr>
	</table>
    
    <br />
    <br />
	<h3>Manage Properties</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField" valign="top">Available Properties</td>
			<td class="formValue">
				<asp:CheckBoxList ID="PrpIDCheckBoxList" runat="server" DataValueField="PrpID" DataTextField="PrpName" Visible="true" />
				<asp:Literal ID="NoPropertiesLiteral" runat="server" Text="No properties are available to add to this business" />
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


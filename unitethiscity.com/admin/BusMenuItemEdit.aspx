<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusMenuItemEdit.aspx.cs" Inherits="admin_BusMenuItemEdit" %>
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
	</table>
    <br />
    <h3>Edit Item</h3>
    <br />
    <table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name</td>
			<td class="formValue">
                <asp:TextBox ID="MenNameTextBox" runat="server" MaxLength="80" Width="350"/>
				<asp:RequiredFieldValidator ID="MenNameTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="MenNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Price</td>
			<td class="formValue">
                <asp:TextBox ID="MenPriceTextBox" runat="server" MaxLength="10" Width="100"/>
				<asp:RequiredFieldValidator ID="MenPriceTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="MenPriceTextBox" ErrorMessage="Required" />
			</td>
		</tr>
    </table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="button" />
	</div>   
</asp:Content>


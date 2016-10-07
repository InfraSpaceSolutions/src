<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusLocLoyaltyDealEdit.aspx.cs" Inherits="admin_BusLocLoyaltyDealEdit" %>
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
			<td class="formValue"><asp:Literal ID="BusNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Formal Name</td>
			<td class="formValue"><asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
		</tr>
	</table>

    <br />
	<h3>Loyalty Deal Definition</h3>
	<br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Summary *</td>
			<td class="formValue">
				<asp:TextBox ID="LoyNameTextBox" runat="server" MaxLength="255" Width="300" />
				<asp:RequiredFieldValidator ID="LoyNameRequired" runat="server" Display="Dynamic"
					ControlToValidate="LoyNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Points *</td>
			<td class="formValue">
				<asp:TextBox ID="LoyPointsTextBox" runat="server" MaxLength="10" Width="100" />
				<asp:RequiredFieldValidator ID="LoyPointsRequired" runat="server" Display="Dynamic"
					ControlToValidate="LoyPointsTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Custom Terms</td>
			<td align="center" class="formValue">
				<asp:TextBox ID="LoyCustomTermsTextBox" runat="server" MaxLength="1024" TextMode="MultiLine" Rows="3" Width="99%" />	
			</td>
		</tr>
	</table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this loyalty deal?');" />
    </div>
</asp:Content>


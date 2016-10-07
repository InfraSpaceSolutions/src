<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusDldEdit.aspx.cs" Inherits="admin_BusDldEdit" %>
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
    <br />
	<h3>Edit Social Deal Definition</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="DldNameTextBox" runat="server" MaxLength="50" Width="200" />
				<asp:RequiredFieldValidator ID="DldNameRequired" runat="server" Display="Dynamic"
					ControlToValidate="DldNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Amount *</td>
			<td class="formValue">
				<asp:TextBox ID="DldAmountTextBox" runat="server" MaxLength="10" Width="100" />
				<asp:RequiredFieldValidator ID="DldAmountRequired" runat="server" Display="Dynamic"
					ControlToValidate="DldAmountTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Description</td>
			<td align="center" class="formValue">
				<asp:TextBox ID="DldDescriptionTextBox" runat="server" MaxLength="255" TextMode="MultiLine" Rows="3" Width="99%" />	
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Custom Terms</td>
			<td align="center" class="formValue">
				<asp:TextBox ID="DldCustomTermsTextBox" runat="server" MaxLength="255" TextMode="MultiLine" Rows="3" Width="99%" />	
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


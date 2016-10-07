<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusPinEdit.aspx.cs" Inherits="admin_BusPinEdit" %>
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
	<h3>Edit Pin</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Pin Number *</td>
			<td class="formValue">
				<asp:TextBox ID="PinNumberTextBox" runat="server" MaxLength="8" Width="200" />
				<asp:RequiredFieldValidator ID="PinNumberTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="PinNumberTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="PinNameTextBox" runat="server" MaxLength="50" Width="200" />
				<asp:RequiredFieldValidator ID="PinNameRequired" runat="server" Display="Dynamic"
					ControlToValidate="PinNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Enabled *</td>
			<td class="formValue">
                <asp:DropDownList ID="PinEnabledDropDownList" runat="server" >
                    <asp:ListItem Value="False">No</asp:ListItem>
                    <asp:ListItem Value="True">Yes</asp:ListItem>
                </asp:DropDownList>
				<asp:RequiredFieldValidator ID="PinEnabledDropDownListRequired" runat="server" Display="Dynamic"
					ControlToValidate="PinEnabledDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


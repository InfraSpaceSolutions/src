<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusLocTipNew.aspx.cs" Inherits="admin_BusLocTipNew" %>
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
	<h3>Add Account Review</h3>
	<br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Account *</td>
			<td class="formValue">
			    <asp:DropDownList ID="AccIDDropDownList" runat="server" DataTextField="AccEMail" DataValueField="AccID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="AccIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AccIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Tip</td>
			<td align="center" class="formValue">
				<asp:TextBox ID="TipTextTextBox" runat="server" MaxLength="255" TextMode="MultiLine" Rows="3" Width="99%" />	
				    <asp:RequiredFieldValidator ID="TipTextTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="TipTextTextBox" ErrorMessage="Required" />
			</td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
    </div>
</asp:Content>


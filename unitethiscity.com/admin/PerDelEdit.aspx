<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PerDelEdit.aspx.cs" Inherits="admin_PerDelEdit" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="PerMenu" runat="server" Name="Periods" Prefix="Per" Type="Deal Modify" />
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
	<h3>Modify Deal</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="DelNameTextBox" runat="server" MaxLength="50" Width="200" />
				<asp:RequiredFieldValidator ID="DelNameRequired" runat="server" Display="Dynamic"
					ControlToValidate="DelNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Amount *</td>
			<td class="formValue">
				<asp:TextBox ID="DelAmountTextBox" runat="server" MaxLength="10" Width="100" />
				<asp:RequiredFieldValidator ID="DelAmountRequired" runat="server" Display="Dynamic"
					ControlToValidate="DelAmountTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Description</td>
			<td align="center" class="formValue">
				<asp:TextBox ID="DelDescriptionTextBox" runat="server" MaxLength="255" TextMode="MultiLine" Rows="3" Width="99%" />	
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Custom Terms</td>
			<td align="center" class="formValue">
				<asp:TextBox ID="DelCustomTermsTextBox" runat="server" MaxLength="255" TextMode="MultiLine" Rows="3" Width="99%" />	
			</td>
		</tr>
		<tr>
			<td class="formField">Assigned Business</td>
			<td class="formValue"><asp:HyperLink ID="BusNameHyperLink" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PerDelNew.aspx.cs" Inherits="admin_PerDelNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="PerMenu" runat="server" Name="Periods" Prefix="Per" Type="Deal Create" />
	<asp:UpdatePanel ID="PerDelNewUpdatePanel" runat="server">
	<ContentTemplate>
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
	<h3>Create a deal for this period</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Choose a Business with an assigned Deal Defenition *</td>
			<td class="formValue">
				<asp:DropDownList ID="BusIDDropDownList" runat="server" AutoPostBack="true" DataTextField="BusName" DataValueField="BusID">
                        <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="BusIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="BusIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
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
	</table>
	</ContentTemplate>
	</asp:UpdatePanel>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


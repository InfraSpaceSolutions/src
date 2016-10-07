<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ProNew.aspx.cs" Inherits="admin_ProNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="New" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="ProNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="ProNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="ProNameTextBox" ErrorMessage="Required" />
				<asp:CustomValidator ID="ProNameDuplicate" runat="server" Display="Dynamic"
					ControlToValidate="ProNameTextBox" ErrorMessage="The supplied promotion already exists" />
			</td>
		</tr>
		<tr>
			<td class="formField">Title *</td>
			<td class="formValue">
				<asp:TextBox ID="ProTitleTextBox" runat="server" MaxLength="128" Width="400" />
				 <asp:RequiredFieldValidator ID="ProTitleRequired" runat="server" Display="Dynamic"
                    ControlToValidate="ProTitleTextbox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Enabled? *</td>
			<td class="formValue">
                <asp:DropDownList ID="ProEnabledDropDownList" runat="server" >
                    <asp:ListItem Value="False">No</asp:ListItem>
                    <asp:ListItem Value="True" Selected="true">Yes</asp:ListItem>
                </asp:DropDownList>
				<asp:RequiredFieldValidator ID="ProAllowRedeemDropDownListRequired" runat="server" Display="Dynamic"
					ControlToValidate="ProEnabledDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" colspan="2">Text</td>
		</tr>
		<tr>
			<td class="formValue" colspan="2">
				<asp:TextBox ID="ProTextTextBox" runat="server" TextMode="MultiLine" Rows="3" Width="95%" />
			</td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
    </div>
</asp:Content>


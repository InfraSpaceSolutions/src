<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccNew.aspx.cs" Inherits="admin_AccNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="Create" />
	<div>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue">TBD</td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue">TBD</td>
		</tr>
		<tr>
			<td class="formField">First Name *</td>
			<td class="formValue">
				<asp:TextBox ID="AccFNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="AccFNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AccFNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Last Name *</td>
			<td class="formValue">
				<asp:TextBox ID="AccLNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="AccLNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AccLNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
        <tr>
            <td class="formField">Email Address *</td>
            <td class="formValue">
                <asp:TextBox ID="AccEMailTextBox" runat="server" MaxLength="128" Width="300" />
                <asp:RequiredFieldValidator ID="AccEMailRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AccEMailTextBox" ErrorMessage="Required" />
				<asp:RegularExpressionValidator ID="AccEMailValid" runat="server" ValidationExpression="\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+?(\.[a-zA-Z]{2,6})+"
                    ControlToValidate="AccEMailTextBox" ErrorMessage="Invalid address" />
				<asp:CustomValidator ID="AccEMailDuplicate" runat="server" Display="Dynamic"
					ControlToValidate="AccEMailTextBox" ErrorMessage="The supplied email address already exists" />
            </td>
        </tr>
		<tr>
			<td class="formField">Phone</td>
			<td class="formValue">
				<asp:TextBox ID="AccPhoneTextBox" runat="server" MaxLength="50" Width="150" />
			</td>
		</tr>
		<tr>
			<td class="formField">City *</td>
			<td class="formValue">
				<asp:DropDownList ID="CitIDDropDownList" runat="server" DataTextField="CitName" DataValueField="CitID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="CitIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="CitIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Referral Code *</td>
			<td class="formValue">
				<asp:DropDownList ID="RfcIDDropDownList" runat="server" DataTextField="RfcCode" DataValueField="RfcID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                    <asp:ListItem Value="0">None</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="RfcIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="RfcIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
        <tr>
            <td class="formField">Password ( Min. 6 characters )</td>
            <td class="formValue">
                <asp:TextBox ID="AccPasswordTextBox" runat="server" MaxLength="50" Width="150"/>
                <span class="note">Note: If a password is not supplied, one will be automatically generated.</span>
            </td>
        </tr>
		<tr>
			<td class="formField">Roles</td>
			<td class="formValue">
                <asp:CheckBox ID="RolMemberCheckbox" runat="server" /> Member
                <asp:CheckBox ID="RolAdminCheckbox" runat="server" /> Administrator
                <asp:CheckBox ID="RolSalesRepCheckbox" runat="server" /> Sales Rep
			</td>
		</tr>
		<tr>
			<td class="formField">Account Type*</td>
			<td class="formValue">
				<asp:DropDownList ID="AtyIDDropDownList" runat="server" DataTextField="AtyName" DataValueField="AtyID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="AtyIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AtyIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Account Gender</td>
			<td class="formValue">
				<asp:DropDownList ID="AccGenderDropDownList" runat="server" DataTextField="AtyName" DataValueField="AtyID" AppendDataBoundItems="true">
                    <asp:ListItem Value="F">Female</asp:ListItem>
                    <asp:ListItem Value="M">Male</asp:ListItem>
                    <asp:ListItem Value="?">Unspecified</asp:ListItem>
                </asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class="formField">Date of Birth</td>
			<td class="formValue">
				<asp:TextBox ID="AccBirthdateTextBox" runat="server" MaxLength="20" Width="150" />
			</td>
		</tr>
		<tr>
			<td class="formField">Zip</td>
			<td class="formValue">
				<asp:TextBox ID="AccZipTextBox" runat="server" MaxLength="10" Width="150" />
			</td>
		</tr>
    </table>
	</div>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


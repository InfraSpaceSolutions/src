<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="CitEdit.aspx.cs" Inherits="admin_CitEdit" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="CitMenu" runat="server" Name="Cities" Prefix="Cit" Type="Edit" />
	<table class="formTable" cellspacing="1">
		<tr>
            <td class="formField">ID</td>
            <td class="formValue"><asp:Literal ID="CitIDLiteral" runat="server" /></td>
        </tr>
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="CitNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="CitNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="CitNameTextBox" ErrorMessage="Required" />
				<asp:CustomValidator ID="CitNameDuplicate" runat="server" Display="Dynamic"
					ControlToValidate="CitNameTextBox" ErrorMessage="The supplied city already exists" />
			</td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


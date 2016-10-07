<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="CitNew.aspx.cs" Inherits="admin_CitNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="CitMenu" runat="server" Name="Cities" Prefix="Cit" Type="Create" />
	<asp:UpdatePanel ID="CitNewUpdatePanel" runat="server">
	<ContentTemplate>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue">TBD</td>
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
	</ContentTemplate>
	</asp:UpdatePanel>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PrpEdit.aspx.cs" Inherits="admin_PrpEdit" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PrpMenu" runat="server" Name="Properties" Prefix="Prp" Type="Edit" />
	<table class="formTable" cellspacing="1">
		<tr>
            <td class="formField">ID</td>
            <td class="formValue"><asp:Literal ID="PrpIDLiteral" runat="server" /></td>
        </tr>
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="PrpNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="PrpNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PrpNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Description</td>
			<td class="formValue" align="center"><asp:TextBox ID="PrpDescriptionTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="3" /></td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PrpNew.aspx.cs" Inherits="admin_PrpNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PrpMenu" runat="server" Name="Properties" Prefix="Prp" Type="Create" />
	<asp:UpdatePanel ID="CatNewUpdatePanel" runat="server">
	<ContentTemplate>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue">TBD</td>
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
			<td class="formField">Description</td>
			<td class="formValue" align="center"><asp:TextBox ID="PrpDescriptionTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="3" /></td>
		</tr>
    </table>
	</ContentTemplate>
	</asp:UpdatePanel>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


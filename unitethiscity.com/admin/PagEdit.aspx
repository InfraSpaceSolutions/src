<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PagEdit.aspx.cs" Inherits="admin_PagEdit" ValidateRequest="False" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PagMenu" runat="server" Name="Pages" Prefix="Pag" Type="Edit" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="PagIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Parent Page</td>
			<td class="formValue"><asp:Literal ID="PagParentNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Filename</td>
			<td class="formValue"><asp:Literal ID="PagFilenameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Sequence # *</td>
			<td class="formValue">
				<asp:TextBox ID="PagSequenceTextBox" runat="server" Columns="5" MaxLength="4" Visible="false" />
				<span ID="PagSequenceNote" runat="server" class="note" visible="false">
				Note: This page is not in the top-level navigation.
				It's sequence may be altered <asp:HyperLink ID="PagSequenceHyperLink" runat="server" />.
				</span>
				&nbsp;
				<asp:RequiredFieldValidator ID="PagSequenceRequired" runat="server" Display="Dynamic"
					ControlToValidate="PagSequenceTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="PagNameTextBox" runat="server" Columns="50" MaxLength="128" />
				<span class="note">Note: This is for internal purposes ONLY.</span>
				&nbsp;
				<asp:RequiredFieldValidator ID="PagNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PagNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Navigation Link Name *</td>
			<td class="formValue">
				<asp:TextBox ID="PagNavNameTextBox" runat="server" Columns="50" MaxLength="50" />
                <asp:RequiredFieldValidator ID="PagNavNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PagNavNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
            <td class="formField">Heading *</td>
            <td class="formValue">
                <asp:TextBox ID="PagHeadingTextBox" runat="server" Columns="50" MaxLength="80" />
				<span class="note">Note: The page heading <em>should be the same</em> as the page navigation link name.</span>
				&nbsp;
                <asp:RequiredFieldValidator ID="PagHeadingRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PagHeadingTextBox" ErrorMessage="Required" />
            </td>
        </tr>
		<tr>
			<td class="formField">Title *</td>
			<td class="formValue">
				<asp:TextBox ID="PagTitleTextBox" runat="server" Columns="80" MaxLength="128" />
				<asp:RequiredFieldValidator ID="PagTitleRequired" runat="server" Display="Dynamic"
                        ControlToValidate="PagTitleTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">META Keywords</td>
			<td class="formValue" align="center"><asp:TextBox ID="PagKeywordsTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="3" /></td>
		</tr>
		<tr>
			<td class="formField">META Description</td>
			<td class="formValue" align="center"><asp:TextBox ID="PagDescriptionTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="3" /></td>
		</tr>
		<tr>
			<td class="formField" colspan="2">HTML Body&nbsp;&nbsp;<span class="note">Note: The HTML Editor is available from the view page.</span></td>
		</tr>
		<tr>
			<td class="formValue" colspan="2" align="center"><asp:TextBox ID="PagBodyTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="15" /></td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>
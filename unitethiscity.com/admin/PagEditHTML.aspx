<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PagEditHTML.aspx.cs" Inherits="admin_PagEditHTML" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>
<%@ Register TagPrefix="CE" Namespace="CuteEditor" Assembly="CuteEditor" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PagMenu" runat="server" Name="Pages" Prefix="Pag" Type="Edit HTML" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="PagIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Sequence #</td>
			<td class="formValue"><asp:Literal ID="PagSequenceLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="PagNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Title</td>
			<td class="formValue"><asp:Literal ID="PagTitleLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Filename</td>
			<td class="formValue"><asp:Hyperlink ID="PagFilenameHyperlink" Target="_blank" runat="server" /></td>
		</tr>
	</table>

	<br />

	<h3>HTML Body *</h3>
	<br />
	<CE:Editor id="PagBodyEditor" Width="100%" Height="430px" runat="server" EnableStripIframeTags="false" /> 
	<asp:RequiredFieldValidator ID="PagBodyEditorRequired" runat="server" Display="Dynamic"
            ControlToValidate="PagBodyEditor" ErrorMessage="Required" />
	
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
	</div>
</asp:Content>
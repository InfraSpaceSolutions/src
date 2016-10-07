<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PagView.aspx.cs" Inherits="admin_PagView" MaintainScrollPositionOnPostback="true" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PagMenu" runat="server" Name="Pages" Prefix="Pag" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="PagIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Level</td>
			<td class="formValue"><asp:Literal ID="PagLevelLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Sequence #</td>
			<td class="formValue"><asp:Literal ID="PagSequenceLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Published</td>
			<td class="formValue"><asp:Literal ID="PagPublishedLiteral" runat="server"/> : <asp:LinkButton ID="PagPublishedLinkButton" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Locked</td>
			<td class="formValue"><asp:Literal ID="PagLockedLiteral" runat="server"/></td>
		</tr>
		<tr>
			<td class="formField">Parent Page</td>
			<td class="formValue"><asp:Literal ID="PagParentNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="PagNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Filename</td>
			<td class="formValue">
                <asp:Hyperlink ID="PagFilenameHyperlink" Target="_blank" runat="server" />
                &nbsp;&nbsp;
                <asp:LinkButton ID="GeneratePageFileLinkButton" runat="server" style="color:Red;">Generate Page File</asp:LinkButton>
            </td>
		</tr>
		<tr>
			<td class="formField">Navigation Link Name</td>
			<td class="formValue"><asp:Literal ID="PagNavNameLiteral" runat="server" /></td>
		</tr>
		<tr>
            <td class="formField">Heading</td>
            <td class="formValue"><asp:Literal ID="PagHeadingLiteral" runat="server" /></td>
        </tr>
		<tr>
			<td class="formField">Title</td>
			<td class="formValue"><asp:Literal ID="PagTitleLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">META Keywords</td>
			<td class="formValue"><asp:Literal ID="PagKeywordsLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">META Description</td>
			<td class="formValue"><asp:Literal ID="PagDescriptionLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Timestamp</td>
			<td class="formValue"><asp:Literal ID="PagCreatedTSLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Modified Timestamp</td>
			<td class="formValue"><asp:Literal ID="PagModifiedTSLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Label ID="NoDeleteLabel" runat="server" Visible="false">* Locked pages cannot be deleted. You may unpublish the page if you do not want it visible to the public.</asp:Label>
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this page?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>

	<h3>HTML Body</h3>
	<hr />
	<div class="PublicPreview">
		<h2><asp:Literal ID="DisplayPageHeadingLiteral" runat="server" /></h2>
		<asp:Literal ID="PagBodyLiteral" runat="server" />
	</div>
	<hr style="clear:both;" />

	<div class="commands">
		<asp:Button ID="EditHTMLButton" runat="server" Text="Edit HTML" CssClass="button" />
	</div>
    
    <br />
	<h3>Children Pages</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:22%;">Navigation Name</td>
            <td class="formFieldRepeater" style="width:28%;">Page Name</td>
            <td class="formFieldRepeater" style="width:24%;">Filename</td>
			<td class="formFieldRepeater" style="width:10%;">Sequence #</td>
			<td class="formFieldRepeater" style="width:8%;">Published</td>
        </tr>
        <tr ID="NoChildrenRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No children pages attached to this page.</td>
        </tr>
		<asp:Repeater ID="ChildrenPagesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="PagView.aspx?ID=<%# Eval( "PagID") %>"><%# WebConvert.ToString( Eval( "PagID" ), "&nbsp;" ) %></a></td>
					<td class="formValueRepeater" style="width:22%;"><a href="PagView.aspx?ID=<%# Eval( "PagID") %>"><%# WebConvert.ToString( Eval( "PagNavName" ), "&nbsp;" ) %></a></td>
                    <td class="formValueRepeater" style="width:28%;"><a href="PagView.aspx?ID=<%# Eval( "PagID") %>"><%# WebConvert.ToString( Eval( "PagName"), "&nbsp;" ) %></a></td>
                    <td class="formValueRepeater" style="width:24%;"><a href="PagView.aspx?ID=<%# Eval( "PagID") %>"><%# WebConvert.ToString( Eval( "PagFilename" ), "&nbsp;" ) %></a></td>
					<td class="formValueRepeater" style="width:10%;"><a href="PagView.aspx?ID=<%# Eval( "PagID") %>"><%# WebConvert.ToString( Eval( "PagSequence" ), "&nbsp;" ) %></a></td>
					<td class="formValueRepeater" style="width:8%;" ><a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToBoolean( Eval( "PagPublished"), false ) ? "Yes" : "No" %></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	
	<div class="commands">
		<asp:Button ID="ManageChildrenSequenceButton" runat="server" Text="Manage Sequence" CssClass="button" />
	</div>
</asp:Content>
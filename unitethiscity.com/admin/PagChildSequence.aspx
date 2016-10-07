<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PagChildSequence.aspx.cs" Inherits="admin_PagChildSequence" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PagMenu" runat="server" Name="Pages" Prefix="Pag" Type="Manage Children Pages Sequence"/>
	<h3>Page - <asp:Literal ID="PagNameLiteral" runat="server" /></h3>

	<br />

	<table class="formTableRepeater" cellspacing="1">
		<tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
            <td class="formFieldRepeater" style="width:21%;">Navigation Name</td>
			<td class="formFieldRepeater" style="width:27%;">Page Name</td>
			<td class="formFieldRepeater" style="width:22%;">Filename</td>
			<td class="formFieldRepeater" style="width:9%;">Sequence #</td>
			<td class="formFieldRepeater" style="width:13%;" align="center">Manage Sequence</td>
        </tr>
		<tr ID="NoChildrenRow" runat="server" visible="false">
			<td colspan="6" class="formValue">No children pages available to sequence manage.</td>
		</tr>
		<asp:Repeater ID="ChildrenPagesRepeater" runat="server">
				<ItemTemplate>
					<tr>
						<td class="formValueRepeater" style="width:8%;"><a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagID" ), "&nbsp;" ) %></a></td>
						<td class="formValueRepeater" style="width:21%;"><a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagNavName" ), "&nbsp;" ) %></a></td>
						<td class="formValueRepeater" style="width:27%;"><a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagName" ), "&nbsp;" ) %></a></td>
						<td class="formValueRepeater" style="width:22%;"><a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagFilename" ), "&nbsp;" ) %></a></td>
						<td class="formValueRepeater" style="width:9%;"><a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagSequence" ), "&nbsp;" ) %></a></td>
						<td class="formValueRepeater" style="width:13%;" align="center"><asp:LinkButton ID="UpSequenceLinkButton" runat="server" Text="Up" style="font-weight:bold;" /> - <asp:LinkButton ID="DownSequenceLinkButton" runat="server" Text="Down" style="font-weight:bold;"/></td>
					</tr>
				</ItemTemplate>
		</asp:Repeater>
	</table>
	<div class="commands">
		<input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
	</div>
</asp:Content>


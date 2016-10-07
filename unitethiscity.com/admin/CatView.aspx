<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="CatView.aspx.cs" Inherits="admin_CatView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="CatMenu" runat="server" Name="Categories" Prefix="Cat" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="CatIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Parent Name</td>
			<td class="formValue"><asp:Literal ID="CatParentNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="CatNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField" valign="top">Description</td>
			<td class="formValue"><asp:Literal ID="CatDescriptionLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this category?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
    
    <br />
	<h3>Children Categories</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:92%;">Name</td>
        </tr>
        <tr ID="NoChildrenRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No children categories attached to this category.</td>
        </tr>
		<asp:Repeater ID="ChildrenCategoriesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="CatView.aspx?ID=<%# Eval( "CatID") %>"><%# WebConvert.ToString( Eval( "CatID" ), "&nbsp;" ) %></a></td>
					<td class="formValueRepeater" style="width:92%;"><a href="CatView.aspx?ID=<%# Eval( "CatID") %>"><%# WebConvert.ToString( Eval( "CatName" ), "&nbsp;" ) %></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    
    <asp:Panel ID="AssignedBusinessesPanel" runat="server" Visible="false">
    <br />
    <br />
	<h3>Assigned Businesses</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:46%;">Name</td>
			<td class="formFieldRepeater" style="width:46%;">Formal Name</td>
        </tr>
        <tr ID="NoBusinessesRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No businesses attached to this category.</td>
        </tr>
		<asp:Repeater ID="BusinessesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "BusID" ), "&nbsp;" ) %></a></td>
					<td class="formValueRepeater" style="width:46%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "BusName" ), "&nbsp;" ) %></a></td>
					<td class="formValueRepeater" style="width:46%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "BusFormalName" ), "&nbsp;" ) %></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    </asp:Panel>
</asp:Content>


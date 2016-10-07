<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PerView.aspx.cs" Inherits="admin_PerView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="PerMenu" runat="server" Name="Periods" Prefix="Per" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="PerIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="PerNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Start Date</td>
			<td class="formValue"><asp:Literal ID="PerStartDateLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">End Date</td>
			<td class="formValue"><asp:Literal ID="PerEndDateLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this period?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>

    <br />
	<h3>Deals</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:35%;">Business Name</td>
			<td class="formFieldRepeater" style="width:35%;">Deal Name</td>
			<td class="formFieldRepeater" style="width:22%;">Deal Amount</td>
        </tr>
        <tr ID="NoDealsRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No deals were found for this period.</td>
        </tr>
		<asp:Repeater ID="DealsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="PerDelView.aspx?ID=<%# Eval( "PerID") %>&DelID=<%# Eval( "DelID") %>"><%# WebConvert.ToString( Eval( "DelID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:35%;"><a href="PerDelView.aspx?ID=<%# Eval( "PerID") %>&DelID=<%# Eval( "DelID") %>"><%# WebConvert.ToString( Eval( "BusName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:35%;"><a href="PerDelView.aspx?ID=<%# Eval( "PerID") %>&DelID=<%# Eval( "DelID") %>"><%# WebConvert.ToString( Eval( "DelName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:22%;"><a href="PerDelView.aspx?ID=<%# Eval( "PerID") %>&DelID=<%# Eval( "DelID") %>"><%# String.Format( "{0:C}",  Eval( "DelAmount" ) ) %></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddDealButton" runat="server" Text="Add Deal" CssClass="button" CausesValidation="false" />
	</div>
</asp:Content>


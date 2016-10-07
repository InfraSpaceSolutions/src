<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="RfcView.aspx.cs" Inherits="admin_RfcView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="RfcMenu" runat="server" Name="Referral Codes" Prefix="Rfc" Type="View" />
    <div style="float:right;width:220px;height:220px;">
        <asp:Image ID="RfcQRImage" runat="server" Height="200" Width="200" ImageAlign="Middle" />
    </div>
	<table class="formTable" style="width:75%" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="RfcIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue"><asp:Literal ID="RfcGuidLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Referral Code</td>
			<td class="formValue"><asp:Literal ID="RfcCodeLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Referral Code URL</td>
			<td class="formValue"><asp:Hyperlink ID="RfcCodeHyperlink" target="_blank" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Owner</td>
			<td class="formValue"><asp:Literal ID="AccEMailLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Assigned Business</td>
			<td class="formValue"><asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Allow Checkin?</td>
			<td class="formValue"><asp:Literal ID="RfcAllowCheckinLiteral" runat="server"/></td>
		</tr>
		<tr>
			<td class="formField">Allow Redeem?</td>
			<td class="formValue"><asp:Literal ID="RfcAllowRedeemLiteral" runat="server"/></td>
		</tr>
		<tr>
			<td class="formField">Promotion</td>
			<td class="formValue"><asp:Literal ID="ProNameLiteral" runat="server"/></td>
		</tr>
	</table>
    <br clear="right" />
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this referral code?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
    
    <br />
	<h3>Assigned Accounts <asp:Literal ID="AccCountLiteral" runat="server" /></h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:30%;">First Name</td>
            <td class="formFieldRepeater" style="width:30%;">Last Name</td>
            <td class="formFieldRepeater" style="width:32%;">Email Address</td>
        </tr>
        <tr ID="NoAccountsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No accounts were found.</td>
        </tr>
		<asp:Repeater ID="AccountsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:30%;"><a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccFName" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:30%;"><a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccLName" ), "N/A" ) %></a></td>
                    <td class="formValueRepeater" style="width:32%;"><a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" ) %></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
</asp:Content>


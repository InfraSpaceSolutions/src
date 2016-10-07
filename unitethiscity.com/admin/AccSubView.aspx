<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccSubView.aspx.cs" Inherits="admin_AccSubView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="AccIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue"><asp:HyperLink ID="AccGuidHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Literal ID="AccFNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Literal ID="AccLNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email Address</td>
			<td class="formValue"><asp:HyperLink ID="AccEMailHyperLink" runat="server" /></td>
		</tr>
	</table>

    <br />
    <br />
	<h3>View Subscription</h3>
	<br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="SubIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Product Name</td>
			<td class="formValue"><asp:Literal ID="PrdNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Product Price</td>
			<td class="formValue"><asp:Literal ID="PrdPriceLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Literal ID="SubBillFNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Literal ID="SubBillLNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Address</td>
			<td class="formValue"><asp:Literal ID="SubBillAddressLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">City</td>
			<td class="formValue"><asp:Literal ID="SubBillCityLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">State</td>
			<td class="formValue"><asp:Literal ID="SubBillStateLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Zip Code</td>
			<td class="formValue"><asp:Literal ID="SubBillZipLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Country</td>
			<td class="formValue"><asp:Literal ID="SubBillCtrNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Payment Type</td>
			<td class="formValue"><asp:Literal ID="PtyNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Card Name</td>
			<td class="formValue"><asp:Literal ID="CarNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Card Number</td>
			<td class="formValue"><asp:Literal ID="SubBillCardNumberLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Expire Month</td>
			<td class="formValue"><asp:Literal ID="SubBillExpMonthLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Expire Year</td>
			<td class="formValue"><asp:Literal ID="SubBillExpYearLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Date Created</td>
			<td class="formValue"><asp:Literal ID="SubTSCreateLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Date Modified</td>
			<td class="formValue"><asp:Literal ID="SubTSModifyLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">IP Address</td>
			<td class="formValue"><asp:Literal ID="SubIPAddressLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Payment Method ID</td>
			<td class="formValue"><asp:Literal ID="SubPaymentMethodIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Promotion</td>
			<td class="formValue"><asp:Literal ID="ProNameLiteral" runat="server" /> (<asp:Literal ID="ProIDLiteral" runat="server" />)</td>
		</tr>
		<tr>
			<td class="formField">Billing Date</td>
			<td class="formValue"><asp:Literal ID="SubBillDateLiteral" runat="server" /></td>
		</tr>
    </table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete Subscription" CssClass="button" CausesValidation="false" />
		<asp:Button ID="CancelButton" runat="server" Text="Cancel Subscription" CssClass="button" CausesValidation="false" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
</asp:Content>


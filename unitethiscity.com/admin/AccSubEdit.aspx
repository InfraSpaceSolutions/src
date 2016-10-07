<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccSubEdit.aspx.cs" Inherits="admin_AccSubEdit" %>
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
	<h3>Modify Subscription</h3>
	<br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="SubIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Product Name *</td>
			<td class="formValue">
				<asp:DropDownList ID="PrdIDDropDownList" runat="server" DataTextField="PrdName" AutoPostBack="true" DataValueField="PrdID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="PrdIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PrdIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Product Price</td>
			<td class="formValue">
				<asp:Literal ID="PrdPriceLiteral" runat="server" />
            </td>
		</tr>
		<tr>
			<td class="formField">First Name *</td>
			<td class="formValue">
				<asp:TextBox ID="SubBillFNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="SubBillFNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="SubBillFNameTextBox" ErrorMessage="Required" />
            </td>
		</tr>
		<tr>
			<td class="formField">Last Name *</td>
			<td class="formValue">
				<asp:TextBox ID="SubBillLNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="SubBillLNameTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="SubBillLNameTextBox" ErrorMessage="Required" />
            </td>
		</tr>
		<tr>
			<td class="formField">Address *</td>
			<td class="formValue">
				<asp:TextBox ID="SubBillAddressTextBox" runat="server" MaxLength="128" Width="200" />
				 <asp:RequiredFieldValidator ID="SubBillAddressTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="SubBillAddressTextBox" ErrorMessage="Required" />
            </td>
		</tr>
		<tr>
			<td class="formField">City *</td>
			<td class="formValue">
				<asp:TextBox ID="SubBillCityTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="SubBillCityTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="SubBillCityTextBox" ErrorMessage="Required" />
            </td>
		</tr>
		<tr>
			<td class="formField">State *</td>
            <td class="formValue">
				<asp:DropDownList ID="StaIDDropDownList" runat="server" DataTextField="StaName" DataValueField="StaAbbr" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="StaIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="StaIDDropDownList" ErrorMessage="Required" />
            </td>
		</tr>
		<tr>
			<td class="formField">Zip Code *</td>
			<td class="formValue">
				<asp:TextBox ID="SubBillZipTextBox" runat="server" MaxLength="20" Width="200" />
				 <asp:RequiredFieldValidator ID="SubBillZipTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="SubBillZipTextBox" ErrorMessage="Required" />
            </td>
		</tr>
		<tr>
			<td class="formField">Country *</td>
            <td class="formValue">
				<asp:DropDownList ID="CtrIDDropDownList" runat="server" DataTextField="CtrName" DataValueField="CtrID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="CtrIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="CtrIDDropDownList" ErrorMessage="Required" />
            </td>
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
    </table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


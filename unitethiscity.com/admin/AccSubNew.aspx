<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccSubNew.aspx.cs" Inherits="admin_AccSubNew" %>
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
	<h3>Create Subscription</h3>
	<br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Product Name *</td>
			<td class="formValue">
				<asp:DropDownList ID="PrdIDDropDownList" runat="server" DataTextField="PrdName" DataValueField="PrdID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="PrdIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PrdIDDropDownList" ErrorMessage="Required" />
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
			<td class="formField">Payment Type *</td>
			<td class="formValue">
				<asp:DropDownList ID="PtyIDDropDownList" runat="server" DataTextField="PtyName" DataValueField="PtyID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList> Only Authorize.NET payments are generated; all other types must be configured manually
				 <asp:RequiredFieldValidator ID="PtyIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PtyIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Card Number</td>
			<td class="formValue"><asp:TextBox ID="SubBillCardNumber" runat="server" MaxLength="16" Width="200" /></td>
		</tr>
		<tr>
			<td class="formField">Card CVV2</td>
			<td class="formValue"><asp:TextBox ID="CardCodeTextBox" runat="server" MaxLength="4" Width="100" /></td>
		</tr>
		<tr>
			<td class="formField">Expire Month</td>
			<td class="formValue"><asp:TextBox ID="SubBillExpMonth" runat="server" Maxlength="2" Width="100" /></td>
		</tr>
		<tr>
			<td class="formField">Expire Year</td>
			<td class="formValue"><asp:TextBox ID="SubBillExpYear" runat="server" Maxlength="4" Width="100" /></td>
		</tr>
		<tr>
			<td class="formField">Promotion</td>
			<td class="formValue">
				<asp:DropDownList ID="ProIDDropDownList" runat="server" DataTextField="ProName" DataValueField="ProID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class="formField">Billing Start Date</td>
			<td class="formValue"><asp:TextBox ID="SubBillDateTextBox" runat="server" MaxLength="16" Width="200" />  Leave blank for default based on promotion; must be in the future for Authorize.net</td>
		</tr>
    </table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


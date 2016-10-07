<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusMenuItems.aspx.cs" Inherits="admin_BusMenuItems" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="View" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="BusIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue"><asp:Literal ID="BusGuidLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:HyperLink ID="BusNameHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Formal Name</td>
			<td class="formValue"><asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
		</tr>
	</table>
    <div class="commands">
		<asp:Button ID="SequenceButton" runat="server" Text="Manage Sequence" CssClass="button" CausesValidation="false" />
    </div>
    <br />
    <h3>Add Item</h3>
    <br />
    <table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name</td>
			<td class="formValue">
                <asp:TextBox ID="MenNameTextBox" runat="server" MaxLength="80" Width="350"/>
				<asp:RequiredFieldValidator ID="MenNameTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="MenNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Price</td>
			<td class="formValue">
                <asp:TextBox ID="MenPriceTextBox" runat="server" MaxLength="10" Width="100"/>
				<asp:RequiredFieldValidator ID="MenPriceTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="MenPriceTextBox" ErrorMessage="Required" />
			</td>
		</tr>
    </table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="button" />
	</div>

	<h3>Menu Items</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:40%;">Name</td>
            <td class="formFieldRepeater" style="width:30%;">Price</td>
			<td class="formFieldRepeater" style="width:20%; text-align:center;">Sequence</td>
            <td class="formFieldRepeater" style="width:10%;">Delete</td>
        </tr>
        <tr ID="NoItemsRow" runat="server" visible="false">
            <td colspan="3" class="formValueRepeater">No menu items were found for this business.</td>
        </tr>
		<asp:Repeater ID="MenuItemsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater">
                        <a href="BusMenuItemEdit.aspx?ID=<%# Eval( "MenID") %>"><%# Eval( "MenName" ) %></a>
                    </td>
                    <td class="formValueRepeater">
                        <a href="BusMenuItemEdit.aspx?ID=<%# Eval( "MenID") %>"><%# WebConvert.ToDecimal( Eval( "MenPrice" ), 0 ).ToString( "C" ) %></a>
                    </td>
					<td class="formValueRepeater" style="text-align:center;"><a href="BusMenuItemEdit.aspx?ID=<%# Eval( "MenID") %>"><%# WebConvert.ToString( Eval( "MenSeq" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater"><asp:LinkButton ID="DeleteMenuItemLinkButton" runat="server" CommandArgument='<%# Eval( "MenID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this menu item?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    
</asp:Content>


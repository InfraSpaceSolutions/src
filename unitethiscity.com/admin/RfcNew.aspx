<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="RfcNew.aspx.cs" Inherits="admin_RfcNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="RfcMenu" runat="server" Name="Referral Codes" Prefix="Rfc" Type="Create" />
	<asp:UpdatePanel ID="RfcNewUpdatePanel" runat="server">
	<ContentTemplate>
	<div>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue">TBD</td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue">TBD</td>
		</tr>
		<tr>
			<td class="formField">Assigned Business *</td>
			<td class="formValue">
				<asp:DropDownList ID="BusIDDropDownList" runat="server" AutoPostBack="true" DataTextField="BusName" DataValueField="BusID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                    <asp:ListItem Value="0">None</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="BusIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="BusIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Owner *</td>
			<td class="formValue">
				<asp:DropDownList ID="RfcOwnerDropDownList" runat="server" DataTextField="AccEMail" DataValueField="AccID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                    <asp:ListItem Value="0">None</asp:ListItem>
                    <asp:ListItem Value="-1">By Business Owner</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="RfcOwnerDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="RfcOwnerDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Referral Code *</td>
			<td class="formValue">
				<asp:TextBox ID="RfcCodeTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="RfcCodeRequired" runat="server" Display="Dynamic"
                    ControlToValidate="RfcCodeTextBox" ErrorMessage="Required" />
				<asp:CustomValidator ID="RfcCodeDuplicate" runat="server" Display="Dynamic"
					ControlToValidate="RfcCodeTextBox" ErrorMessage="The supplied referral code already exists" />
			</td>
		</tr>
		<tr>
			<td class="formField">Allow Checkin? *</td>
			<td class="formValue">
                <asp:DropDownList ID="RfcAllowCheckinDropDownList" runat="server" >
                    <asp:ListItem Value="False">No</asp:ListItem>
                    <asp:ListItem Value="True">Yes</asp:ListItem>
                </asp:DropDownList>
				<asp:RequiredFieldValidator ID="RfcAllowCheckinDropDownListRequired" runat="server" Display="Dynamic"
					ControlToValidate="RfcAllowCheckinDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Allow Redeem? *</td>
			<td class="formValue">
                <asp:DropDownList ID="RfcAllowRedeemDropDownList" runat="server" >
                    <asp:ListItem Value="False">No</asp:ListItem>
                    <asp:ListItem Value="True">Yes</asp:ListItem>
                </asp:DropDownList>
				<asp:RequiredFieldValidator ID="RfcAllowRedeemDropDownListRequired" runat="server" Display="Dynamic"
					ControlToValidate="RfcAllowRedeemDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Promotion</td>
			<td class="formValue">
				<asp:DropDownList ID="ProIDDropDownList" runat="server" DataTextField="ProName" DataValueField="ProID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                    <asp:ListItem Value="0">None</asp:ListItem>
                </asp:DropDownList>
			</td>
		</tr>
    </table>
	</div>
	</ContentTemplate>
	</asp:UpdatePanel>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


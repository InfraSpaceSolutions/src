<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusEvtNew.aspx.cs" Inherits="admin_BusEvtNew" %>
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
			<td class="formValue"><asp:Literal ID="BusNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Formal Name</td>
			<td class="formValue"><asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
		</tr>
	</table>
    
    <br />
    <br />
	<h3>Add New Event</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Type *</td>
			<td class="formValue">
			    <asp:DropDownList ID="EttIDDropDownList" runat="server" DataTextField="EttName" DataValueField="EttID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="EttIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EttIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Summary *</td>
			<td class="formValue">
				<asp:TextBox ID="EvtSummaryTextBox" runat="server" MaxLength="140" Width="99%" />	
				<asp:RequiredFieldValidator ID="EvtSummaryRequired" runat="server" Display="Dynamic"
					ControlToValidate="EvtSummaryTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Date *</td>
			<td class="formValue">
			    <dxe:ASPxDateEdit ID="ChkStartDateEdit" runat="server" AllowNull="false">
				    <Paddings PaddingTop="0px" PaddingBottom="0px" />
				    <DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			    </dxe:ASPxDateEdit>
			    <asp:RequiredFieldValidator ID="ChkStartDateEditRequired" runat="server" Display="Dynamic"
				    ControlToValidate="ChkStartDateEdit" ErrorMessage="Required" />
                to
			    <dxe:ASPxDateEdit ID="ChkEndDateEdit" runat="server" AllowNull="true">
				    <Paddings PaddingTop="0px" PaddingBottom="0px" />
				    <DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			    </dxe:ASPxDateEdit>
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Body *</td>
			<td class="formValue">
                <asp:TextBox ID="EvtBodyTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="10" />
				<asp:RequiredFieldValidator ID="EvtBodyTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="EvtBodyTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Event Link</td>
			<td class="formValue">
				<asp:TextBox ID="EventLinkTextBox" runat="server" MaxLength="512" Width="99%" />	
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="MsjNew.aspx.cs" Inherits="admin_MsjNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="MsjMenu" runat="server" Name="Message Jobs" Prefix="Msj" Type="New" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Message Job State</td>
			<td class="formValue">
				<asp:DropDownList ID="MjsIDDropDownList" runat="server" DataTextField="MjsName" DataValueField="MjsID" />
			</td>
		</tr>
		<tr>
			<td class="formField">Send Time</td>
			<td class="formValue">
				<asp:TextBox ID="SendDateTextBox" runat="server" MaxLength="20" Width="100" />
				<asp:DropDownList ID="SendHourDropDownList" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="formField">Recipient Type</td>
			<td class="formValue">
				<asp:DropDownList ID="RolIDDropDownList" runat="server" DataTextField="RolName" DataValueField="RolID" />
			</td>
		</tr>
		<tr>
			<td class="formField">Associated Business</td>
			<td class="formValue">
				<asp:DropDownList ID="BusIDDropDownList" runat="server" DataTextField="BusName" DataValueField="BusID" AppendDataBoundItems="true">
                    <asp:ListItem Value="0">None</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="BusIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="BusIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">From Name *</td>
			<td class="formValue">
				<asp:TextBox ID="MsgFromNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="MsgFromNameTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="MsgFromNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Summary *</td>
			<td class="formValue">
				<asp:TextBox ID="MsgSummaryTextBox" runat="server" MaxLength="128" Width="400" />
				 <asp:RequiredFieldValidator ID="MsgSummaryTextBoxRequired" runat="server" Display="Dynamic"
                    ControlToValidate="MsgSummaryTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" colspan="2">Body *</td>
        </tr>
		<tr>
			<td class="formValue" colspan="2">
				<asp:TextBox ID="MsgBodyTextBox" runat="server" TextMode="MultiLine" Rows="10" Width="800" />
			</td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
    </div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusLocChkNew.aspx.cs" Inherits="admin_BusLocChkNew" %>
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
	<h3>Add Loyalty Point</h3>
	<br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Account *</td>
			<td class="formValue">
			    <asp:DropDownList ID="AccIDDropDownList" runat="server" DataTextField="AccEMail" DataValueField="AccID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="AccIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AccIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Period *</td>
			<td class="formValue">
			    <asp:DropDownList ID="PerIDDropDownList" runat="server" DataTextField="PerName" DataValueField="PerID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="PerIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PerIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Date *</td>
			<td class="formValue">
			<dxe:ASPxDateEdit ID="ChkDateEdit" runat="server" AllowNull="false">
				<Paddings PaddingTop="0px" PaddingBottom="0px" />
				<DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			</dxe:ASPxDateEdit>
			<asp:RequiredFieldValidator ID="ChkDateEditRequired" runat="server" Display="Dynamic"
				ControlToValidate="ChkDateEdit" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Time *</td>
			<td class="formValue">
			<dxe:ASPxTimeEdit ID="ChkTimeEdit" EditFormat="Time" runat="server" AllowNull="false">
				<Paddings PaddingTop="0px" PaddingBottom="0px" />
                <ButtonStyle Paddings-Padding="0px" />
			</dxe:ASPxTimeEdit>
			<asp:RequiredFieldValidator ID="ChkTimeEditRequired" runat="server" Display="Dynamic"
				ControlToValidate="ChkTimeEdit" ErrorMessage="Required" />
			</td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
    </div>
</asp:Content>


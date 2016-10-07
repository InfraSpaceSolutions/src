<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PerNew.aspx.cs" Inherits="admin_PerNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PerMenu" runat="server" Name="Periods" Prefix="Per" Type="Create" />
	<asp:UpdatePanel ID="PerNewUpdatePanel" runat="server">
	<ContentTemplate>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue">TBD</td>
		</tr>
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="PerNameTextBox" runat="server" MaxLength="16" Width="200" />
				 <asp:RequiredFieldValidator ID="PerNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="PerNameTextBox" ErrorMessage="Required" />
				<asp:CustomValidator ID="PerNameDuplicate" runat="server" Display="Dynamic"
					ControlToValidate="PerNameTextBox" ErrorMessage="The supplied period name already exists" />
			</td>
		</tr>
		<tr>
			<td class="formField">Start Date *</td>
			<td class="formValue">
			<dxe:ASPxDateEdit ID="PerStartDateEdit" runat="server" AllowNull="false">
				<Paddings PaddingTop="0px" PaddingBottom="0px" />
				<DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			</dxe:ASPxDateEdit>
			<asp:RequiredFieldValidator ID="PerStartDateEditRequired" runat="server" Display="Dynamic"
				ControlToValidate="PerStartDateEdit" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">End Date *</td>
			<td class="formValue">
			<dxe:ASPxDateEdit ID="PerEndDateEdit" runat="server" AllowNull="false">
				<Paddings PaddingTop="0px" PaddingBottom="0px" />
				<DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			</dxe:ASPxDateEdit>
			<asp:RequiredFieldValidator ID="PerEndDateEditRequired" runat="server" Display="Dynamic"
				ControlToValidate="PerEndDateEdit" ErrorMessage="Required" />
			</td>
		</tr>
    </table>
	</ContentTemplate>
	</asp:UpdatePanel>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusNew.aspx.cs" Inherits="admin_BusNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="Create" />
	<asp:Panel ID="ImageInvalidPanel" CssClass="statusMessage" runat="server" Visible="false">
		<span class="Error">The provided image is invalid.  Please check the image file type <strong>(image/*)</strong>.</span>
	</asp:Panel>
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
			<td class="formField">Category *</td>
			<td class="formValue">
				<asp:DropDownList ID="CatIDDropDownList" runat="server" DataTextField="CatName" DataValueField="CatID" />
				 <asp:RequiredFieldValidator ID="CatIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="CatIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Entertainer</td>
			<td class="formValue">
                <asp:CheckBox ID="EntertainerCheckbox" runat="server"/>  <span class="note">Entertainers are not shown in business lists</span>
			</td>
		</tr>
		<tr>
			<td class="formField">City *</td>
			<td class="formValue">
				<asp:DropDownList ID="CitIDDropDownList" runat="server" DataTextField="CitName" DataValueField="CitID" />
				 <asp:RequiredFieldValidator ID="CitIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="CitIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Business Representative *</td>
			<td class="formValue">
				<asp:DropDownList ID="AccIDDropDownList" runat="server" DataTextField="AccName" DataValueField="AccID" />
				 <asp:RequiredFieldValidator ID="AccIDRequired" runat="server" Display="Dynamic"
                    ControlToValidate="AccIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Business Name *</td>
			<td class="formValue">
				<asp:TextBox ID="BusNameTextBox" runat="server" MaxLength="50" Width="200" />
				 <asp:RequiredFieldValidator ID="BusNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="BusNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Business Formal Name *</td>
			<td class="formValue">
				<asp:TextBox ID="BusFormalNameTextBox" runat="server" MaxLength="128" Width="250" />
				 <asp:RequiredFieldValidator ID="BusFormalNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="BusFormalNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Business Summary</td>
			<td class="formValue">
				<asp:TextBox ID="BusSummaryTextBox" runat="server" MaxLength="140" Width="600" /><br /><span class="note">(up to 140 characters)</span>
			</td>
		</tr>
		<tr>
			<td class="formField">Website Link<br /></td>
			<td class="formValue">
				<asp:TextBox ID="BusWebsiteTextBox" runat="server" MaxLength="255" Width="400" /> <span class="note">(http://www.unitethiscity.com)</span>
			</td>
		</tr>
		<tr>
			<td class="formField">Facebook Link<br /></td>
			<td class="formValue">
				<asp:TextBox ID="BusFacebookLinkTextBox" runat="server" MaxLength="255" Width="400" /> <span class="note">(https://www.facebook.com/UniteThisCity)</span>
			</td>
		</tr>
		<tr>
			<td class="formField">Facebook Profile ID<br /></td>
			<td class="formValue">
				<asp:TextBox ID="BusFacebookIDTextBox" runat="server" MaxLength="50" Width="400" /> <span class="note">(289883941125076)</span>
			</td>
		</tr>
		<tr>
			<td class="formField" style="vertical-align:top;">Logo Upload</td>
			<td class="formValue">
				<asp:FileUpload ID="LogoFileUpload" Size="70" runat="server" />
			</td>
		</tr>
    </table>
	</div>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


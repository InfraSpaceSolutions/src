<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" ClientIDMode="Static" AutoEventWireup="true" CodeFile="BusEdit.aspx.cs" Inherits="admin_BusEdit" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
	<script type="text/javascript" src="/Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript">
        function LookupFacebookProfileID() {
            var name = $('#BusFacebookIDTextBox')[0].value;
            $.getJSON('https://graph.facebook.com/' + name) 
            .done(function (data) {
                $('#BusFacebookIDTextBox')[0].value = data.id;
            })
            .fail(function(jqxhr, textStatus, error) {
                alert('Facebook Profile ID not found');
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="Edit" />
	<asp:Panel ID="ImageInvalidPanel" CssClass="statusMessage" runat="server" Visible="false">
		<span class="Error">The provided image is invalid.  Please check the image file type <strong>(image/*)</strong>.</span>
	</asp:Panel>
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
                <asp:Button ID="GetFacebookIDButton" runat="server" Text="Name to Profile ID" CausesValidation="false" OnClientClick="javascript: LookupFacebookProfileID(); return false;" CssClass="button" />
                <div>Apps require the numeric Profile ID to access Facebook. To look up a Profile ID, enter the profile name and use the conversion button.</div>
			</td>
		</tr>
		<tr>
			<td class="formField">Proximity Range</td>
			<td class="formValue">
				<asp:TextBox ID="BusProximityRange" runat="server" MaxLength="50" Width="150" />
                <div>The proximity range can be used to prevent auto-scanning. The user must be within the specified number of meters of the location when attempting to check in or redeem. Set to 1000000 (1000 km) to disable.</div>
			</td>
		</tr>
		<tr>
			<td class="formField">Menu Link<br /></td>
			<td class="formValue">
				<asp:TextBox ID="MenLinkTextBox" runat="server" MaxLength="255" Width="400" /> <span class="note">Link to an online menu - (https://www.mywesite.com/menu.pdf)</span>
			</td>
		</tr>
		<tr>
			<td class="formField" style="vertical-align:top;">Logo Upload</td>
			<td class="formValue">
				<asp:FileUpload ID="LogoFileUpload" Size="70" runat="server" />
				<div class="note">* Leave blank if you do not want to update the logo.</div>
			</td>
		</tr>
    </table>
    <div class="commands">
        <asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript:history.go(-1);" />
    </div>
</asp:Content>


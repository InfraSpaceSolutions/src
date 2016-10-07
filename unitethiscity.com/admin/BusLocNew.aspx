<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" ClientIDMode="Static" AutoEventWireup="true" CodeFile="BusLocNew.aspx.cs" Inherits="admin_BusLocNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3&key=<%= SiteSettings.GetValue( "GoogleMapsAPIKey" ) %>&sensor=false"></script>
	<script type="text/javascript" language="JavaScript">
	    var objGeocoder;

	    function BuildLocationIdentifier() {
	        var addrResult = "";

	        if (obj = document.getElementById("LocAddressTextBox")) {
	            if (obj.value != "") {
	                addrResult += (addrResult != "") ? ", " : "";
	                addrResult += obj.value;
	            }
	        }
	        if (obj = document.getElementById("LocCityTextBox")) {
	            if (obj.value != "") {
	                addrResult += (addrResult != "") ? ", " : "";
	                addrResult += obj.value;
	            }
	        }
	        if (obj = document.getElementById("StaNameDropDownList")) {
	            if (obj.value != "") {
	                addrResult += (addrResult != "") ? ", " : "";
	                addrResult += obj.value;
	            }
	        }
	        if (obj = document.getElementById("LocZipTextBox")) {
	            if (obj.value != "") {
	                addrResult += (addrResult != "") ? ", " : "";
	                addrResult += obj.value;
	            }
	        }

	        return addrResult;
	    }

	    function SetLocationCoords() {
	        objGeocoder = new google.maps.Geocoder();
	        var strLocIdentifier = "";
	        var latlng = "";
	        var longitude = 0;
	        var latitidue = 0;

	        strLocIdentifier = BuildLocationIdentifier();
	        document.getElementById("LocIdentifierTextBox").value = strLocIdentifier;

	        objGeocoder.geocode({ 'address': strLocIdentifier }, function (results, status) {
	            if (status == google.maps.GeocoderStatus.OK) {
	                document.getElementById("LocLatitudeTextBox").value = results[0].geometry.location.lat();
	                document.getElementById("LocLongitudeTextBox").value = results[0].geometry.location.lng();
	            } else {
	                alert("Geocode was not successful for the following reason: " + status);
	            }
	        });
	    }

	    function SetLocation() {
	        document.getElementById("LocLookupLink").href = "http://maps.google.com/maps?q=" + document.getElementById("LocIdentifierTextBox").value;
	    }
    </script>
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
	<h3>Add New Location</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name *</td>
			<td class="formValue">
				<asp:TextBox ID="LocNameTextBox" runat="server" MaxLength="50" Width="200" />
				<asp:RequiredFieldValidator ID="LocNameRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Address *</td>
			<td class="formValue">
				<asp:TextBox ID="LocAddressTextBox" runat="server" MaxLength="50" Width="300" />
				<asp:RequiredFieldValidator ID="LocAddressTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocAddressTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">City *</td>
			<td class="formValue">
				<asp:TextBox ID="LocCityTextBox" runat="server" MaxLength="50" Width="200" />
				<asp:RequiredFieldValidator ID="LocCityRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocCityTextBox" ErrorMessage="Required" />
			</td>
		</tr>
        <tr>
            <td class="formField">State *</td>
            <td class="formValue">
                <asp:DropDownList ID="StaNameDropDownList" DataValueField="StaName" DataTextField="StaName" AppendDataBoundItems="true" runat="server">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				 <asp:RequiredFieldValidator ID="StaNameDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="StaNameDropDownList" ErrorMessage="Required" />
            </td>
        </tr>
		<tr>
			<td class="formField">Zip Code *</td>
			<td class="formValue">
				<asp:TextBox ID="LocZipTextBox" runat="server" MaxLength="50" Width="100" />
				<asp:RequiredFieldValidator ID="LocZipTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocZipTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Phone *</td>
			<td class="formValue">
				<asp:TextBox ID="LocPhoneTextBox" runat="server" MaxLength="50" Width="150" />
				<asp:RequiredFieldValidator ID="LocPhoneTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocPhoneTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" colspan="2">After entering the address above, you must click the "Set Location Coords" button so that the Google map will show the new address.</td>
		</tr>
		<tr>
			<td class="formField">Location Identifier *</td>
			<td class="formValue">
				<asp:TextBox ID="LocIdentifierTextBox" runat="server" MaxLength="150" Width="200" />
                &nbsp;
				<a href="http://maps.google.com" onclick="javascript:SetLocation();" ID="LocLookupLink" target="_blank">View Map</a>
			</td>
		</tr>
		<tr>
			<td class="formField">Latitude, Longitude *</td>
			<td class="formValue">
				<asp:TextBox ID="LocLatitudeTextBox" runat="server" MaxLength="128" Width="200" />
				<asp:RequiredFieldValidator ID="LocLatitudeTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocLatitudeTextBox" ErrorMessage="Required" />
				<asp:TextBox ID="LocLongitudeTextBox" runat="server" MaxLength="128" Width="200" />
				<asp:RequiredFieldValidator ID="LocLongitudeTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="LocLongitudeTextBox" ErrorMessage="Required" />
                &nbsp;
                &nbsp;
                <asp:Button ID="SetCoordsButton" runat="server" Text="Set Location Coords" CausesValidation="false" OnClientClick="javascript: SetLocationCoords(); return false;" CssClass="button" />
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
        <asp:HiddenField ID="LocLatLng" runat="server" Value="" />
	</div>
</asp:Content>


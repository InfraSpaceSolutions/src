<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="BusView.aspx.cs" Inherits="admin_BusView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="View" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
    <div style="float:right;width:220px;height:220px;">
        <asp:Image ID="BusQRImage" runat="server" Height="200" Width="200" ImageAlign="Middle" />
    </div>
	<table class="formTable" style="width:75%" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="BusIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue"><asp:Literal ID="BusGuidLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Enabled</td>
			<td class="formValue"><asp:Literal ID="BusEnabledLiteral" runat="server"/> : <asp:LinkButton ID="BusEnabledLinkButton" runat="server" CausesValidation="false" /></td>
		</tr>
		<tr>
			<td class="formField">Require Pin</td>
			<td class="formValue"><asp:Literal ID="BusRequirePinLiteral" runat="server"/> : <asp:LinkButton ID="BusRequirePinLinkButton" runat="server" CausesValidation="false" /></td>
		</tr>
		<tr>
			<td class="formField">City</td>
			<td class="formValue"><asp:Literal ID="CitNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Account Manager</td>
			<td class="formValue"><asp:HyperLink ID="AccEMailHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Category</td>
			<td class="formValue"><asp:HyperLink ID="CatNameHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Entertainer</td>
			<td class="formValue"><asp:Literal ID="EntertainerLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="BusNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Formal Name</td>
			<td class="formValue"><asp:Literal ID="BusFormalNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Summary</td>
			<td class="formValue"><asp:Literal ID="BusSummaryLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Rating</td>
			<td class="formValue"><asp:Literal ID="BusRatingLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Website</td>
			<td class="formValue"><asp:HyperLink ID="BusWebsiteHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Facebook Link</td>
			<td class="formValue"><asp:HyperLink ID="BusFacebookLinkHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Facebook Profile ID</td>
			<td class="formValue"><asp:HyperLink ID="BusFacebookIDHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Check-In Proximity</td>
			<td class="formValue"><asp:Literal ID="BusProximityRangeLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Menu Link</td>
			<td class="formValue"><asp:HyperLink ID="MenLinkHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Assigned Social Deal</td>
			<td class="formValue">
                <asp:DropDownList ID="BusAssignedDldIDDropDownList" runat="server" DataTextField="DldName" DataValueField="DldID" />
                <asp:Button ID="UpdateBusAssignedDldIDButton" runat="server" Text="Update" CssClass="button" CausesValidation="false" />
		        <asp:Label ID="UpdateMessageLabel" runat="server" CssClass="Success" Visible="false" />
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this business?');" />
        <asp:Button ID="GalleryItemsButton" runat="server" Text="Image Gallery" CssClass="button" CausesValidation="false" />
        <asp:Button ID="MenuItemsButton" runat="server" Text="Menu Items" CssClass="button" CausesValidation="false" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" CausesValidation="false" />
	</div>

    <asp:Panel ID="BusImagePanel" runat="server" visible="false">
        <asp:Button ID="ImageDeleteButton" runat="server" Text="Delete Logo" CssClass="button" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to delete this logo?');" /><br /><br />
    </asp:Panel>
    
    <br />
	<h3>Locations</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:50%;">Name</td>
            <td class="formFieldRepeater" style="width:30%;">City</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoLocationsRow" runat="server" visible="false">
            <td colspan="4" class="formValueRepeater">No locations were found for this business.</td>
        </tr>
		<asp:Repeater ID="LocationsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:50%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocName" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:30%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocCity" ), "&nbsp;" ) %></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteLocationLinkButton" runat="server" CommandArgument='<%# Eval( "LocID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this location from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddLocationButton" runat="server" Text="Add Location" CssClass="button" CausesValidation="false" />
	</div>
    <br />
	<h3>Associated Accounts</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:10%;">ID</td>
			<td class="formFieldRepeater" style="width:20%;">First Name</td>
			<td class="formFieldRepeater" style="width:20%;">Last Name</td>
			<td class="formFieldRepeater" style="width:30%;">E-Mail Address</td>
			<td class="formFieldRepeater" style="width:20%;">Role</td>
        </tr>
        <tr ID="NoRolesRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No associated account roles were found for this business.</td>
        </tr>
		<asp:Repeater ID="RolesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:10%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccFName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccLName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:30%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "RolName" ), "&nbsp;" )%></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    <br />
	<h3>Properties</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:80%;">Name</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoPropertiesRow" runat="server" visible="false">
            <td colspan="3" class="formValueRepeater">No properties were found for this business.</td>
        </tr>
		<asp:Repeater ID="PropertiesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="PrpView.aspx?ID=<%# Eval( "PrpID") %>"><%# WebConvert.ToString( Eval( "PrpID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:80%;"><a href="PrpView.aspx?ID=<%# Eval( "PrpID") %>"><%# WebConvert.ToString( Eval( "PrpName" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeletePropertiesLinkButton" runat="server" CommandArgument='<%# Eval( "BprID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this property from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="ManagePropertyButton" runat="server" Text="Manage Properties" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Social Deal Definitions</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:50%;">Name</td>
            <td class="formFieldRepeater" style="width:30%;">Amount</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoDealDefinitionsRow" runat="server" visible="false">
            <td colspan="4" class="formValueRepeater">No deal definitions were found for this business.</td>
        </tr>
		<asp:Repeater ID="DealDefinitionsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusDldEdit.aspx?ID=<%# Eval( "BusID") %>&DldID=<%# Eval( "DldID") %>"><%# WebConvert.ToString( Eval( "DldID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:50%;"><a href="BusDldEdit.aspx?ID=<%# Eval( "BusID") %>&DldID=<%# Eval( "DldID") %>"><%# WebConvert.ToString( Eval( "DldName" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:30%;"><a href="BusDldEdit.aspx?ID=<%# Eval( "BusID") %>&DldID=<%# Eval( "DldID") %>"><%# String.Format( "{0:C}",  Eval( "DldAmount" ) ) %></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteDealDefinitionLinkButton" runat="server" CommandArgument='<%# Eval( "DldID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this deal definition from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddDealDefinitionButton" runat="server" Text="Add Social Deal Definition" CssClass="button" CausesValidation="false" />
	</div>

    <br />
	<h3>Pins</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:20%;">Number</td>
			<td class="formFieldRepeater" style="width:36%;">Name</td>
			<td class="formFieldRepeater" style="width:12%;">Date Created</td>
			<td class="formFieldRepeater" style="width:12%;">Enabled</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoPinsRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No pins were found for this business.</td>
        </tr>
		<asp:Repeater ID="PinsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusPinEdit.aspx?ID=<%# Eval( "BusID") %>&pinID=<%# Eval( "PinID") %>"><%# WebConvert.ToString( Eval( "PinID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="BusPinEdit.aspx?ID=<%# Eval( "BusID") %>&pinID=<%# Eval( "PinID") %>"><%# WebConvert.ToString( Eval( "PinNumber" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:36%;"><a href="BusPinEdit.aspx?ID=<%# Eval( "BusID") %>&pinID=<%# Eval( "PinID") %>"><%# WebConvert.ToString( Eval( "PinName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:12%;"><a href="BusPinEdit.aspx?ID=<%# Eval( "BusID") %>&pinID=<%# Eval( "PinID") %>"><%# WebConvert.ToDateTime( Eval( "PinTS" ), DateTime.Now ).ToShortDateString( )%></a></td>
					<td class="formValueRepeater" style="width:12%;"><a href="BusPinEdit.aspx?ID=<%# Eval( "BusID") %>&pinID=<%# Eval( "PinID") %>"><%# WebConvert.ToBoolean( Eval( "PinEnabled" ), false ) ? "Yes" : "No"%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteBusinessPropertiesLinkButton" runat="server" CommandArgument='<%# Eval( "PinID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this pin from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddPinButton" runat="server" Text="Add Pin" CssClass="button" CausesValidation="false" />
	</div>

    <br />
	<h3>Social Deal Redemptions</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:30%;">Account</td>
			<td class="formFieldRepeater" style="width:20%;">Deal Name</td>
			<td class="formFieldRepeater" style="width:10%;">Deal Amount</td>
			<td class="formFieldRepeater" style="width:20%;">Date Created</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoRedemptionsRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No redemptions were found for this business.</td>
        </tr>
		<asp:Repeater ID="RedemptionsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><%# WebConvert.ToString( Eval( "RedID" ), "&nbsp;" )%></td>
					<td class="formValueRepeater" style="width:30%;"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></td>
					<td class="formValueRepeater" style="width:20%;"><%# WebConvert.ToString( Eval( "DelName" ), "&nbsp;" )%></td>
					<td class="formValueRepeater" style="width:10%;"><%# String.Format( "{0:C}",  Eval( "DelAmount" ) ) %></td>
					<td class="formValueRepeater" style="width:20%;"><%# WebConvert.ToDateTime( Eval( "RedTS" ), DateTime.Now )%></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteRedemptionLinkButton" runat="server" CommandArgument='<%# Eval( "RedID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this redemption from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddRedemptionButton" runat="server" Text="Add Redemption" CssClass="button" CausesValidation="false" />
	</div>

    <br />
	<h3>Events</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:20%;">When</td>
			<td class="formFieldRepeater" style="width:20%;">Type</td>
			<td class="formFieldRepeater" style="width:40%;">Summary</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoEventsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No events were found for this business.</td>
        </tr>
		<asp:Repeater ID="EventsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusEvtEdit.aspx?ID=<%# Eval( "BusID") %>&EvtID=<%# Eval( "EvtID") %>"><%# WebConvert.ToString( Eval( "EvtID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="BusEvtEdit.aspx?ID=<%# Eval( "BusID") %>&EvtID=<%# Eval( "EvtID") %>"><%# SiteEvent.EventDateToString(WebConvert.ToDateTime( Eval( "EvtStartDate" ), DateTime.Now ), WebConvert.ToDateTime( Eval( "EvtEndDate" ), DateTime.Now ))%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="BusEvtEdit.aspx?ID=<%# Eval( "BusID") %>&EvtID=<%# Eval( "EvtID") %>"><%# WebConvert.ToString( Eval( "EttName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:40%;"><a href="BusEvtEdit.aspx?ID=<%# Eval( "BusID") %>&EvtID=<%# Eval( "EvtID") %>"><%# WebConvert.ToString( Eval( "EvtSummary" ), "&nbsp;" ) %></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteEventLinkButton" runat="server" CommandArgument='<%# Eval( "EvtID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this event from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddEventButton" runat="server" Text="Add Event" CssClass="button" CausesValidation="false" />
	</div>

    <br />
	<h3>Recurring Events</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:25%;">Name</td>
			<td class="formFieldRepeater" style="width:20%;">Date Range</td>
			<td class="formFieldRepeater" style="width:25%;">Summary</td>
			<td class="formFieldRepeater" style="width:10%;">Active</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoRecurringEventsRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No recurring events were found for this business.</td>
        </tr>
		<asp:Repeater ID="RecurringEventsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusEvjEdit.aspx?ID=<%# Eval( "BusID") %>&EvjID=<%# Eval( "EvjID") %>"><%# WebConvert.ToString( Eval( "EvjID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:25%;"><a href="BusEvjEdit.aspx?ID=<%# Eval( "BusID") %>&EvjID=<%# Eval( "EvjID") %>"><%# WebConvert.ToString( Eval( "EvjName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:20%;"><a href="BusEvjEdit.aspx?ID=<%# Eval( "BusID") %>&EvjID=<%# Eval( "EvjID") %>"><%# SiteEvent.EventDateToString(WebConvert.ToDateTime( Eval( "EvjBeginDate" ), DateTime.Now ), WebConvert.ToDateTime( Eval( "EvjStopDate" ), DateTime.Now ))%></a></td>
					<td class="formValueRepeater" style="width:25%;"><a href="BusEvjEdit.aspx?ID=<%# Eval( "BusID") %>&EvjID=<%# Eval( "EvjID") %>"><%# WebConvert.Summarize(WebConvert.ToString( Eval( "EvjSummary" ), "&nbsp;" ), 30)%></a></td>
					<td class="formValueRepeater" style="width:10%;"><a href="BusEvjEdit.aspx?ID=<%# Eval( "BusID") %>&EvjID=<%# Eval( "EvjID") %>"><%# WebConvert.ToBoolean( Eval( "EvjEnabled" ), false ) ? "Yes" : "No"%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteEventLinkButton" runat="server" CommandArgument='<%# Eval( "EvjID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this recurring event from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddRecurringEventButton" runat="server" Text="Add Recurring Event" CssClass="button" CausesValidation="false" />
	</div>
	<h3>Opt Outs</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:25%;">Name</td>
			<td class="formFieldRepeater" style="width:30%;">Business</td>
			<td class="formFieldRepeater" style="width:25%;">Timestamp</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoOptOutsRow" runat="server" visible="false">
            <td colspan="6" class="formValueRepeater">No opt outs were found for this business.</td>
        </tr>
		<asp:Repeater ID="OptOutRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><%# WebConvert.ToString( Eval( "OptID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:25%;"><%# WebConvert.ToString( Eval( "AccFName" ) + " " + Eval( "AccLName" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:30%;"><%# WebConvert.ToString( Eval( "BusName" ), "&nbsp;")%></a></td>
					<td class="formValueRepeater" style="width:25%;"><%# WebConvert.ToDateTime( Eval( "OptTS" ), DateTime.Now ).ToShortDateString( ) %></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteOptOutLinkButton" runat="server" CommandArgument='<%# Eval( "OptID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this opt out from this business?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    <br />
    <br />
</asp:Content>


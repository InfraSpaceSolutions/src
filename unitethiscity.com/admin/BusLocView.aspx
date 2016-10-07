<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusLocView.aspx.cs" Inherits="admin_BusLocView" %>
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
    
    <br />
    <br />
	<h3>View Location</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="LocNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Address</td>
			<td class="formValue"><asp:Literal ID="LocAddressLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">City</td>
			<td class="formValue"><asp:Literal ID="LocCityLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">State</td>
			<td class="formValue"><asp:Literal ID="LocStateLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Zip Code</td>
			<td class="formValue"><asp:Literal ID="LocZipLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Phone</td>
			<td class="formValue"><asp:Literal ID="LocPhoneLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Latitude</td>
			<td class="formValue"><asp:Literal ID="LocLatitudeLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Longitude</td>
			<td class="formValue"><asp:Literal ID="LocLongitudeLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Rating</td>
			<td class="formValue"><asp:Literal ID="LocRatingLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Loyalty Deal</td>
			<td class="formValue"><asp:Literal ID="LoyaltyDealLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="LoyaltyDealButton" runat="server" Text="Loyalty Deal" CssClass="button" />
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this location?');" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
    
    <br />
	<h3>Location Properties</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:80%;">Name</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoPropertiesRow" runat="server" visible="false">
            <td colspan="3" class="formValueRepeater">No properties were found for this location.</td>
        </tr>
		<asp:Repeater ID="PropertiesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="PrpView.aspx?ID=<%# Eval( "PrpID") %>"><%# WebConvert.ToString( Eval( "PrpID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:80%;"><a href="PrpView.aspx?ID=<%# Eval( "PrpID") %>"><%# WebConvert.ToString( Eval( "PrpName" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeletePropertiesLinkButton" runat="server" CommandArgument='<%# Eval( "LprID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this property from this location?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="ManagePropertyButton" runat="server" Text="Manage Properties" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Account Favorite</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:60%;">Account</td>
            <td class="formFieldRepeater" style="width:20%;">Timestamp</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoFavoritesRow" runat="server" visible="false">
            <td colspan="4" class="formValueRepeater">No favorites were found for this location.</td>
        </tr>
		<asp:Repeater ID="FavoritesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "FavID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:60%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# ( Eval( "favTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "FavTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteFavoriteLinkButton" runat="server" CommandArgument='<%# Eval( "FavID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this favorite from this location?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddFavoriteButton" runat="server" Text="Add Account Favorite" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Reviews</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:20%;">Account</td>
			<td class="formFieldRepeater" style="width:40%;">Review</td>
            <td class="formFieldRepeater" style="width:20%;">Date Created</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoTipsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No reviews were found for this location.</td>
        </tr>
		<asp:Repeater ID="TipsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "TipID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:40%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.PreserveBreaks( WebConvert.ToString( Eval( "TipText" ), "&nbsp;" ) )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# ( Eval( "TipTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "TipTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteTipLinkButton" runat="server" CommandArgument='<%# Eval( "TipID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this tip from this location?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddTipButton" runat="server" Text="Add Review" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Ratings</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:50%;">Account</td>
			<td class="formFieldRepeater" style="width:10%;">Rating</td>
            <td class="formFieldRepeater" style="width:20%;">Date Created</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoRatingsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No ratings were found for this location.</td>
        </tr>
		<asp:Repeater ID="RatingsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "RatID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:50%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:10%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "RatRating" ), "&nbsp;" )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# ( Eval( "RatTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "RatTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteRatingLinkButton" runat="server" CommandArgument='<%# Eval( "RatID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this rating from this location?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddRatingButton" runat="server" Text="Add Rating" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Loyalty Points</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:30%;">Account Name</td>
			<td class="formFieldRepeater" style="width:30%;">Period</td>
            <td class="formFieldRepeater" style="width:20%;">Date Checked-In</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoCheckInsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No loyalty points were found for this location.</td>
        </tr>
		<asp:Repeater ID="CheckInsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "ChkID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# WebConvert.ToString( Eval( "PerName" ), "&nbsp;" )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="AccView.aspx?ID=<%# Eval( "AccID") %>"><%# ( Eval( "ChkTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "ChkTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteCheckInLinkButton" runat="server" CommandArgument='<%# Eval( "ChkID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this check-in from this location?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddCheckInButton" runat="server" Text="Add Loyalty" CssClass="button" CausesValidation="false" />
	</div>
</asp:Content>


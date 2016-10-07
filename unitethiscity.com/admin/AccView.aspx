<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="AccView.aspx.cs" Inherits="admin_AccView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="View" />
	<asp:Panel ID="WelcomePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="WelcomeMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
    <div style="float:right;width:220px;height:220px;">
        <asp:Image ID="AccQRImage" runat="server" Height="200" Width="200" ImageAlign="Middle" />
    </div>
	<table class="formTable" style="width:75%" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="AccIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Guid</td>
			<td class="formValue"><asp:Literal ID="AccGuidLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Enabled</td>
			<td class="formValue"><asp:Literal ID="AccEnabledLiteral" runat="server"/> : <asp:LinkButton ID="AccEnabledLinkButton" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Literal ID="AccFNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Literal ID="AccLNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email Address</td>
			<td class="formValue"><asp:HyperLink ID="AccEMailHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">City</td>
			<td class="formValue"><asp:Literal ID="CitNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Phone</td>
			<td class="formValue"><asp:Literal ID="AccPhoneLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Referral Code</td>
			<td class="formValue"><asp:Literal ID="RfcCodeLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Roles</td>
			<td class="formValue">
                <asp:Literal ID="RolMemberLiteral" runat="server" Text="Member " />
                <asp:Literal ID="RolAdminLiteral" runat="server" Text="Administrator " />
                <asp:Literal ID="RolSalesRepLiteral" runat="server" Text="SalesRep" />
			</td>
		</tr>
		<tr>
			<td class="formField">Stats and Analytics</td>
			<td class="formValue">
                <asp:Literal ID="GlobalStatsLiteral" runat="server" Text="Global-Stats " Visible="false" />
                <asp:Literal ID="GlobalAnalyticsLiteral" runat="server" Text="Global-Analytics " Visible="false" />
                <asp:Literal ID="BusinessStatsLiteral" runat="server" Text="Business-Stats " Visible="false" />
                <asp:Literal ID="BusinessAnalyticsLiteral" runat="server" Text="Business-Analytics " Visible="false" />
                &nbsp;
			</td>
		</tr>
		<tr>
			<td class="formField">Account Type</td>
			<td class="formValue"><asp:Literal ID="AtyNameLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Account Gender</td>
			<td class="formValue"><asp:Literal ID="AccGenderLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Account Date of Birth</td>
			<td class="formValue"><asp:Literal ID="AccBirthDateLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Account Zip</td>
			<td class="formValue"><asp:Literal ID="AccZipLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Facebook Identifier</td>
			<td class="formValue"><asp:Literal ID="AccFacebookIdentifierLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Account Creation</td>
			<td class="formValue"><asp:Literal ID="AccTSCreatedLiteral" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this account?');" />
		<asp:Button ID="ForcePasswordButton" runat="server" Text="Force Password" CssClass="button" />
		<asp:Button ID="SendCredentialButton" runat="server" Text="Send Welcome" CssClass="button" OnClientClick="javascript:return confirm( 'Are you sure you want to send a welcome e-mail to this account?' );" />
		<asp:Button ID="EditButton" runat="server" Text="Edit" CssClass="button" />
	</div>
    
    <br />
	<h3>Business Roles</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:30%;">Role Name</td>
            <td class="formFieldRepeater" style="width:30%;">Business</td>
            <td class="formFieldRepeater" style="width:20%;">Access Level</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoRolesRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No roles were found for this account.</td>
        </tr>
		<asp:Repeater ID="RolesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><%# WebConvert.ToString( Eval( "AcrID" ), "&nbsp;" )%></td>
					<td class="formValueRepeater" style="width:30%;"><%# WebConvert.ToString( Eval( "RolName" ), "&nbsp;" )%></td>
                    <td class="formValueRepeater" style="width:30%;"><%# WebConvert.ToString( Eval( "BusName" ), "N/A" ) %></td>
                    <td class="formValueRepeater" style="width:20%;"><%# WebConvert.ToString( Eval( "AclName" ), "&nbsp;" ) %></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteRoleLinkButton" runat="server" CommandArgument='<%# Eval( "AcrID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this role from this account?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddRoleButton" runat="server" Text="Add Role" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Favorites</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:60%;">Location Name</td>
            <td class="formFieldRepeater" style="width:20%;">Timestamp</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoFavoritesRow" runat="server" visible="false">
            <td colspan="4" class="formValueRepeater">No favorite locations were found for this account.</td>
        </tr>
		<asp:Repeater ID="FavoritesRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater" style="width:8%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "FavID" ), "&nbsp;" )%></a></td>
					<td class="formValueRepeater" style="width:60%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocName" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" style="width:20%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# ( Eval( "favTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "FavTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteFavoriteLinkButton" runat="server" CommandArgument='<%# Eval( "FavID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this favorite location from this account?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddFavoriteButton" runat="server" Text="Add Favorite Location" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Reviews</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:20%;">Location Name</td>
			<td class="formFieldRepeater" style="width:40%;">Review</td>
            <td class="formFieldRepeater" style="width:20%;">Date Created</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoTipsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No reviews were found for this account.</td>
        </tr>
		<asp:Repeater ID="TipsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "TipID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:20%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocName" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:40%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.PreserveBreaks( WebConvert.ToString( Eval( "TipText" ), "&nbsp;" ) )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# ( Eval( "TipTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "TipTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteTipLinkButton" runat="server" CommandArgument='<%# Eval( "TipID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this tip from this account?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddTipButton" runat="server" Text="Add Review" CssClass="button" CausesValidation="false" />
	</div>
    
    <br />
	<h3>Location Ratings</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:50%;">Location Name</td>
			<td class="formFieldRepeater" style="width:10%;">Rating</td>
            <td class="formFieldRepeater" style="width:20%;">Date Created</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoRatingRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No location ratings were found for this account.</td>
        </tr>
		<asp:Repeater ID="RatingsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "RatID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:50%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocName" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:10%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "RatRating" ), "&nbsp;" )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# ( Eval( "RatTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "RatTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteRatingLinkButton" runat="server" CommandArgument='<%# Eval( "RatID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this rating from this account?');" /></td>
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
			<td class="formFieldRepeater" style="width:30%;">Location Name</td>
			<td class="formFieldRepeater" style="width:30%;">Period</td>
            <td class="formFieldRepeater" style="width:20%;">Date Checked-In</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoCheckInsRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No loyalty points were found for this account.</td>
        </tr>
		<asp:Repeater ID="CheckInsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "ChkID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "LocName" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# WebConvert.ToString( Eval( "PerName" ), "&nbsp;" )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="BusLocView.aspx?ID=<%# Eval( "BusID") %>&LocID=<%# Eval( "LocID") %>"><%# ( Eval( "ChkTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "ChkTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteCheckInLinkButton" runat="server" CommandArgument='<%# Eval( "ChkID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this checkin from this account?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddCheckInButton" runat="server" Text="Add Loyalty" CssClass="button" CausesValidation="false" />
	</div>

	<h3>Social Deal Redemptions</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:30%;">Location Name</td>
			<td class="formFieldRepeater" style="width:10%;">Period</td>
			<td class="formFieldRepeater" style="width:10%;">PIN</td>
			<td class="formFieldRepeater" style="width:10%;">Amount</td>
            <td class="formFieldRepeater" style="width:20%;">Date Redeemed</td>
            <td class="formFieldRepeater" style="width:12%;">Delete</td>
        </tr>
        <tr ID="NoRedemptionsRow" runat="server" visible="false">
            <td colspan="7" class="formValueRepeater">No social deal redemptions were found for this account.</td>
        </tr>
		<asp:Repeater ID="RedemptionsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "RedID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "BusName" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:10%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "PerName" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:10%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# WebConvert.ToString( Eval( "PinNumber" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:10%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# String.Format( "{0:C}",  Eval( "DelAmount" ))%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="BusView.aspx?ID=<%# Eval( "BusID") %>"><%# ( Eval( "RedTS" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "RedTS" ), DateTime.Now ).ToString( )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:12%;"><asp:LinkButton ID="DeleteRedemptionLinkButton" runat="server" CommandArgument='<%# Eval( "RedID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this redemption from this account?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    <br />
	<h3>Subscription</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:8%;">ID</td>
			<td class="formFieldRepeater" style="width:36%;">Product Name</td>
			<td class="formFieldRepeater" style="width:36%;">Payment Type</td>
            <td class="formFieldRepeater" style="width:20%;">Date Created</td>
        </tr>
        <tr ID="NoSubscriptionRow" runat="server" visible="false">
            <td colspan="5" class="formValueRepeater">No subscription was found for this account.</td>
        </tr>
		<asp:Repeater ID="SubscriptionRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td valign="top" class="formValueRepeater" style="width:8%;"><a href="AccSubView.aspx?ID=<%# Eval( "AccID") %>&SubID=<%# Eval( "SubID") %>"><%# WebConvert.ToString( Eval( "SubID" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="AccSubView.aspx?ID=<%# Eval( "AccID") %>&SubID=<%# Eval( "SubID") %>"><%# WebConvert.ToString( Eval( "PrdName" ), "&nbsp;" )%></a></td>
					<td valign="top" class="formValueRepeater" style="width:30%;"><a href="AccSubView.aspx?ID=<%# Eval( "AccID") %>&SubID=<%# Eval( "SubID") %>"><%# WebConvert.ToString( Eval( "PtyName" ), "&nbsp;" )%></a></td>
                    <td valign="top" class="formValueRepeater" style="width:20%;"><a href="AccSubView.aspx?ID=<%# Eval( "AccID") %>&SubID=<%# Eval( "SubID") %>"><%# ( Eval( "SubTSCreate" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "SubTSCreate" ), DateTime.Now ).ToString( )%></a></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
	<div class="commands">
		<asp:Button ID="AddSubscriptionButton" runat="server" Text="Add Subscription" CssClass="button" CausesValidation="false" />
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


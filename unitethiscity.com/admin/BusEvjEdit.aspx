<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusEvjEdit.aspx.cs" Inherits="admin_BusEvjEdit" %>
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
	<h3>Edit Recurring Event</h3>
    <br />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">Event Name *</td>
			<td class="formValue">
				<asp:TextBox ID="EvjNameTextBox" runat="server" MaxLength="50" Width="350"/>
				<asp:RequiredFieldValidator ID="EvjNameRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EvjNameTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Period *</td>
			<td class="formValue">
			    <asp:DropDownList ID="EjtIDDropDownList" runat="server" DataTextField="EjtName" DataValueField="EjtID" AppendDataBoundItems="true">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                </asp:DropDownList>
				    <asp:RequiredFieldValidator ID="EjtIDDropDownListRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EjtIDDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Every N Days (Daily) *</td>
			<td class="formValue">
				<asp:TextBox ID="EvjIntervalDailyTextBox" runat="server" MaxLength="5" Width="100"/>
				<asp:RequiredFieldValidator ID="EvjIntervalDailyRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EvjIntervalDailyTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Day of Week (Weekly) *</td>
			<td class="formValue">
			    <asp:DropDownList ID="EvjIntervalWeeklyDropDownList" runat="server">
                    <asp:ListItem Value="">Select...</asp:ListItem>
                    <asp:ListItem Value="0">Sunday</asp:ListItem>
                    <asp:ListItem Value="1">Monday</asp:ListItem>
                    <asp:ListItem Value="2">Tuesday</asp:ListItem>
                    <asp:ListItem Value="3">Wednesday</asp:ListItem>
                    <asp:ListItem Value="4">Thursday</asp:ListItem>
                    <asp:ListItem Value="5">Friday</asp:ListItem>
                    <asp:ListItem Value="6">Saturday</asp:ListItem>
                </asp:DropDownList>
				<asp:RequiredFieldValidator ID="EvjIntervalWeeklyRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EvjIntervalWeeklyDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Day of Month (Monthly) *</td>
			<td class="formValue">
				<asp:TextBox ID="EvjIntervalMonthlyTextBox" runat="server" MaxLength="5" Width="100"/>
				<asp:RequiredFieldValidator ID="EvjIntervalMonthlyRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EvjIntervalMonthlyTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Date Range for Valid Events *</td>
			<td class="formValue">
			    <dxe:ASPxDateEdit ID="ChkBeginDateEdit" runat="server" AllowNull="false">
				    <Paddings PaddingTop="0px" PaddingBottom="0px" />
				    <DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			    </dxe:ASPxDateEdit>
			    <asp:RequiredFieldValidator ID="ChkBeginDateEditRequired" runat="server" Display="Dynamic"
				    ControlToValidate="ChkBeginDateEdit" ErrorMessage="Required" />
                to
			    <dxe:ASPxDateEdit ID="ChkStopDateEdit" runat="server" AllowNull="false">
				    <Paddings PaddingTop="0px" PaddingBottom="0px" />
				    <DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
			    </dxe:ASPxDateEdit>
			    <asp:RequiredFieldValidator ID="ChkStopDateEditRequired" runat="server" Display="Dynamic"
				    ControlToValidate="ChkStopDateEdit" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Active *</td>
			<td class="formValue">
                <asp:DropDownList ID="EvjEnabledDropDownList" runat="server" >
                    <asp:ListItem Value="False">No</asp:ListItem>
                    <asp:ListItem Value="True">Yes</asp:ListItem>
                </asp:DropDownList>
				<asp:RequiredFieldValidator ID="EvjEnabledDropDownListRequired" runat="server" Display="Dynamic"
					ControlToValidate="EvjEnabledDropDownList" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField">Duration (Days) *</td>
			<td class="formValue">
				<asp:TextBox ID="EvjDurationTextBox" runat="server" MaxLength="5" Width="100"/>
				<asp:RequiredFieldValidator ID="EvjDurationRequired" runat="server" Display="Dynamic"
                    ControlToValidate="EvjDurationTextBox" ErrorMessage="Required" />
			</td>
		</tr>
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
				<asp:TextBox ID="EvjSummaryTextBox" runat="server" MaxLength="140" Width="99%" />	
				<asp:RequiredFieldValidator ID="EvjSummaryRequired" runat="server" Display="Dynamic"
					ControlToValidate="EvjSummaryTextBox" ErrorMessage="Required" />
			</td>
		</tr>
		<tr>
			<td class="formField" valign="top">Body *</td>
			<td class="formValue">
                <asp:TextBox ID="EvjBodyTextBox" runat="server" Width="99%" TextMode="MultiLine" Rows="10" />
				<asp:RequiredFieldValidator ID="EvjBodyTextBoxRequired" runat="server" Display="Dynamic"
					ControlToValidate="EvjBodyTextBox" ErrorMessage="Required" />
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
		<asp:Button ID="PurgeButton" runat="server" Text="Purge" CssClass="button" OnClientClick="javascript:return confirm( 'Are you sure you want to remove existing events for this definition?' );" />
        <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


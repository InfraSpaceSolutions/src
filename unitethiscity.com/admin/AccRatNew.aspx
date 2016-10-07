<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccRatNew.aspx.cs" Inherits="admin_AccRatNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="Add Favorite Location" />
	<div>
	    <table class="formTable" cellspacing="1">
		    <tr>
			    <td class="formField">ID</td>
			    <td class="formValue"><asp:Literal ID="AccIDLiteral" runat="server" /></td>
		    </tr>
		    <tr>
			    <td class="formField">Guid</td>
			    <td class="formValue"><asp:Literal ID="AccGuidLiteral" runat="server" /></td>
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
	    </table>

        <br />
	    <h3>Add Rating</h3>
	    <br />
	    <table class="formTable" cellspacing="1">
		    <tr>
			    <td class="formField">Location *</td>
			    <td class="formValue">
			        <asp:DropDownList ID="LocIDDropDownList" runat="server" DataTextField="LocName" DataValueField="LocID" AppendDataBoundItems="true">
                        <asp:ListItem Value="">Select...</asp:ListItem>
                    </asp:DropDownList>
				        <asp:RequiredFieldValidator ID="LocIDDropDownListRequired" runat="server" Display="Dynamic"
                        ControlToValidate="LocIDDropDownList" ErrorMessage="Required" />
			    </td>
		    </tr>
		    <tr>
			    <td class="formField">Rating *</td>
			    <td class="formValue">
			        <asp:DropDownList ID="RatRatingDropDownList" runat="server">
                        <asp:ListItem Value="">Select...</asp:ListItem>
                        <asp:ListItem Value="1">1</asp:ListItem>
                        <asp:ListItem Value="2">2</asp:ListItem>
                        <asp:ListItem Value="3">3</asp:ListItem>
                        <asp:ListItem Value="4">4</asp:ListItem>
                        <asp:ListItem Value="5">5</asp:ListItem>
                    </asp:DropDownList>
				        <asp:RequiredFieldValidator ID="RatRatingDropDownListRequired" runat="server" Display="Dynamic"
                        ControlToValidate="RatRatingDropDownList" ErrorMessage="Required" />
			    </td>
		    </tr>
        </table>
        <div class="commands">
            <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
            <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
        </div>
    </div>
</asp:Content>


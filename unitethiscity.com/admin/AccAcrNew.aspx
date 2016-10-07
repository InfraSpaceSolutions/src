<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccAcrNew.aspx.cs" Inherits="admin_AccAcrNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="Create Role" />
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
	    <h3>Create Role</h3>
	    <br />
	    <table class="formTable" cellspacing="1">
		    <tr>
			    <td class="formField">Role *</td>
			    <td class="formValue">
			        <asp:DropDownList ID="RolIDDropDownList" runat="server" DataTextField="RolName" DataValueField="RolID" AppendDataBoundItems="true">
                        <asp:ListItem Value="">Select...</asp:ListItem>
                    </asp:DropDownList>
				        <asp:RequiredFieldValidator ID="RolIDDropDownListRequired" runat="server" Display="Dynamic"
                        ControlToValidate="RolIDDropDownList" ErrorMessage="Required" />
			    </td>
		    </tr>
		    <tr>
			    <td class="formField">Business *</td>
			    <td class="formValue">
			        <asp:DropDownList ID="BusIDDropDownList" runat="server" DataTextField="BusName" DataValueField="BusID" AppendDataBoundItems="true">
                        <asp:ListItem Value="">Select...</asp:ListItem>
                    </asp:DropDownList>
				        <asp:RequiredFieldValidator ID="BusIDDropDownListRequired" runat="server" Display="Dynamic"
                        ControlToValidate="BusIDDropDownList" ErrorMessage="Required" />
			    </td>
		    </tr>
        </table>
        <div class="commands">
            <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
            <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
        </div>
    </div>
</asp:Content>


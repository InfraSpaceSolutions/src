<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccChkNew.aspx.cs" Inherits="admin_AccChkNew" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="View" />
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
        <br />
	    <h3>Add Loyalty Point</h3>
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
			    <td class="formField">Date *</td>
			    <td class="formValue">
				<dxe:ASPxDateEdit ID="ChkDateEdit" runat="server" AllowNull="false">
					<Paddings PaddingTop="0px" PaddingBottom="0px" />
					<DropDownButton Width="5px"  Image-Height="4px" Image-Width="7px" Image-Url="images/icons/LargeArrow.png"></DropDownButton>
				</dxe:ASPxDateEdit>
				<asp:RequiredFieldValidator ID="ChkDateEditRequired" runat="server" Display="Dynamic"
					ControlToValidate="ChkDateEdit" ErrorMessage="Required" />
			    </td>
		    </tr>
		    <tr>
			    <td class="formField">Time *</td>
			    <td class="formValue">
				<dxe:ASPxTimeEdit ID="ChkTimeEdit" EditFormat="Time" runat="server" AllowNull="false">
					<Paddings PaddingTop="0px" PaddingBottom="0px" />
                    <ButtonStyle Paddings-Padding="0px" />
				</dxe:ASPxTimeEdit>
				<asp:RequiredFieldValidator ID="ChkTimeEditRequired" runat="server" Display="Dynamic"
					ControlToValidate="ChkTimeEdit" ErrorMessage="Required" />
			    </td>
		    </tr>
        </table>
        <div class="commands">
            <asp:Button ID="SubmitButton" runat="server" Text="Create" CssClass="button" />
            <input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
        </div>
    </div>
</asp:Content>


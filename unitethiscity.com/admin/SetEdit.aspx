<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="SetEdit.aspx.cs" Inherits="admin_SetEdit" ValidateRequest="False" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>
<%@ Register TagPrefix="CE" Namespace="CuteEditor" Assembly="CuteEditor" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="SetMenu" runat="server" Name="Settings" Prefix="Set" Type="Edit" ShowAdd="false" />
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Literal ID="SetIDLiteral" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Name</td>
			<td class="formValue"><asp:Literal ID="SetNameLiteral" runat="server" /></td>
		</tr>
    </table>
	<br />
    <asp:Panel ID="SingleLinePanel" runat="server" Visible="false">
        <!-- Single Line Editor -->
	    <table class="formTable" cellspacing="1">
		    <tr>
			    <td class="formField">Value *</td>
			    <td class="formValue">
				    <asp:TextBox ID="SetValueSingleTextBox" runat="server" Columns="75"/>
				    <asp:RequiredFieldValidator ID="SetValueSingleRequired" runat="server" Display="Dynamic" ControlToValidate="SetValueSingleTextBox" ErrorMessage="Required" />
			    </td>
		    </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="MultipleLinePanel" runat="server" Visible="false">
        <!-- Multiple Line Editor -->
	    <table class="formTable" cellspacing="1">
		    <tr>
			    <td class="formField">Value *</td>
            </tr>
            <tr>
			    <td class="formValue">
				    <asp:TextBox ID="SetValueMultiTextBox" runat="server" TextMode="MultiLine" Rows="5" Width="95%"/>
				    <asp:RequiredFieldValidator ID="SetValueMultiRequired" runat="server" Display="Dynamic" ControlToValidate="SetValueMultiTextBox" ErrorMessage="Required" />
			    </td>
		    </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="HTMLPanel" runat="server" Visible="false">
        <!-- HTML Editor -->
	    <h3>HTML Body *</h3>
	    <CE:Editor id="SetHtmlEditor" Width="100%" Height="430px" runat="server" EnableStripIframeTags="false" /> 
    </asp:Panel>
	<div class="commands">
		<asp:Button ID="SubmitButton" runat="server" Text="Update" CssClass="button" />
		<input type="button" value="Cancel" class="button" onclick="javascript: history.go(-1);" />
	</div>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="TstView.aspx.cs" Inherits="admin_TstView" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="TstMenu" runat="server" Name="Testimonial Forms" Prefix="Tst" Type="View" ShowAdd="false"/>
	<table class="formTable" cellspacing="1">
		<tr>
			<td class="formField">ID</td>
			<td class="formValue"><asp:Label ID="TstIDLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">First Name</td>
			<td class="formValue"><asp:Label ID="TstFNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Last Name</td>
			<td class="formValue"><asp:Label ID="TstLNameLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Email</td>
			<td class="formValue"><asp:HyperLink ID="TstEMailHyperLink" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">UTC Member?</td>
			<td class="formValue"><asp:Label ID="TstMemberLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">UTC Business Partner?</td>
			<td class="formValue"><asp:Label ID="TstPartnerLabel" runat="server" /></td>
		</tr>
		<tr>
			<td class="formField">Timestamp</td>
			<td class="formValue"><asp:Label ID="TstTimestampLabel" runat="server" /></td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button" OnClientClick="javascript: return confirm('Are you sure you want to delete this Testimonial Form?');" />
	</div>

	<h3>Testimonial</h3>
	<hr />
	<div class="PublicPreview">
		<asp:Literal ID="TstTestimonialLiteral" runat="server"></asp:Literal> 
	</div>
	<hr />
</asp:Content>


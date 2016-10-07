<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusGalleryItems.aspx.cs" Inherits="admin_BusGalleryItems" %>
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
		<tr>
			<td class="formField">Add Image</td>
			<td class="formValue">
                <asp:FileUpload ID="GalleryFileUpload" runat="server" />
                <asp:Button ID="UploadButton" runat="server" Text="Upload" CssClass="button" />
			</td>
		</tr>
	</table>
	<div class="commands">
		<asp:Button ID="SequenceButton" runat="server" Text="Manage Sequence" CssClass="button" CausesValidation="false" />
	</div>

	<h3>Gallery Images</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:10%;">Image</td>
			<td class="formFieldRepeater" style="width:80%; text-align:center;">Sequence</td>
            <td class="formFieldRepeater" style="width:10%;">Delete</td>
        </tr>
        <tr ID="NoItemsRow" runat="server" visible="false">
            <td colspan="3" class="formValueRepeater">No gallery images were found for this business.</td>
        </tr>
		<asp:Repeater ID="GalleryItemsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater">
                        <a href="BusGalleryItemEdit.aspx?ID=<%# Eval( "GalID") %>">
                            <img src="/BusinessGallery/<%# Eval( "GalGuid" ) %>.png?t=<%= ImageTicks %>" alt="Edit Image" border="0" style="width:200px; height:auto;" />
                        </a>
                    </td>
					<td class="formValueRepeater" style="text-align:center;"><a href="BusGalleryItemEdit.aspx?ID=<%# Eval( "GalID") %>"><%# WebConvert.ToString( Eval( "GalSeq" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater"><asp:LinkButton ID="DeleteGalleryItemLinkButton" runat="server" CommandArgument='<%# Eval( "GalID" ) %>' Text="Delete" Font-Bold="true" CausesValidation="false" OnClientClick="javascript: return confirm('Are you sure you want to remove this gallery image?');" /></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    
</asp:Content>


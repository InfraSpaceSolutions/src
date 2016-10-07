<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusGalleryItemSequence.aspx.cs" Inherits="admin_BusGalleryItemSequence" %>
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
	<div class="commands">
        <asp:Button ID="DoneButton" runat="server" Text="Done" CssClass="button" CausesValidation="false" />
	</div>

	<h3>Sequence Gallery Images</h3>
	<br />
	<table class="formTableRepeater" cellspacing="1" width="100%">
        <tr>
            <td class="formFieldRepeater" style="width:10%;">Image</td>
			<td class="formFieldRepeater" style="width:50%; text-align:center;">Sequence</td>
			<td class="formFieldRepeater" style="width:40%; text-align:center;">Manage Sequence</td>
        </tr>
        <tr ID="NoItemsRow" runat="server" visible="false">
            <td colspan="3" class="formValueRepeater">No gallery images were found for this business.</td>
        </tr>
		<asp:Repeater ID="GalleryItemsRepeater" runat="server" >
			<ItemTemplate>
                <tr>
                    <td class="formValueRepeater">
                        <a href="BusGalleryItemEdit.aspx?ID=<%# Eval( "GalID") %>">
                            <img src="/BusinessGallery/<%# Eval( "GalGuid" ) %>.png" alt="Edit Image" border="0" style="width:200px; height:auto;" />
                        </a>
                    </td>
					<td class="formValueRepeater" style="text-align:center;"><a href="BusGalleryItemEdit.aspx?ID=<%# Eval( "GalID") %>"><%# WebConvert.ToString( Eval( "GalSeq" ), "&nbsp;" )%></a></td>
                    <td class="formValueRepeater" align="center"><asp:LinkButton ID="UpSequenceLinkButton" runat="server" Text="Up" style="font-weight:bold;" /> - <asp:LinkButton ID="DownSequenceLinkButton" runat="server" Text="Down" style="font-weight:bold;"/></td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
    </table>
    
</asp:Content>


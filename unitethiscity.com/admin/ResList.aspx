<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ResList.aspx.cs" Inherits="admin_ResList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
    <div style="float: right;">
        <br />
        <div style="padding-bottom:5px;">
            <b>File Upload: </b>
            <span class="note">10 MB maximum file size.</span>
        </div>
        <div>
            <asp:FileUpload ID="ResourceFileUpload" runat="server" />
            <asp:Button ID="UploadButton" runat="server" Text="Upload" CssClass="button" />
        </div>
        <br />
    </div>
	<div id="menu">
		<a href="ResList.aspx"><img src="images/Resources48.png" width="48" height="48" border="0" title="View Resource List" alt="Resources" style="float:left; margin-right:5px;" /></a>
		<h2>Resources</h2>
		<h3>Browse</h3>
	</div>
    <br style="clear:right;" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="MessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<asp:Panel ID="ErrorPanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="ErrorLabel" runat="server" CssClass="Error" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" AutoGenerateColumns="False" runat="server">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="Download" FieldName="DownloadUrl" VisibleIndex="0" Width="10%" CellStyle-HorizontalAlign="Center" Settings-AllowAutoFilter="False" Settings-AllowSort="False">
				<DataItemTemplate>	
					<a href="<%# Eval( "DownloadUrl" ) %>" title="Download (New Window)" target="_blank"><img src="/admin/images/icons/iconDownloads24.png" alt="Download" /></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Filename" FieldName="Filename" VisibleIndex="1" Width="30%">
				<DataItemTemplate>	
					<%# WebConvert.ToString( Eval( "Filename" ), "&nbsp;" )%>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Size" FieldName="Length" VisibleIndex="2" Width="10%">
				<DataItemTemplate>
					<%# WebConvert.ToString( Eval( "FileSize" ), "&nbsp;" )%>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Created" FieldName="CreationTime" VisibleIndex="3" Width="20%" CellStyle-HorizontalAlign="Right">
				<DataItemTemplate>	
					<%# WebConvert.ToString(Eval( "CreationTime" ),"")%>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Modified" FieldName="LastWriteTime" VisibleIndex="4" Width="20%" CellStyle-HorizontalAlign="Right">
				<DataItemTemplate>
					<%# WebConvert.ToString( Eval( "LastWriteTime" ), "&nbsp;" )%>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Delete" FieldName="DeleteUrl" VisibleIndex="5" Width="10%" CellStyle-HorizontalAlign="Center" Settings-AllowAutoFilter="False" Settings-AllowSort="False">
				<DataItemTemplate>	
					<a href="javascript: if(confirm('Are you sure you want to permanently delete this file?')) { window.location='<%# Eval( "DeleteUrl" ) %>'; }" title="Delete" target="_blank"><img src="/admin/images/icons/iconDelete24.png" alt="Delete" /></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


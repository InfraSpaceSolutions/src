<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PagList.aspx.cs" Inherits="admin_PagList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PagMenu" runat="server" Name="Pages" Prefix="Pag" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success"/>
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="PagID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="PagID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Name" FieldName="PagName" VisibleIndex="1" Width="40%">
				<DataItemTemplate>	
					<a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Filename" FieldName="PagFilename" VisibleIndex="2" Width="52%">
				<DataItemTemplate>	
					<a href="PagView.aspx?ID=<%# Eval( "PagID" ) %>"><%# WebConvert.ToString( Eval( "PagFilename" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true" />
	</dxwgv:ASPxGridView>
</asp:Content>
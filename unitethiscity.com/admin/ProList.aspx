<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ProList.aspx.cs" Inherits="admin_ProList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="ProMenu" runat="server" Name="Promotions" Prefix="Pro" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="ProID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="ProID" VisibleIndex="0" Width="10%">
				<DataItemTemplate>	
					<a href="ProView.aspx?ID=<%# Eval( "ProID" ) %>"><%# WebConvert.ToString( Eval( "ProID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Name" FieldName="ProName" VisibleIndex="1" Width="25%">
				<DataItemTemplate>	
					<a href="ProView.aspx?ID=<%# Eval( "ProID" ) %>"><%# WebConvert.ToString( Eval( "ProName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Title" FieldName="ProTitle" VisibleIndex="2" Width="50%">
				<DataItemTemplate>	
					<a href="ProView.aspx?ID=<%# Eval( "ProID" ) %>"><%# WebConvert.ToString( Eval( "ProTitle" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Enabled" FieldName="ProEnabled" VisibleIndex="3" Width="15%">
				<DataItemTemplate>	
					<a href="ProView.aspx?ID=<%# Eval( "ProID" ) %>"><%# WebConvert.ToString( Eval( "ProEnabled" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


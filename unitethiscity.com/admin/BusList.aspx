<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BusList.aspx.cs" Inherits="admin_BusList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="BusMenu" runat="server" Name="Businesses" Prefix="Bus" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="BusID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="BusID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="BusView.aspx?ID=<%# Eval( "BusID" ) %>"><%# WebConvert.ToString( Eval( "BusID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Business Name" FieldName="BusName" VisibleIndex="1" Width="40%">
				<DataItemTemplate>	
					<a href="BusView.aspx?ID=<%# Eval( "BusID" ) %>"><%# WebConvert.ToString( Eval( "BusName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Sales Rep" FieldName="BusRepEMail" VisibleIndex="2" Width="40%">
				<DataItemTemplate>	
					<a href="BusView.aspx?ID=<%# Eval( "BusID" ) %>"><%# WebConvert.ToString( Eval( "BusRepEMail" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Enabled" FieldName="BusEnabled" VisibleIndex="3" Width="12%">
				<DataItemTemplate>	
					<a href="BusView.aspx?ID=<%# Eval( "BusID" ) %>"><%# WebConvert.ToBoolean( Eval( "BusEnabled" ), false ) ? "Yes" : "No"%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


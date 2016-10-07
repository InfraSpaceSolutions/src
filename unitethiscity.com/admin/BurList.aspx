<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="BurList.aspx.cs" Inherits="admin_BurList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="BurMenu" runat="server" Name="Business Registration Forms" Prefix="Bur" Type="Browse" ShowAdd="false" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success"/>
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="BurID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="BurID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="BurView.aspx?ID=<%# Eval( "BurID" ) %>"><%# WebConvert.ToString( Eval( "BurID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="First Name" FieldName="BurFName" VisibleIndex="1" Width="20%">
				<DataItemTemplate>	
					<a href="BurView.aspx?ID=<%# Eval( "BurID" ) %>"><%# WebConvert.ToString( Eval( "BurFName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Last Name" FieldName="BurLName" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="BurView.aspx?ID=<%# Eval( "BurID" ) %>"><%# WebConvert.ToString( Eval( "BurLName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Email" FieldName="BurEMail" VisibleIndex="3" Width="32%">
				<DataItemTemplate>	
					<a href="BurView.aspx?ID=<%# Eval( "BurID" ) %>"><%# WebConvert.ToString( Eval( "BurEMail" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Date Created" FieldName="BurTimestamp" VisibleIndex="4" Width="20%">
				<DataItemTemplate>	
					<a href="BurView.aspx?ID=<%# Eval( "BurID" ) %>"><%# ( Eval( "BurTimestamp" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "BurTimestamp" ), DateTime.Now ).ToString( )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


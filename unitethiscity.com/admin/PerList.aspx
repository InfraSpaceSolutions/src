<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PerList.aspx.cs" Inherits="admin_PerList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PerMenu" runat="server" Name="Periods" Prefix="Per" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="PerID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="PerID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="PerView.aspx?ID=<%# Eval( "PerID" ) %>"><%# WebConvert.ToString( Eval( "PerID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Name" FieldName="PerName" VisibleIndex="1" Width="62%">
				<DataItemTemplate>	
					<a href="PerView.aspx?ID=<%# Eval( "PerID" ) %>"><%# WebConvert.ToString( Eval( "PerName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Start Date" FieldName="PerStartDate" VisibleIndex="2" Width="15%">
				<DataItemTemplate>	
					<a href="PerView.aspx?ID=<%# Eval( "PerID" ) %>"><%# ( Eval( "PerStartDate" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "PerStartDate" ), DateTime.Now ).ToShortDateString( )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="End Date" FieldName="PerEndDate" VisibleIndex="3" Width="15%">
				<DataItemTemplate>	
					<a href="PerView.aspx?ID=<%# Eval( "PerID" ) %>"><%# ( Eval( "PerEndDate" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "PerEndDate" ), DateTime.Now ).ToShortDateString( )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


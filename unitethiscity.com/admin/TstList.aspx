<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="TstList.aspx.cs" Inherits="admin_TstList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="TstMenu" runat="server" Name="Testimonial Forms" Prefix="Tst" Type="Browse" ShowAdd="false" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success"/>
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="TstID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="TstID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="TstView.aspx?ID=<%# Eval( "TstID" ) %>"><%# WebConvert.ToString( Eval( "TstID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="First Name" FieldName="TstFName" VisibleIndex="1" Width="20%">
				<DataItemTemplate>	
					<a href="TstView.aspx?ID=<%# Eval( "TstID" ) %>"><%# WebConvert.ToString( Eval( "TstFName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Last Name" FieldName="TstLName" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="TstView.aspx?ID=<%# Eval( "TstID" ) %>"><%# WebConvert.ToString( Eval( "TstLName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Email" FieldName="TstEMail" VisibleIndex="3" Width="32%">
				<DataItemTemplate>	
					<a href="TstView.aspx?ID=<%# Eval( "TstID" ) %>"><%# WebConvert.ToString( Eval( "TstEMail" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Date Created" FieldName="TstTimestamp" VisibleIndex="4" Width="20%">
				<DataItemTemplate>	
					<a href="TstView.aspx?ID=<%# Eval( "TstID" ) %>"><%# ( Eval( "TstTimestamp" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "TstTimestamp" ), DateTime.Now ).ToString( )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


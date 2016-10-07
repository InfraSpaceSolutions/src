<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="CatList.aspx.cs" Inherits="admin_CatList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="CatMenu" runat="server" Name="Categories" Prefix="Cat" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="CatID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="CatID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="CatView.aspx?ID=<%# Eval( "CatID" ) %>"><%# WebConvert.ToString( Eval( "CatID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Name" FieldName="CatName" VisibleIndex="1" Width="46%">
				<DataItemTemplate>	
					<a href="CatView.aspx?ID=<%# Eval( "CatID" ) %>"><%# WebConvert.ToString( Eval( "CatName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Parent Name" FieldName="CatParentName" VisibleIndex="2" Width="46%">
				<DataItemTemplate>	
					<a href="CatView.aspx?ID=<%# Eval( "CatID" ) %>"><%# WebConvert.ToString( Eval( "CatParentName" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


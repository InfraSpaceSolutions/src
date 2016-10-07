<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="PrpList.aspx.cs" Inherits="admin_PrpList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="PrpMenu" runat="server" Name="Properties" Prefix="Prp" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="PrpID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="PrpID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="PrpView.aspx?ID=<%# Eval( "PrpID" ) %>"><%# WebConvert.ToString( Eval( "PrpID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Name" FieldName="PrpName" VisibleIndex="1" Width="92%">
				<DataItemTemplate>	
					<a href="PrpView.aspx?ID=<%# Eval( "PrpID" ) %>"><%# WebConvert.ToString( Eval( "PrpName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


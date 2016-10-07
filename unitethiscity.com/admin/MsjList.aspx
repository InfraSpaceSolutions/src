<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="MsjList.aspx.cs" Inherits="admin_MsjList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="MsjMenu" runat="server" Name="Message Jobs" Prefix="Msj" Type="Browse" />
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="MsjID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="MsjID" VisibleIndex="0" Width="10%">
				<DataItemTemplate>	
					<a href="MsjView.aspx?ID=<%# Eval( "MsjID" ) %>"><%# WebConvert.ToString( Eval( "MsjID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="State" FieldName="MjsName" VisibleIndex="1" Width="10%">
				<DataItemTemplate>	
					<a href="MsjView.aspx?ID=<%# Eval( "MsjID" ) %>"><%# WebConvert.ToString( Eval( "MjsName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="To" FieldName="RolName" VisibleIndex="2" Width="10%">
				<DataItemTemplate>	
					<a href="MsjView.aspx?ID=<%# Eval( "MsjID" ) %>"><%# WebConvert.ToString( Eval( "RolName" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="From" FieldName="MsgFromName" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="MsjView.aspx?ID=<%# Eval( "MsjID" ) %>"><%# WebConvert.ToString( Eval( "MsgFromName" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Summary" FieldName="MsgSummary" VisibleIndex="2" Width="30%">
				<DataItemTemplate>	
					<a href="MsjView.aspx?ID=<%# Eval( "MsjID" ) %>"><%# WebConvert.ToString( Eval( "MsgSummary" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Send Time" FieldName="MsjSendTS" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="MsjView.aspx?ID=<%# Eval( "MsjID" ) %>"><%# Eval( "MsjSendTS" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


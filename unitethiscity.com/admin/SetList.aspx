<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="SetList.aspx.cs" Inherits="admin_SetList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="SetMenu" runat="server" Name="Settings" Prefix="Set" Type="Browse" ShowAdd="false" />
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="AdmID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="SetID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="SetEdit.aspx?ID=<%# Eval( "SetID" ) %>"><%# WebConvert.ToString( Eval( "SetID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Name" FieldName="SetName" VisibleIndex="1" Width="20%">
				<DataItemTemplate>	
					<a href="SetEdit.aspx?ID=<%# Eval( "SetID" ) %>"><%# WebConvert.ToString( Eval( "SetName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Value" FieldName="SetValue" VisibleIndex="2" Width="72%">
				<DataItemTemplate>	
					<a href="SetEdit.aspx?ID=<%# Eval( "SetID" ) %>"><%# HttpUtility.HtmlEncode(WebConvert.Truncate(WebConvert.ToString( Eval( "SetValue" ), "&nbsp;" ), 80))%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


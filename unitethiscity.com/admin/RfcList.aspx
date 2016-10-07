<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="RfcList.aspx.cs" Inherits="admin_RfcList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="RfcMenu" runat="server" Name="Referral Codes" Prefix="Rfc" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="RfcID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="RfcID" VisibleIndex="0" Width="10%">
				<DataItemTemplate>	
					<a href="RfcView.aspx?ID=<%# Eval( "RfcID" ) %>"><%# WebConvert.ToString( Eval( "RfcID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Code" FieldName="RfcCode" VisibleIndex="1" Width="15%">
				<DataItemTemplate>	
					<a href="RfcView.aspx?ID=<%# Eval( "RfcID" ) %>"><%# WebConvert.ToString( Eval( "RfcCode" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Owner" FieldName="AccEMail" VisibleIndex="2" Width="25%">
				<DataItemTemplate>	
					<a href="RfcView.aspx?ID=<%# Eval( "RfcID" ) %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Assigned Business" FieldName="BusName" VisibleIndex="3" Width="30%">
				<DataItemTemplate>	
					<a href="RfcView.aspx?ID=<%# Eval( "RfcID" ) %>"><%# WebConvert.ToString( Eval( "BusName" ), "N/A" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Promotion" FieldName="ProName" VisibleIndex="4" Width="20%">
				<DataItemTemplate>	
					<a href="RfcView.aspx?ID=<%# Eval( "RfcID" ) %>"><%# WebConvert.ToString( Eval( "ProName" ), "None" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


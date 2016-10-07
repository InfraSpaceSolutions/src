<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ChaList.aspx.cs" Inherits="admin_ChaList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="ChaMenu" runat="server" Name="Charity Registration Forms" Prefix="Cha" Type="Browse" ShowAdd="false" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success"/>
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="ChaID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="ChaID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="ChaView.aspx?ID=<%# Eval( "ChaID" ) %>"><%# WebConvert.ToString( Eval( "ChaID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="First Name" FieldName="ChaFName" VisibleIndex="1" Width="20%">
				<DataItemTemplate>	
					<a href="ChaView.aspx?ID=<%# Eval( "ChaID" ) %>"><%# WebConvert.ToString( Eval( "ChaFName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Last Name" FieldName="ChaLName" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="ChaView.aspx?ID=<%# Eval( "ChaID" ) %>"><%# WebConvert.ToString( Eval( "ChaLName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Email" FieldName="ChaEMail" VisibleIndex="3" Width="32%">
				<DataItemTemplate>	
					<a href="ChaView.aspx?ID=<%# Eval( "ChaID" ) %>"><%# WebConvert.ToString( Eval( "ChaEMail" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Date Created" FieldName="ChaTimestamp" VisibleIndex="4" Width="20%">
				<DataItemTemplate>	
					<a href="ChaView.aspx?ID=<%# Eval( "ChaID" ) %>"><%# ( Eval( "ChaTimestamp" ) == null ) ? "N/A" : WebConvert.ToDateTime( Eval( "ChaTimestamp" ), DateTime.Now ).ToString( )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="AccList.aspx.cs" Inherits="admin_AccList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="AccMenu" runat="server" Name="Accounts" Prefix="Acc" Type="Browse" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success" />
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="AccID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="AccID" VisibleIndex="0" Width="5%">
				<DataItemTemplate>	
					<a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="First Name" FieldName="AccFName" VisibleIndex="1" Width="20%">
				<DataItemTemplate>	
					<a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccFName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Last Name" FieldName="AccLName" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccLName" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Phone" FieldName="AccPhone" VisibleIndex="3" Width="15%">
				<DataItemTemplate>	
					<a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# Phone.Format( WebConvert.ToString(Eval( "AccPhone" ),""))%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Email Address" FieldName="AccEMail" VisibleIndex="4" Width="30%">
				<DataItemTemplate>	
					<a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToString( Eval( "AccEMail" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Enabled" FieldName="AccEnabled" VisibleIndex="5" Width="10%">
				<DataItemTemplate>	
					<a href="AccView.aspx?ID=<%# Eval( "AccID" ) %>"><%# WebConvert.ToBoolean( Eval( "AccEnabled" ), false ) ? "Yes" : "No"%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


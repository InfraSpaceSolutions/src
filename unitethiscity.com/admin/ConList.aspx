<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="ConList.aspx.cs" Inherits="admin_ConList" %>
<%@ MasterType VirtualPath="~/admin/SiteAdminRestricted.master" %>
<%@ Register Src="~/admin/Menu.ascx" TagPrefix="menu" TagName="Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<menu:Admin ID="ConMenu" runat="server" Name="Contact Us Forms" Prefix="Con" Type="Browse" ShowAdd="false" />
	<asp:Panel ID="MessagePanel" CssClass="statusMessage" runat="server" Visible="false">
		<asp:Label ID="DeleteMessageLabel" runat="server" CssClass="Success"/>
	</asp:Panel>
	<dxwgv:ASPxGridView ID="ListGridView" runat="server" KeyFieldName="ConID" AutoGenerateColumns="False">
		<Columns>
			<dxwgv:GridViewDataTextColumn Caption="ID" FieldName="ConID" VisibleIndex="0" Width="8%">
				<DataItemTemplate>	
					<a href="ConView.aspx?ID=<%# Eval( "ConID" ) %>"><%# WebConvert.ToString( Eval( "ConID" ), "&nbsp;" )%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="First Name" FieldName="ConFName" VisibleIndex="1" Width="20%">
				<DataItemTemplate>	
					<a href="ConView.aspx?ID=<%# Eval( "ConID" ) %>"><%# WebConvert.ToString(Eval("ConFName"), "&nbsp;")%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Last Name" FieldName="ConLName" VisibleIndex="2" Width="20%">
				<DataItemTemplate>	
					<a href="ConView.aspx?ID=<%# Eval( "ConID" ) %>"><%# WebConvert.ToString(Eval("ConLName"), "&nbsp;")%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
            <dxwgv:GridViewDataTextColumn Caption="Email" FieldName="ConEMail" VisibleIndex="3" Width="32%">
				<DataItemTemplate>	
					<a href="ConView.aspx?ID=<%# Eval( "ConID" ) %>"><%# WebConvert.ToString(Eval("ConEMail"), "&nbsp;")%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
			<dxwgv:GridViewDataTextColumn Caption="Date Created" FieldName="ConTimestamp" VisibleIndex="4" Width="20%">
				<DataItemTemplate>	
					<a href="ConView.aspx?ID=<%# Eval( "ConID" ) %>"><%# (Eval("ConTimestamp") == null) ? "N/A" : WebConvert.ToDateTime(Eval("ConTimestamp"), DateTime.Now).ToString()%></a>
				</DataItemTemplate>
			</dxwgv:GridViewDataTextColumn>
		</Columns>
		<Settings ShowFilterRow="true" ShowHeaderFilterButton="true"/>
	</dxwgv:ASPxGridView>
</asp:Content>


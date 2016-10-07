<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdmin.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="admin_Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<div align="center">
		<div id="loginForm">
			<table cellpadding="0" cellspacing="0" border="0" class="loginTable">
				<tr>
					<td width="100">Email Address:</td>
					<td width="244">
						<asp:TextBox ID="AccEMailTextBox" runat="server" Width="244" MaxLength="128" />
						<asp:RequiredFieldValidator ID="AccEMailRequired" runat="server" Display="Dynamic"
							ControlToValidate="AccEMailTextBox" ErrorMessage="<br />Required" />
					</td>
				</tr>
				<tr>
					<td>Password:</td>
					<td>
						<asp:TextBox ID="AccPasswordTextBox" runat="server" Width="244" TextMode="Password" MaxLength="50" />
						<asp:RequiredFieldValidator ID="AccPasswordRequired" runat="server" Display="Dynamic"
							ControlToValidate="AccPasswordTextBox" ErrorMessage="<br />Required" />
					</td>
				</tr>
			</table>
			<div class="loginButton">
				<asp:Button ID="OkButton" Text="Login" CssClass="button" runat="server" />
			</div>
		</div>
    </div>
</asp:Content>
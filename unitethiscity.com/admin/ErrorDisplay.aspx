<%@ Page Title="" Language="C#" MasterPageFile="~/admin/SiteAdmin.master" AutoEventWireup="true" CodeFile="ErrorDisplay.aspx.cs" Inherits="admin_ErrorDisplay" %>


<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">	
    <div align="center" style="padding:40px 0px 40px 0px;">
		<table cellpadding="0" cellspacing="0" class="ErrorTable">
			<tr>
				<td class="ErrorHeader">
					<asp:Label ID="ErrHeaderLabel" CssClass="ErrorHeaderLabel" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="ErrorBody">
					<div>The following error has occurred when attempting to process the web request:</div>
					<br />
					<div style="text-align:center;">
						<asp:Label ID="ErrMessage" CssClass="ErrorMessage" runat="server" />
					</div>
					<br />
					<div>Please attempt to correct this problem and retry the request.  If problems persist, 
					please contact technical support for assistance.</div>
					<br />
					<div><a href="javascript:history.go(-1);">&lt;&lt; Return to the previous page</a></div>
				</td>
			</tr>
		</table>
    </div>
</asp:Content>
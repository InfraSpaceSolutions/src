<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="admin_Menu" %>

<div id="menu">
<%
	if ( ShowMenu )
	{
%>
	<div id="menuOperations">
	<%
		if ( ShowBrowse )
		{
	%>
		<a href="<%= Prefix %>List.aspx"><img src="images/icons/iconBrowse24.png" width="24" height="24" border="0" alt="Browse All" title="Browse All" /></a>&nbsp;
	<%
		}
        if ( ShowAdd )
        {
	%>
		<a href="<%= Prefix %>New.aspx"><img src="images/icons/iconAdd24.png" width="24" height="24" border="0" alt="Create New" title="Create New" /></a>
	<%
		}
    %>
	</div>
<%
	}
%>
	<div>
		<a href="<%= Prefix %>List.aspx"><img src="images/<%= Name.Replace( " ", "" ) %>48.png" width="48" height="48" border="0" title="View <%= Name %> List" alt="<%= Name %>" style="float:left; margin-right:5px;" /></a>
		<h2><%= Name%></h2>
		<h3><%= Type%></h3>
	</div>
</div>
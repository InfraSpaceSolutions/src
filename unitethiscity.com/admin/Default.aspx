<%@ Page Language="C#" MasterPageFile="~/admin/SiteAdminRestricted.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BodyPlaceHolder" Runat="Server">
	<h1>UniteThisCity.com Administration Website</h1>
	
	<p>Welcome to the administration website for 
	<a href="<%= SiteSettings.GetValue("RootURL") %>" target="_blank"><%= SiteSettings.GetValue("RootURL") %></a>. This site is intended to support 
	the ongoing configuration, maintenance and support of the UniteThisCity.com website. It is intended for use only by authorized personnel and 
	website support providers.</p>
	<hr />
	<div style="margin-left:10px;">
        <h3>Account Management</h3>
		<div class="contentSection">
            <div><a href="AccList.aspx"><img src="images/Accounts72.png" alt="" width="72" height="72" /><br/>Accounts</a></div>
			<br style="clear:left;" />
		</div>
		<hr style="clear:left;" />
    </div>
	<div style="margin-left:10px;">
        <h3>Content Management</h3>
		<div class="contentSection">
			<div><a href="PagList.aspx"><img src="images/Pages72.png" alt="" width="72" height="72" /><br/>Pages</a></div>
			<div><a href="ResList.aspx"><img src="images/Resources72.png" alt="" width="72" height="72" /><br/>Resources</a></div>
			<br style="clear:left;" />
		</div>
		<hr style="clear:left;" />
    </div>
	<div style="margin-left:10px;">
        <h3>Business Management</h3>
		<div class="contentSection">
			<div><a href="BusList.aspx"><img src="images/Businesses72.png" alt="" width="72" height="72" /><br/>Businesses</a></div>
			<div><a href="CatList.aspx"><img src="images/Categories72.png" alt="" width="72" height="72" /><br/>Categories</a></div>
			<div><a href="PrpList.aspx"><img src="images/Properties72.png" alt="" width="72" height="72" /><br/>Properties</a></div>
			<div><a href="CitList.aspx"><img src="images/Cities72.png" alt="" width="72" height="72" /><br/>Cities</a></div>
			<div><a href="PerList.aspx"><img src="images/Periods72.png" alt="" width="72" height="72" /><br/>Periods</a></div>
			<div><a href="RfcList.aspx"><img src="images/ReferralCodes72.png" alt="" width="72" height="72" /><br/>Referral Codes</a></div>
			<div><a href="ProList.aspx"><img src="images/Promotions72.png" alt="" width="72" height="72" /><br/>Promotions</a></div>
			<div><a href="MsjList.aspx"><img src="images/MessageJobs72.png" alt="" width="72" height="72" /><br/>Messsage Jobs</a></div>
			<br style="clear:left;" />
		</div>
		<hr style="clear:left;" />
    </div>
	<div style="margin-left:10px;">
        <h3>Submission Forms</h3>
		<div class="contentSection">
			<div><a href="BurList.aspx"><img src="images/BusinessRegistrationForms72.png" alt="" width="72" height="72" /><br/>Business Registrations</a></div>
			<div><a href="ChaList.aspx"><img src="images/CharityRegistrationForms72.png" alt="" width="72" height="72" /><br/>Charity Registrations</a></div>
			<div><a href="ConList.aspx"><img src="images/ContactUsForms72.png" alt="" width="72" height="72" /><br/>Contact Us</a></div>
			<div><a href="TstList.aspx"><img src="images/TestimonialForms72.png" alt="" width="72" height="72" /><br/>Testimonials</a></div>
			<br style="clear:left;" />
		</div>
		<hr style="clear:left;" />
    </div>
    <div style="margin-left:10px;">
        <h3>Settings</h3>
		<div class="contentSection">
			<div><a href="SetList.aspx"><img src="images/Settings72.png" alt="" width="72" height="72" /><br/>Site Settings</a></div>
			<br style="clear:left;" />
		</div>
    </div>
	<hr style="clear:left;" />
    <br />

</asp:Content>
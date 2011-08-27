<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewTrip.aspx.cs" MasterPageFile="~/MasterPage.master" Inherits="ViewTrip" %>

<asp:Content ContentPlaceHolderID="content" runat="server">

<!-- Begin server form -->

<form id="TripLockForm" runat="server">

<!-- Begin trip -->
<div class="content">

<div class="content_header" style="position: relative;">

<asp:Label ID="TripLockIcon" runat="server"/>
<asp:Label ID="TripEditIcon" CssClass="uneditable" Visible="false" runat="server"/>
<span style="font-size: 1.8em; margin-left: 15px; vertical-align: top;"><asp:Literal ID="TripName" runat="server"></asp:Literal></span>

</div>

<asp:ScriptManager runat="server" />
<asp:UpdatePanel runat="server" ID="Segment">
<Triggers>
 <asp:AsyncPostBackTrigger ControlID="AddNewSegmentButton" EventName="Click"/>
</Triggers>
<ContentTemplate>

<div class="content_body">

<div class="segment_menu">

<asp:Repeater ID="Segment_Menu" runat="server">
 <HeaderTemplate></HeaderTemplate>
 <ItemTemplate>  
   <div class="segment_menu_item" name="<%# Container.ItemIndex %>">
    <%# DataBinder.Eval(Container.DataItem, "Name") %>
   </div>
 </ItemTemplate>
 <FooterTemplate>
 </FooterTemplate>
</asp:Repeater>
<span id="TripNewSegmentButton" rel="#TripNewSegmentContainer"><div class="segment_menu_special" id="segment_add_new">Add New Segment</div></span>
</div>

<div class="segment_content">
<asp:Repeater ID="Segment_Content" runat="server">
 <HeaderTemplate></HeaderTemplate>
 <ItemTemplate>
   <div class="segment_content_item" name="<%# Container.ItemIndex %>">
    <i><%# DataBinder.Eval(Container.DataItem, "Description") %></i>
   </div>
 </ItemTemplate>
 <FooterTemplate>
 </FooterTemplate>
</asp:Repeater>

</div>

<div class="content_footer">
Activity controls here
</div>

</ContentTemplate>
</asp:UpdatePanel>

</div>

<!-- End trip -->

<div class="overlay_form" id="TripLockFormContainer">
<h3>Protect Your Trip</h3><br />
<asp:TextBox runat="server" ID="TripLockPassword" ValidationGroup="TripLockForm"></asp:TextBox>
<asp:Button runat="server" ID="TripLockButton" Text="Make Private" onclick="btn_set_access" ValidationGroup="TripLockForm"/>
<asp:Button runat="server" ID="TripViewOnlyButton" Text="Make View Only" onclick="btn_set_access" ValidationGroup="TripLockForm"/>
<br /><asp:RequiredFieldValidator runat="server" ControlToValidate="TripLockPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Password is required." ValidationGroup="TripLockForm"></asp:RequiredFieldValidator>
<div class="overlay_content">
Want to protect your trip? Lock it here and only you can see and edit it.
<br /><br />
Want to share with others? Make it view only so that others can see it,
<br />but only you can edit it.
</div>
</div>

<div class="overlay_form" id="TripUnlockFormContainer">
<h3>Unprotect Your Trip</h3><br />
<asp:TextBox runat="server" ID="TripUnlockPassword" ValidationGroup="TripUnlockForm"></asp:TextBox>
<asp:Button runat="server" ID="TripUnlockButton" Text="Unlock" onclick="btn_set_access" ValidationGroup="TripUnlockForm"/>
<br /><asp:RequiredFieldValidator runat="server" ControlToValidate="TripUnlockPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Password is required." ValidationGroup="TripUnlockForm"></asp:RequiredFieldValidator>
<div class="overlay_content">
No longer need to protect your trip? Enter your
<br />password to remove protection.
</div>
</div>

<div class="overlay_form" id="TripAuthFormContainer">
<h3>Unlock Your Trip</h3><br />
<asp:TextBox runat="server" ID="TripAuthPassword" ValidationGroup="TripAuthForm" ViewStateMode="Disabled"></asp:TextBox>
<asp:Button runat="server" ID="TripAuthButton" Text="Edit" onclick="btn_auth_Click" ValidationGroup="TripAuthForm"/>
<br />
<asp:RequiredFieldValidator runat="server" ControlToValidate="TripAuthPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Password is required." ValidationGroup="TripAuthForm"></asp:RequiredFieldValidator>
<asp:CustomValidator runat="server" OnServerValidate="btn_auth_Validate" ControlToValidate="TripAuthPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Invalid password." ValidationGroup="TripAuthForm"></asp:CustomValidator>
<div class="overlay_content">
To edit your trip, enter your password.
</div>
</div>

<div class="overlay_form" id="TripNewSegmentContainer">
<h3>Add a new segment</h3><br />
<fieldset>
<asp:TextBox runat="server" ID="Segment_Name" ValidationGroup="TripNewSegment"></asp:TextBox><br />
<asp:TextBox runat="server" ID="Segment_Description" TextMode="MultiLine" Rows="3" Width="300"></asp:TextBox><br />
<asp:Button runat="server" ID="AddNewSegmentButton" Text="Add" OnClientClick="NewSegment()" OnClick="btn_new_segment" ValidationGroup="TripNewSegment" />
<br /><asp:RequiredFieldValidator runat="server" ControlToValidate="Segment_Name" CssClass="error" SetFocusOnError="true" ErrorMessage="Segment name is required." ValidationGroup="TripNewSegment"></asp:RequiredFieldValidator>
</fieldset>
<br />
<div class="overlay_content">
Enter a name for your new segment.
</div>
</div>

</form>

<!-- End server form -->

<script type="text/javascript">
/**
 * Enable visuals on page load
 **/
function pageLoad() {
    //Hide all segment content by default
    $('.segment_content_item').hide();
    //Special handling for segment mouseovers
    $('.segment_menu_item').mouseenter(function() {
     if ($(this).attr("class") == "segment_menu_item") {
       $(this).attr("class","segment_menu_hover");
     }
    });
    $('.segment_menu_item').mouseleave(function() {
     if ($(this).attr("class") == "segment_menu_hover") {
       $(this).attr("class","segment_menu_item");
     }
    });
    $('.segment_menu_item').click(function() {
     $('.segment_menu_active').attr("class","segment_menu_item");
     $(this).attr("class", "segment_menu_active");
     $('.segment_content_item:visible').hide();
     var numSelected=$(this).attr("name");
     $('.segment_content_item[name="' + numSelected + '"]').show();
    });
    //Make dialogs clickable
    $('span[rel]').each(function() {    
     $(this).overlay({
        top: '27%',
        speed: 0,
     });
    });
}
/**
 * Close new segment dialog after creating a new segment
 **/
function NewSegment()
{
  if ($('#<%=Segment_Name.ClientID %>').val().length != 0) {    
    $('#TripNewSegmentButton').data('overlay').close();
  }
}
/**
 * Clear inputs after async postback
 **/
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
function EndRequestHandler(sender, args) {
  $('#<%=Segment_Name.ClientID %>').val('');
}
</script>

</asp:Content>
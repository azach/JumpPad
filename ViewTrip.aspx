<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewTrip.aspx.cs" MasterPageFile="~/MasterPage.master" Inherits="ViewTrip" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
<link rel="stylesheet" href="css/leaflet.css" />
<!--[if lte IE 8]><link rel="stylesheet" href="leaflet/leaflet.ie.css" /><![endif]-->
</asp:Content>

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
   <div class="segment_menu_item" name="<%# DataBinder.Eval(Container.DataItem, "Segment_ID") %>">
       <span title="Name"><%# DataBinder.Eval(Container.DataItem, "Name") %></span>
       <input style="display: none" name="Latitude" Value='<%# DataBinder.Eval(Container.DataItem, "Latitude") %>' />
       <input style="display: none" name="Longitude" Value='<%# DataBinder.Eval(Container.DataItem, "Longitude") %>' />
   </div>
 </ItemTemplate>
 <FooterTemplate>
 </FooterTemplate>
</asp:Repeater>
<asp:Literal runat="server" ID="TripNewSegment"><span id="TripNewSegmentButton" rel="#TripNewSegmentContainer"><div class="segment_menu_special" id="segment_add_new">Add New Segment</div></span></asp:Literal>
<!-- Segment map -->
<script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=true"></script>
<div id="map_canvas" style="height: 250px; width: 230px;"></div>
<input type="text" style="width: 198px; margin-left: 2px"></input>
</div>
<!-- End segment map -->

<div class="segment_content">
<asp:Repeater ID="Segment_Content" runat="server">
 <HeaderTemplate></HeaderTemplate>
 <ItemTemplate>
   <div class="segment_content_item" name="<%# DataBinder.Eval(Container.DataItem, "Segment_ID") %>">
    <i><%# DataBinder.Eval(Container.DataItem, "Description") %></i>
   </div>
 </ItemTemplate>
 <FooterTemplate>
 </FooterTemplate>
</asp:Repeater>
</div>

<div class="content_footer">
<asp:HiddenField ID="Delete_Segment_ID" runat="server" />
Activity controls here <span class="content_footer_item"><asp:Button ID="Delete_Segment" Text="Delete Segment" runat="server" OnClick="btn_delete_Segment" /></span>
</div>

</ContentTemplate>
</asp:UpdatePanel>

</div>

<!-- End trip -->

<div class="overlay_form" id="TripLockFormContainer">
<div class="overlay_header">
Protect your trip
</div>
<div class="overlay_content">
<asp:TextBox runat="server" ID="TripLockPassword" ValidationGroup="TripLockForm"></asp:TextBox>
<asp:Button runat="server" ID="TripLockButton" Text="Make Private" onclick="btn_set_access" ValidationGroup="TripLockForm"/>
<asp:Button runat="server" ID="TripViewOnlyButton" Text="Make View Only" onclick="btn_set_access" ValidationGroup="TripLockForm"/>
<br /><asp:RequiredFieldValidator runat="server" ControlToValidate="TripLockPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Password is required." ValidationGroup="TripLockForm"></asp:RequiredFieldValidator>
<br />Want to protect your trip? Lock it here and only you can see and edit it.
<br /><br />
Want to share with others? Make it view only so that others can see it,
<br />but only you can edit it.
</div>
</div>

<div class="overlay_form" id="TripUnlockFormContainer">
<div class="overlay_header">
Unprotect your trip
</div>
<div class="overlay_content">
<asp:TextBox runat="server" ID="TripUnlockPassword" ValidationGroup="TripUnlockForm"></asp:TextBox>
<asp:Button runat="server" ID="TripUnlockButton" Text="Unlock" onclick="btn_set_access" ValidationGroup="TripUnlockForm"/>
<br /><asp:RequiredFieldValidator runat="server" ControlToValidate="TripUnlockPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Password is required." ValidationGroup="TripUnlockForm"></asp:RequiredFieldValidator>
<br />No longer need to protect your trip? Enter your
<br />password to remove protection.
</div>
</div>

<div class="overlay_form" id="TripAuthFormContainer">
<div class="overlay_header">
Edit your trip
</div>
<div class="overlay_content">
<asp:TextBox runat="server" ID="TripAuthPassword" ValidationGroup="TripAuthForm" ViewStateMode="Disabled"></asp:TextBox>
<asp:Button runat="server" ID="TripAuthButton" Text="Edit" onclick="btn_auth_Click" ValidationGroup="TripAuthForm"/>
<br />
<asp:RequiredFieldValidator runat="server" ControlToValidate="TripAuthPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Password is required." ValidationGroup="TripAuthForm"></asp:RequiredFieldValidator>
<asp:CustomValidator runat="server" OnServerValidate="btn_auth_Validate" ControlToValidate="TripAuthPassword" CssClass="error" SetFocusOnError="true" ErrorMessage="Invalid password." ValidationGroup="TripAuthForm"></asp:CustomValidator>
<br />To edit your trip, enter your password.
</div>
</div>

<div class="overlay_form" id="TripNewSegmentContainer">
<div class="overlay_header">
Add a new segment
</div>
<asp:TextBox runat="server" ID="Segment_Name" Width="300" ValidationGroup="TripNewSegment"></asp:TextBox><br />
<asp:TextBox runat="server" ID="Segment_Description" TextMode="MultiLine" Rows="3" Width="300"></asp:TextBox><br />
<asp:Button runat="server" ID="AddNewSegmentButton" Width="100" Text="Add" OnClientClick="NewSegment()" OnClick="btn_new_segment" ValidationGroup="TripNewSegment" />
<br /><asp:RequiredFieldValidator runat="server" ControlToValidate="Segment_Name" CssClass="error" SetFocusOnError="true" ErrorMessage="Segment name is required." ValidationGroup="TripNewSegment"></asp:RequiredFieldValidator>
<br />
<div class="overlay_content">
</div>
</div>

</form>

<!-- End server form -->

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">

<script type="text/javascript" src="/scripts/map.js"></script>
<script type="text/javascript" src="/scripts/jquery.watermarkinput.js"></script>
<script type="text/javascript">
/**
 * Enable visuals on page load
 **/
function pageLoad() {
    //Watermarks
    $('#<%=Segment_Name.ClientID %>').Watermark('Enter a segment name');
    $('#<%=Segment_Description.ClientID %>').Watermark('Enter a description');
    //Create Google Maps canvas with function necessary to save on changes
    InitializeMap(function SaveLocation() {

        var id = $('.segment_menu_active').attr('name');
        var lat = this.getPosition().lat();
        var lng = this.getPosition().lng();

        if (id.length == 0) { return; }
        
        if ($('.segment_menu_active') != null) {
            // Replace the div's content with the values return.
            var loc_info = "{'segment':" + id + ", 'lat':" + lat + ", 'lng':" + lng + " }";
            $.ajax({
              type: "POST",
              url: "TripService.asmx/SetLocation",
              data: loc_info,
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              success: function(msg) {
                $('.segment_menu_active').children('input[name="Latitude"]').val(lat);
                $('.segment_menu_active').children('input[name="Longitude"]').val(lng);
              }
            });
        }
    });
    //Add markers for each location on map
    $('.segment_menu_item').each(function() {
        var segment_id=$(this).attr("name");
        var lat = $(this).children('input[name="Latitude"]').val();
        var lng = $(this).children('input[name="Longitude"]').val();
        AddMarker({
            name: segment_id,
            latitude: lat,
            longitude: lng,
            title: $(this).children('span[title="Name"]').html()
        });
    });
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
    //Events to fire when active segment is changed
    $('.segment_menu_item').click(function() {
        $('#<%=Delete_Segment_ID.ClientID %>').val($(this).attr('name'));
        $('.segment_menu_active').attr("class","segment_menu_item");
        $(this).attr("class", "segment_menu_active");
        $('.segment_content_item:visible').hide();
        var numSelected=$(this).attr("name");
        $('.segment_content_item[name="' + numSelected + '"]').show();
        //Set map marker
        SetActive(numSelected);
    });
    //Make dialog links clickable
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
  $('#<%=Segment_Description.ClientID %>').val('');
}
</script>
</asp:Content>
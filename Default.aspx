﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" MasterPageFile="~/MasterPage.master" Inherits="_Default" %>

<asp:Content ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">

<center>
<div style="min-width: 400px; width: 450px; padding-top: 50px;">
<img id="logo_J" src="images/J.png" class="logo_sep" />
<img id="logo_u" src="images/u.png" class="logo_sep" />
<img id="logo_m" src="images/m.png" class="logo_sep" />
<img id="logo_p" src="images/p.png" class="logo_sep" />
<img id="logo_Pcap" src="images/Pcap.png" class="logo_sep" />
<img id="logo_a" src="images/a.png" class="logo_sep" />
<img id="logo_d" src="images/d.png" class="logo_sep" />
<div style="text-align: right;"><h2>Get Excited!</h2></div>
</div>
</center>

<div style="display: block; margin-left: 400px; margin-right: 400px; min-width: 400px;">
<ul>
<h3 style="color: #B9CBD6;">
<li>Why use us?</li>
<li>Lorem ipsum dolor sit amet, consectetur adipiscing.</li>
<li>Proin pulvinar risus sed nulla interdum tincidunt.</li>
<li>Aliquam ac quam felis.</li>
</h3>
</ul>
</div>

<center>
<form id="form_create" runat="server">
<input type="text" value="" name="name" id="name" autocomplete="off" style="width: 300px"/>
<asp:Button id="btn_create" Text="Create Trip" runat="server" onclick="btn_create_Click" />
</form>
</center>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
<script type="text/javascript">
$(document).ready(function () {
    var bounce = false;
    var name_def = "Enter a trip name";
    $("#name").Watermark(name_def);
    //Bouncing logo!
    $('img[class="logo_sep"]').mouseenter(function () {
        if (!bounce) {
            bounce = true;
            $(this).animate({ top: ['-=50', 'easeOutQuad'] }, 250);
            $(this).animate({ top: ['+=50', 'easeInQuad'] }, 250);
            $(this).animate({ top: ['-=25', 'easeOutQuad'] }, 250);
            $(this).animate({ top: ['+=25', 'easeInQuad'] }, 250);
            bounce = false;
        }
    });
});
</script>
</asp:Content>
// Copyright 2011 JumpPad All Rights Reserved.

/**
* @fileoverview Custom map for defining the location and area of a trip area
* @author azach (https://github.com/azach)
*/

var map;
var home;
var segment_marker;
var markers = new Array();
var EndClick;
var active;

var disabled_marker = "/images/map_marker_inactive.png";

/**
* Initialize Google map
* @param {function} EndClick - Code to fire when the marker is clicked after it stops bouncing.
**/
function InitializeMap(end_click_func) {
    // Create a div to hold the control.
    var controlDiv = document.createElement('DIV');
    EndClick = end_click_func;

    // Set CSS styles for the DIV containing the control
    // Setting padding to 5 px will offset the control
    // from the edge of the map
    controlDiv.style.padding = '5px';

    // Set CSS for the control image
    var controlUI = document.createElement('DIV');
    controlUI.style.width = "18px";
    controlUI.style.height = "40px";
    controlUI.style.backgroundImage = "url('/images/map_marker.png')";
    controlUI.style.backgroundRepeat = "no-repeat";
    controlUI.style.cursor = 'pointer';
    controlUI.title = 'Click to set the map to Home';
    controlDiv.appendChild(controlUI);

    home = new google.maps.LatLng(40.69847032728747, -73.9514422416687);

    var options = {
        zoom: 4,
        center: home,
        disableDefaultUI: true,
        zoomControl: true,
        streetViewControl: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    //Create the map object
    map = new google.maps.Map(document.getElementById("map_canvas"), options);
}

/**
* Add a segment marker to the map
* @param {object} options - Literal with following parameters
*                             name - Unique name of marker
*                             latitude - Latitude of marker
*                             longitude - Longitude of marker
*                             title - Text to display in marker tooltip
**/
function AddMarker(options) {    
    var name = options.name;
    var lat = options.latitude;
    var lng = options.longitude;

    //Already exists
    if (markers[name] != null) { return; }
    if ((lat == null) || (lng == null)) { return; }

    markers[name] = new google.maps.Marker({
        position: home,
        animation: google.maps.Animation.DROP,
        draggable: false,
        flat: true,
        icon: disabled_marker,
        cursor: 'none',
        title: options.title
    });

    markers[name].setMap(map);
    SetLocation(name, lat, lng);

    //Set up marker events
    //Auto-bounce on drag; save position when user ends bouncing
    google.maps.event.addListener(markers[name], 'dragend', StartBounce);
    google.maps.event.addListener(markers[name], 'click', StopBounce);
    if (EndClick != null) {
        google.maps.event.addListener(markers[name], 'click', EndClick);
    }
}

/**
* Attempts to set the map marker location to the given latitude, longitude
* If it fails, sets the map marker to the home location.
**/
function SetLocation(name, lat, lng) {
    if (markers[name] == null) { return; }
    var new_center;
    try {
        //Default to home location if one isn't defined
        if ((lat == "") || (lng == "")) {
            new_center = home;
        }
        else {
            new_center = new google.maps.LatLng(lat, lng);
        }
    }
    //Couldn't create latlng object; default to home
    catch(err) {
        new_center = home;
    }
    //Update map location
    map.panTo(new_center);
    markers[name].setPosition(new_center);
}

//Sets the specified marker as the active one
function SetActive(name) {
    if (markers[name] == null) { return; }
    var old_active = active;
    //Deactive old marker
    if (markers[old_active] != null) {
        markers[old_active].setDraggable(false);
        markers[old_active].setIcon(disabled_marker);
        markers[old_active].setCursor('none');
    }
    //Activate new marker
    active = name;

    markers[active].setDraggable(true);
    markers[active].setIcon("");
    map.panTo(markers[active].getPosition());
}

//Start bouncing marker
function StartBounce() {
    this.setAnimation(google.maps.Animation.BOUNCE)
}

//Stop bouncing marker
function StopBounce() {
    this.setAnimation(null);
}
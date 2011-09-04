// Copyright 2011 JumpPad All Rights Reserved.

/**
* @fileoverview Custom map for defining the location and area of a trip area
* @author azach (https://github.com/azach)
*/

var map;
var home;
var segment_marker;

/**
* Initialize Google map
* @param {function} EndClick - Code to fire when the marker is clicked after it stops bouncing.
**/
function InitializeMap(EndClick) {
    // Create a div to hold the control.
    var controlDiv = document.createElement('DIV');

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
        zoom: 8,
        center: home,
        disableDefaultUI: true,
        zoomControl: true,
        streetViewControl: false,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    //Create the map object
    map = new google.maps.Map(document.getElementById("map_canvas"), options);

    segment_marker = new google.maps.Marker({
        position: home,
        animation: google.maps.Animation.DROP,
        draggable: true,
        flat: true
    });

    //Initialize the segment marker
    segment_marker.setMap(map);

    //Set up marker events
    //Auto-bounce on drag; save position when user ends bouncing
    google.maps.event.addListener(segment_marker, 'dragend', StartBounce);
    google.maps.event.addListener(segment_marker, 'click', StopBounce);
    google.maps.event.addListener(segment_marker, 'click', EndClick);
}

/**
* Attempts to set the map marker location to the given latitude, longitude
* If it fails, sets the map marker to the home location.
**/
function SetLocation(lat, lng) {
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
    //Couldn't create latlng object; defalut to home
    catch(err) {
        new_center = home;
    }
    //Update map location
    map.panTo(new_center);
    segment_marker.setPosition(new_center);
}

//Start bouncing marker
function StartBounce() {
    segment_marker.setAnimation(google.maps.Animation.BOUNCE)
}

//Stop bouncing marker
function StopBounce() {
    segment_marker.setAnimation(null);
}
// Copyright 2011 JumpPad All Rights Reserved.

/**
* @fileoverview Custom map for defining the location and area of a trip area
* @author azach (https://github.com/azach)
*/

var map;
var menu;
var home;
var segment_marker;
var markers = new Array();
var active;
var disabled_marker = "/images/map_marker_inactive.png";


/************************************************
Map functions
************************************************/

/**
* Initialize Google map
* @param object opts - List of options. Save is the function used to save a marker's position
**/
function InitializeMap(opts) {    

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

    if (opts.save != undefined) {
        map.saveLocation = opts.save;
    }

    menu = new ContextMenu({ map: map });

    menu.addItem('Place Marker Here', function (map, latLng) {
        active.setLocation(latLng.lat(), latLng.lng())
        active.saveLocation();
        map.panTo(latLng);
    });

    //Set home locations
    GetUserHome();
    home = new google.maps.LatLng(60, 150);
}

/************************************************
  Marker functions
************************************************/

/**
* Add a segment marker to the map
* @param {object} options - Literal with following parameters
*                             name - Unique name of marker
*                             latitude - Latitude of marker
*                             longitude - Longitude of marker
*                             title - Text to display in marker tooltip
**/
function addMarker(options) {
    var name = options.name;
    var lat = options.latitude;
    var lng = options.longitude;

    //Already exists or bad parameters
    if (markers[name] != null) { return; }
    if ((lat == null) || (lng == null)) { return; }

    markers[name] = new google.maps.Marker({
        position: home,
        animation: google.maps.Animation.DROP,
        draggable: false,
        flat: true,
        icon: disabled_marker,
        cursor: 'none',
        title: options.title,
        saveLocation: map.saveLocation
    });

    markers[name].setMap(map);
    markers[name].setLocation(lat, lng); 

    //Set up default marker events (bounce on drag, stop bouncing to save) 
    google.maps.event.addListener(markers[name], 'dragend', markers[name].StartBounce);
    google.maps.event.addListener(markers[name], 'click', markers[name].StopBounce);

    //Set up custom save event
    if (markers[name].saveLocation != null) {
        google.maps.event.addListener(markers[name], 'click', markers[name].saveLocation);
    }
}

/**
* Attempts to set the map marker location to the given latitude, longitude
* If it fails, sets the map marker to the home location.
**/
google.maps.Marker.prototype.setLocation = function (lat, lng) {
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
    catch (err) {
        new_center = home;
    }

    //Update map location
    map.panTo(new_center);
    this.setPosition(new_center);
}

//Sets the specified marker as the active one
google.maps.Marker.prototype.SetActive = function () {

    var old_active = active;

    //Deactive old marker
    if (old_active != null) {
        old_active.setDraggable(false);
        old_active.setIcon(disabled_marker);
        old_active.setCursor('none');
    }
    //Activate new marker
    active = this;

    this.setDraggable(true);
    this.setIcon("");
    map.panTo(this.getPosition());
}

/**
* Start bouncing the marker
**/
google.maps.Marker.prototype.StartBounce = function() {
    this.setAnimation(google.maps.Animation.BOUNCE)
}

/**
* Stop bouncing the marker
**/
google.maps.Marker.prototype.StopBounce = function () { this.setAnimation(null); }

/**
* Get the user's home location and set it to home
**/
function GetUserHome() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            map.setCenter(initialLocation);
        });
    }
}


/************************************************
Context menu functions
************************************************/

/**
* Create a right=click context menu
* @param object opts List of options for context menu
*/
function ContextMenu(opts) {
    // A way to access 'this' object from inside functions
    var self = this;

    if (opts.map != undefined) {
        // Put the map onto the object
        this.theMap = opts.map;

        // Keep track of where you clicked, for the callback functions.
        this.clickedLatLng = null;

        // Create the context menu element
        this.theMenu = $(document.createElement('div'))
				.attr('class', 'contextMenu')

        // .. disable the browser context menu on our context menu
				.bind('contextmenu', function () { return false; })

        // .. append a ul list element
				.append($(document.createElement('ul')))

        // .. then append it to the map object
				.appendTo(this.theMap.getDiv());

        // Display and position the menu
        google.maps.event.addListener(this.theMap, 'rightclick', function (e) {
            if ((active == null) || (active == undefined)) { return; } //Only show if a segment is selected
            // Shorthand some stuff
            var mapDiv = $(self.theMap.getDiv()),
					menu = self.theMenu,
					x = e.pixel.x,
					y = e.pixel.y;

            // Hide the context menu if its open
            menu.hide();

            // Save the clicked location
            self.clickedLatLng = e.latLng;

            // Adjust the menu if clicked to close to the edge of the map
            if (x > mapDiv.width() - menu.width())
                x -= menu.width();

            if (y > mapDiv.height() - menu.height())
                y -= menu.height();

            // Set the location and fade in the context menu
            menu.css({ top: y, left: x }).fadeIn(200);
        });

        // Hide context menu on several events
        $.each('click dragstart zoom_changed maptypeid_changed center_changed'.split(' '), function (i, name) {
            google.maps.event.addListener(self.theMap, name, function () { self.theMenu.hide(); });
        });
    }
}
/**
* Add new items to the context menu
* @param string   name     Name of the list item.
* @param function callback Function to run when you click the list item
* @return jQuery           The list item that is created.
*/
ContextMenu.prototype.addItem = function (name, loc, callback) {
    // If no loc was provided
    if (typeof loc === 'function') {
        callback = loc;
        loc = undefined;
    }

    // A way to access 'this' object from inside functions
    var self = this,

    // The name turned into camelCase for use in the li id, and anchor href
			idName = name,

    // The li element
			li = $(document.createElement('li'))
				.attr('id', idName);

    // the anchor element
    $(document.createElement('a'))
			.attr('href', '#' + idName).html(name)
			.appendTo(li)

    // Add some nice hover effects
			.hover(function () {
			    $(this).parent().toggleClass('hover');
			})

    // Set the click event
			.click(function () {

			    // fade out the menu
			    self.theMenu.hide();

			    // call the callback function - 'this' would refer back to the jQuery object of the item element
			    callback.call(this, self.theMap, self.clickedLatLng);

			    // make sure the click doesnt take us anywhere
			    return false;
			});

    // If `loc` is a number put it at that location
    if (typeof loc === 'number' && loc < this.theMenu.find('li').length)
        this.theMenu.find('li').eq(loc).before(li);

    // Else appened it to the end of the menu
    else {
        this.theMenu.find('ul').append(li);
    }

    // Return the whole list item
    return li;
};

/**
* Add a seperators
* @params loc Location to add separator (optional)
* @return jQuery The list item that is created.
*/
ContextMenu.prototype.addSep = function (loc) {
    // Create the li element
    var li = $(document.createElement('li'))
			.addClass('separator')

    // .. add a div child
			.append($(document.createElement('div')))

    // If loc is a number put the li at that location
    if (typeof loc === 'number')
        this.theMenu.find('li').eq(loc).before(li)

    // .. else appened it to the end
    else
        this.theMenu.find('ul').append(li);

    // Return the li element
    return li
};

/**
* Remove a menu list item.
* @param string name The string used to create the list item.
* @param number name The index value of the list item.
* @param jQuery name The jQuery object that is returned by addItem() or addSep()
*/
ContextMenu.prototype.remove = function (item) {
    // No need to search for name if its a jquery object
    if (item instanceof $)
        item.remove();

    // Find all the elements and remove the one at the specified index
    else if (typeof item === 'number')
        this.theMenu.find('li').eq(item).remove()

    // Find all the items by the id name and remove them
    else if (typeof item === 'string') {
        // Find and remove the element
        this.theMenu.find('#' + item).remove()
    }
};
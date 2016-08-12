function initialize(lat, long) {
    var latt;
    var longg;
    console.log("111",lat);
    if (lat !== undefined) {
        latt = lat;
    } else {
        latt = 40.561766399999996;
    }
    if (long !== undefined) {
        longg = long;
    } else {
        longg = 49.710874399999994;
    }
    var mapOptions = {
        center: new google.maps.LatLng(latt,longg),
        zoom: 9,
        scrollwheel: true,
        draggable: true,
        panControl: true,
        zoomControl: true,
        mapTypeControl: true,
        scaleControl: true,
        streetViewControl: true,
        overviewMapControl: true,
        rotateControl: true,
    };
    var map = new google.maps.Map(document.getElementById("map-canvas"), mapOptions);
}
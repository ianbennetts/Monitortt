
var deviceStatus = new Array(5);

$(document).ready(function () {
    setInterval(validateCode, 10000);
});


function validateCode() {
    var datatosend = project;
    var request = new XMLHttpRequest();
    request.open("POST", "/API/Data/Device", true);
    request.setRequestHeader("Content-Type", "text/plain");
    request.onreadystatechange = function () {
        if ((request.readyState == 4) && (request.status == 200)) {
            processData(request.responseText);
        } else processData(request.response);
    };
    request.send(datatosend);
}

function processData(response) {
    var listofdevices = response.split(",");
    var length = parseInt(listofdevices[1]);
    var when;
    for (i = 0; i < length; i++) {
        s = "When[" + (i + 1) + "]";
        when = document.getElementById("When[" + (i + 1) + "]");
        when.innerHTML = listofdevices[(((i + 1) * 3) + 1)];
        var noofsights = document.getElementById("noSightings["+ (i+1) +"]");
        noofsights.innerHTML = listofdevices[(((i + 1) * 3)-1)];

        if (listofdevices[(((i + 1) * 3) )] == 1) {

            setIconDevice((i+1), 1)
        }
        else {

            setIconDevice((i+1 ), 0)
        }
    }
}
 

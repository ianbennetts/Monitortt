$(document).ready(function () {
    // setInterval(validateCode, 10000);
    validateCode();
    setInterval(validateCode, 10000);
    var graphArea;
});


function validateCode() {
    var datatosend = "GetGraph";
    var request = new XMLHttpRequest();
    request.open("POST", "/API/Data/index", true);
    request.setRequestHeader("Content-Type", "text/plain");
    request.onreadystatechange = function () {
        if ((request.readyState == 4) && (request.status == 200)) {
            processData(request.responseText);
        } else processData(request.response);
    };
    request.send(datatosend);
}

function processData(response) {

    var listofData = response.split(',');
    var length = parseInt(listofData[1]);
    var s = "";
    var t = 0;
    var buffer = "s";
    var graphArea = document.getElementById("Last90TT[" + (0) + "]");
    var k = 0;
    var p = 0;
    for (var j = 1; j < (6 * 8) ; j = j + 6) {
        buffer = (listofData[(j + 6)].split(' '));
        s = "";
        for (var i = 1; i < (buffer.length - 1) ; i++) {
            t = t + parseInt(buffer[i]);
        }
        graphArea = document.getElementById("Last90TT[" + (k++) + "]");
        graphArea.innerHTML = t;     
    }

}

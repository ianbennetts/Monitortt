﻿var compareData = new Array(180);
var currentData = new Array(180);
var highBound, lowBound;
var scaler=360/120;
$(document).ready(function () {
    // setInterval(validateCode, 10000);
    var compareData = new Array(180);
    var currentData = new Array(180);
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
    var t = "";
    var bufferTT = "s";
    var bufferBase = "s";
    var graphArea = document.getElementById("Last90TT[" + (0) + "]");
    var speed = document.getElementById("CurrentSpeed[" + (0) + "]");
    var currentTT = document.getElementById("CurrentTT[" + (0) + "]");
    var baseSpeed = document.getElementById("BaseSpeed[" + (0) + "]");
    var baseTT = document.getElementById("BaseTT[" + (0) + "]");
    var timeDelta = document.getElementById("TimeDelta[" + (0) + "]");
    var speedDelta = document.getElementById("SpeedDelta[" + (0) + "]");
    var highBase, lowBase, highTT, lowTT;
    var k = 0;
    var p = 0;
    var hold;
    for (var j = 1; j < (7 * 8) ; j = j + 7) {
        if (typeof listofData[(j + 7)] !== 'undefined') {
            bufferTT = (listofData[(j + 6)].split(' '));
            bufferBase = (listofData[(j + 7)].split(' '));
            s = "";
            graphArea = document.getElementById("Last90TT[" + (k) + "]");
            t = "";
            for (var i = 1; i < (bufferTT.length - 1) ; i++) {
                t = t + bufferTT[i] + " ";
                currentData[i - 1] = parseInt(bufferTT[i]);
            }
            for (var i = 1; i < (bufferBase.length - 1) ; i++) {
                //      s = s + bufferBase[i] + " ";
                compareData[i - 1] = parseInt(bufferBase[i]);
            }
            lowBound = getLow(currentData);
            hold = getLow(compareData);
            if (lowBound > hold) { lowBound = hold; }
            highBound = getHigh(currentData);
            hold = getHigh(compareData);
            if (highBound < hold) { highBound = hold; }
            var diff = highBound - lowBound;
            if (diff < 300) { diff = (300-diff) / 2; highBound = highBound + diff; lowBound = lowBound - diff;}
            drawGraph(k, lowBound, highBound, currentData, compareData);
            // graphArea.innerHTML = lowBound+" "+highBound+" "+currentData;
            // graphArea = document.getElementById("Last90TT[" + (k) + "]");
            // graphArea.innerHTML = listofData[(j + 5)];
            speed = document.getElementById("CurrentSpeed[" + (k) + "]");
            speed.innerHTML = (listofData[(j + 5)]+"kph");
            min = Math.floor(parseInt(listofData[(j + 4)]) / 60);
            sec = parseInt(listofData[(j + 4)]) - (min * 60);
            currentTT = document.getElementById("CurrentTT[" + (k) + "]");
            currentTT.innerHTML = (min + "m " + sec + "s");
            baseSpeed = document.getElementById("BaseSpeed[" + (k) + "]");
            baseSpeed.innerHTML = (listofData[(j + 3)]+"kph");
            min = Math.floor(parseInt(listofData[(j + 2)]) / 60);
            sec = parseInt(listofData[(j + 2)]) - (min * 60);
            baseTT = document.getElementById("BaseTT[" + (k++) + "]");
            baseTT.innerHTML = (min + "m " + sec + "s");
            speedDelta = document.getElementById("SpeedDelta[" + (k++) + "]");
            speedDelta.innerHTML = ((listofData[(j + 3)] - listofData[(j + 5)]) + "kph");
            timeDelta = document.getElementById("TimeDelta[" + (k++) + "]");
            min = Math.floor(parseInt(listofData[(j + 2)])-parseInt(listofData[(j + 4)]) / 60);
            sec = parseInt(listofData[(j + 2)])-parseInt(listofData[(j + 4)]) - (min * 60);
            timeDelta.innerHTML = (min + "m " + sec + "s");

        }
        
    }

}
function drawGraph(link,lowBound,highBound,currentData,compareData) {
    var canvas = document.getElementById("Last90TT["+ (link) +"]");
    var graph = canvas.getContext("2d");
    var scale, offset, window;
    var high, low, topMargin, bottomMargin, yhight;
    window = 120 - 10 - 10;
    topMargin = Math.ceil((highBound - lowBound) * .1)
    scale = (highBound - lowBound) / window
    graph.beginPath();
    graph.rect(0, 0, 360, 120);
    drawBackGround(graph, highBound, lowBound, topMargin, scale);
    graph.stroke();
    graph.strokeStyle = 'red';
    graph.lineWidth = 1;
    graph.beginPath();
    graph.moveTo(0, 50);
    for (var j = 0; j < 180; j++) {
        if (currentData[j] != 1) graph.lineTo(j*(360/180), (((highBound + topMargin) / scale) - (currentData[j] / scale)));
    }
    graph.lineJoin = 'round';
    graph.stroke();
    graph.stroke();
    graph.strokeStyle = 'blue';
    graph.lineWidth = 1;
    graph.beginPath();
    graph.moveTo(0, 50);
    for (var j = 0; j < 180; j++) {
        graph.lineTo(j * (360 / 180), (((highBound + topMargin) / scale) - (compareData[j] / scale)));
    }
    graph.lineJoin = 'round';
    min = Math.floor(highBound / 60);
    sec = highBound - (min * 60);
    graph.stroke();
    graph.font = "10px Arial";
    graph.fillStyle = "rgba(0,0,0, 1)";
    graph.fillText(min + "m " + sec + "s", 2, 17);
    min = Math.floor(lowBound / 60);
    sec = lowBound - (min * 60);
    graph.fillText(min + "m " + sec + "s", 2, 87);
    graph.fillText("90               75", 29, 116);
    graph.fillText("60              45", 138, 116);
    graph.fillText("30               15", 245,116);

}
function drawBackGround(graph,high,low,topMargin,scale) {
    graph.beginPath();
    graph.fillStyle = "rgba(200,200,200, 1)";
    //     context.clearRect(0, 0, 300, 100);
    graph.fillRect(0, 0, 360, 120);
    graph.fillStyle = "rgba(0,0,0, 1)";
    graph.rect(0, 0, 360, 120);
    graph.lineWidth = 1;
    graph.strokeStyle = 'grey';
    graph.moveTo(((scaler) * 12), 0);
    graph.lineTo((((scaler) * 12)), 120);
    graph.moveTo(((scaler) * 30), 0);
    graph.lineTo((((scaler) * 30)), 120);
    graph.moveTo(((scaler) * 48), 0);
    graph.lineTo((((scaler) * 48)), 120);
    graph.moveTo(((scaler) * 65), 0);
    graph.lineTo((((scaler) * 65)), 120);
    graph.moveTo(((scaler) * 84), 0);
    graph.lineTo((((scaler) * 84)), 120);
    graph.moveTo(((scaler) * 102), 0);
    graph.lineTo((((scaler) * 102)), 120);
    graph.moveTo(0, 36);
    graph.lineTo(360, 36);
    graph.moveTo(0, 72);
    graph.lineTo(360, 72);
    graph.moveTo(0,(((high+topMargin)/scale)- (high/scale)));
    graph.lineTo(360, (((high + topMargin) / scale) - (high / scale)))
    graph.moveTo(0, (((high + topMargin) / scale) - (low / scale)));
    graph.lineTo(360, (((high + topMargin) / scale) - (low / scale)))
    graph.stroke();
    /*
    
        for (var i = 0; i < 270; i = i + 45) {
            graph.beginPath();
            graph.moveTo(i, 100);
            graph.lineTo(i, 0);
            graph.stroke();
        }*/
}
function getHigh(a) {
    var high = 0;
    var i;
    for ( i = 0; i < 180; i++) {
        if (high < a[i]) {
            high = a[i];
        }
        
    }
    return high;
}

function getLow(a) {
    var low = 20000;
    for (var i = 0; i < 180; i++) {
        if (a[i] != 1 && low > a[i]) {
            low = a[i];
        }
    }
    return low;
}
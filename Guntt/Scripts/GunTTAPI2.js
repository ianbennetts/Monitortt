var compareData = new Array(180);
var currentData = new Array(180);
var highBound, lowBound;
var scaler = 360 / 120;

$(document).ready(function () {
    // setInterval(validateCode, 10000);
    var compareData = new Array(180);
    var currentData = new Array(180);
    validateCode();
    setInterval(validateCode, 10000);
    var graphArea;

});


function validateCode() {
    var datatosend = "GetGraphSth";
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
    var totalBaseTT = 0;
    var totalBaseSP = 0;
    var totalTT = 0;
    var totalSP = 0;
    var accBaseTT = 0;
    var accBaseSP = 0;
    var accTT = 0;
    var accSP = 0;
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
    var docBaseTT = document.getElementById("BaseTT");
    var docBaseSpeed = document.getElementById("BaseSpeed");
    var docCurrentTT = document.getElementById("CurrentTT");
    var docCurrentSpeed = document.getElementById("CurrentSpeed");
    var docTTDelta = document.getElementById("TimeDelta");

    var highBase, lowBase, highTT, lowTT;
    var compareBigData = new Array(180).fill(0);
    var currentBigData = new Array(180).fill(0);
    var k = 0;
    var p = 0;
    var hold;
    for (var j = 1; j < (7 * 4) ; j = j + 7) {
        if (typeof listofData[(j + 7)] !== 'undefined') {
            bufferTT = (listofData[(j + 6)].split(' '));
            bufferBase = (listofData[(j + 7)].split(' '));
            s = "";
            graphArea = document.getElementById("Last90TT[" + (k) + "]");
            t = "";
            for (var i = 1; i < (bufferBase.length - 1) ; i++) {
                //      s = s + bufferBase[i] + " ";
                compareData[i - 1] = parseInt(bufferBase[i]);
                compareBigData[i - 1] += compareData[i - 1];
            }
            for (var i = 1; i < (bufferTT.length - 1) ; i++) {
                t = t + bufferTT[i] + " ";
                currentData[i - 1] = parseInt(bufferTT[i]);
                if (currentData[i - 1] != 1) {
                    currentBigData[i - 1] += currentData[i - 1];
                } else {
                    currentBigData[i - 1] += getData(i - 1, currentData, compareData[0]);
                }

            }
 
            lowBound = getLow(currentData);
            hold = getLow(compareData);
            if (lowBound > hold) { lowBound = hold; }
            highBound = getHigh(currentData);
            hold = getHigh(compareData);
            if (highBound < hold) { highBound = hold; }
            var diff = highBound - lowBound;
            if (diff < 300) { diff = (300 - diff) / 2; highBound = highBound + diff; lowBound = lowBound - diff; }
            drawGraph(k, lowBound, highBound, currentData, compareData);
            // graphArea.innerHTML = lowBound+" "+highBound+" "+currentData;
            // graphArea = document.getElementById("Last90TT[" + (k) + "]");
            // graphArea.innerHTML = listofData[(j + 5)];
            speed = document.getElementById("CurrentSpeed[" + (k) + "]");
            speed.innerHTML = (listofData[(j + 5)] + "kph");
            min = Math.floor(parseInt(listofData[(j + 4)]) / 60);
            sec = parseInt(listofData[(j + 4)]) - (min * 60);
            currentTT = document.getElementById("CurrentTT[" + (k) + "]");
            currentTT.innerHTML = (min + "m " + sec + "s");
            totalTT = totalTT + (sec + (min * 60));
            baseSpeed = document.getElementById("BaseSpeed[" + (k) + "]");
            baseSpeed.innerHTML = (listofData[(j + 3)] + "kph");
            totalBaseSP = totalBaseSP + parseInt(listofData[(j + 3)]);
            min = Math.floor(parseInt(listofData[(j + 2)]) / 60);
            sec = parseInt(listofData[(j + 2)]) - (min * 60);
            totalBaseTT = totalBaseTT + (sec+(min*60));
            baseTT = document.getElementById("BaseTT[" + (k) + "]");
            baseTT.innerHTML = (min + "m " + sec + "s");
            speedDelta = document.getElementById("SpeedDelta["+(k)+"]");
            speedDelta.innerHTML = ((listofData[(j + 5)] - listofData[(j + 3)]) + " kph");
            timeDelta = document.getElementById("TimeDelta[" + (k++) + "]");
           // min = Math.floor((parseInt(listofData[(j + 2)]) - parseInt(listofData[(j + 4)])) / 60);
            sec = parseInt(listofData[(j + 2)]) - parseInt(listofData[(j + 4)]);
            var secsTotal = sec;
            min = Math.floor(Math.abs(sec) / 60);
            sec = (sec % 60);
            var deltaStr = (min + "m " + Math.abs(sec) + "s");
            if (secsTotal < 0) {
                var displayStr = deltaStr.fontcolor("red").bold();
            } else {
                var displayStr = deltaStr.fontcolor("black");
            }
            timeDelta.innerHTML = displayStr;
 
           
            totalSP = totalSP+parseInt(speed.value);
        }
        docBaseTT.innerHTML = "  " + Math.floor(totalBaseTT / 60) + "m " + (totalBaseTT % 60) + "s";
        docCurrentTT.innerHTML = "  " + Math.floor(totalTT / 60) + "m " + (totalTT % 60) + "s";
        var TTDeltaStr = "  " + Math.floor((Math.abs(totalBaseTT - totalTT)) / 60) + "m " + (Math.abs(totalBaseTT - totalTT) % 60) + "s";
        if ((totalBaseTT - totalTT) < 0) {
            var displayDelta = TTDeltaStr.fontcolor("red").bold();
        } else {
            var displayDelta = TTDeltaStr.fontcolor("black");
        }
        docTTDelta.innerHTML = displayDelta;
        docBaseSpeed.innerHTML = Math.round((17200 / totalBaseTT) * 3.600) + " kph";
        docCurrentSpeed.innerHTML = Math.round((17200 / totalTT) * 3.600) + " kph";
        //drawBigGraph(lowBound, highBound, currentData, compareData);
    }
    lowBound = getLow(currentBigData);
    hold = getLow(compareBigData);
    if (lowBound > hold) { lowBound = hold; }
    highBound = getHigh(currentBigData);
    hold = getHigh(compareBigData);
    if (highBound < hold) { highBound = hold; }
    var diff = highBound - lowBound;
    if (diff < 1800) { diff = (1800 - diff) / 2; highBound = highBound + diff; lowBound = lowBound - diff; }

    drawBigGraph(lowBound, highBound, currentBigData, compareBigData);
}
function drawGraph(link, lowBound, highBound, currentData, compareData) {
    var canvas = document.getElementById("Last90TT[" + (link) + "]");
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
        if (currentData[j] != 1) graph.lineTo(j * (360 / 180), (((highBound + topMargin) / scale) - (currentData[j] / scale)));
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
    graph.fillText("30               15", 245, 116);

}
function drawBackGround(graph, high, low, topMargin, scale) {
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
    graph.moveTo(0, (((high + topMargin) / scale) - (high / scale)));
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
    for (i = 0; i < 180; i++) {
        if (high < a[i]) {
            high = a[i];
        }

    }
    return high;
}
function getData(index, currentData, defaultData) {

    for (i = index-1; i > 0; i--) {
        if (currentData[i] != 1) { return currentData[i];}
    }
    return defaultData;
}

function getLow(a) {
    var low = 20000;
    for (var i = 0; i < 180; i++) {
        if (a[i] > 4 && low > a[i]) {
            low = a[i];
        }
    }
    return low;
}
function drawBigGraph(lowBound, highBound, currentData, compareData) {
    //  return;
    var canvas = document.getElementById("TotalLast90Big");
    var graph = canvas.getContext("2d");
    var scale, offset, window;
    var high, low, topMargin, bottomMargin, yhight;
    window = 200 - 10 - 10;
    topMargin = Math.ceil((highBound - lowBound) * .1)
    scale = (highBound - lowBound) / window
    graph.beginPath();
    graph.rect(0, 0, 600, 200);
    drawBigBackGround(graph, highBound, lowBound, topMargin, scale);
    graph.stroke();
    graph.strokeStyle = 'red';
    graph.lineWidth = 1;
    graph.beginPath();
    graph.moveTo(0, 50);
    for (var j = 0; j < 180; j++) {
        if (currentData[j] != 1) graph.lineTo(j * (600 / 180), (((highBound + topMargin) / scale) - (currentData[j] / scale)));
    }
    graph.lineJoin = 'round';
    graph.stroke();
    graph.stroke();
    graph.strokeStyle = 'blue';
    graph.lineWidth = 1;
    graph.beginPath();
    graph.moveTo(0, 50);
    for (var j = 0; j < 180; j++) {
        if (compareData[j] != 0) graph.lineTo(j * (600 / 180), (((highBound + topMargin) / scale) - (compareData[j] / scale)));
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
    graph.fillText(min + "m " + sec + "s", 2, 180);   
    graph.fillText("90                           75", 55, 190);
    graph.fillText("60                          45", 235, 190);
    graph.fillText("30                           15", 415, 190);

}
function drawBigBackGround(graph, high, low, topMargin, scale) {
    graph.beginPath();
    graph.fillStyle = "rgba(200,200,200, 1)";
    //     context.clearRect(0, 0, 300, 100);
    graph.fillRect(0, 0, 600, 200);
    graph.fillStyle = "rgba(0,0,0, 1)";
    graph.rect(0, 0, 600, 200);
    graph.lineWidth = 1;
    graph.strokeStyle = 'grey';
    graph.moveTo(((scaler) * 20), 0);
    graph.lineTo((((scaler) * 20)), 200);
    graph.moveTo(((scaler) * 50), 0);
    graph.lineTo((((scaler) * 50)), 200);
    graph.moveTo(((scaler) * 80), 0);
    graph.lineTo((((scaler) * 80)), 200);
    graph.moveTo(((scaler) * 109), 0);
    graph.lineTo((((scaler) * 109)), 200);
    graph.moveTo(((scaler) * 140), 0);
    graph.lineTo((((scaler) * 140)), 200);
    graph.moveTo(((scaler) * 170), 0);
    graph.lineTo((((scaler) * 170)), 200);
    graph.moveTo(0, 60);
    graph.lineTo(600, 60);
    graph.moveTo(0, 120);
    graph.lineTo(600, 120);
    graph.moveTo(0, (((high + topMargin) / scale) - (high / scale)));
    graph.lineTo(600, (((high + topMargin) / scale) - (high / scale)))
    graph.moveTo(0, (((high + topMargin) / scale) - (low / scale)));
    graph.lineTo(600, (((high + topMargin) / scale) - (low / scale)))
    graph.stroke();
    /*
    
        for (var i = 0; i < 270; i = i + 45) {
            graph.beginPath();
            graph.moveTo(i, 100);
            graph.lineTo(i, 0);
            graph.stroke();
        }*/
}

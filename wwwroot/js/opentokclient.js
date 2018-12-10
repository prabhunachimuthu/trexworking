
var apiKey;
var sessionId;
var token;

var session;
var publisher;

var PUB_VIDEO_WIDTH = 190;
var PUB_VIDEO_HEIGHT = 170;

var VIDEO_WIDTH = 305;
var VIDEO_HEIGHT = 266;
var patientName = "";
var _selfstream;
var subscribers = new Array();
var _streams = new Array();
var _connections = new Array();

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
function callmyserver(sessionid, token, patientname) {
    connect(sessionid, token, patientname);
    //  beginCall(button);
}
function connect(sessionid, token, patientname) {
    //var apikey = "45442042";
    //var sessionid = "1_MX40NTQ0MjA0Mn5-MTQ1MDg0OTc3NjY1Mn5veVJ0dDBncytpOG9maUEyN1RsdXU1VHZ-UH4";
    //var tokenid = "T1==cGFydG5lcl9pZD00NTQ0MjA0MiZzaWc9ZjFiMzNiYjQ5MjEwYjgyNDQ0Zjc0Y2VhZDI0ZTQ0MmM1MDFkMzAxNjpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTFfTVg0ME5UUTBNakEwTW41LU1UUTFNRGcwT1RjM05qWTFNbjV2ZVZKMGREQm5jeXRwT0c5bWFVRXlOMVJzZFhVMVZIWi1VSDQmY3JlYXRlX3RpbWU9MTQ1MDg0OTgwOCZub25jZT0wLjg0OTMxOTYxOTUzMTExNzEmZXhwaXJlX3RpbWU9MTQ1MzQzNzI5MCZjb25uZWN0aW9uX2RhdGE9";

    //var appid = getParameterByName("appid");

    //var spl = {
    //    SessionID: "5875875883",
    //    AppointmentID: appid,
    //    Secretkey: "396f7097fe6487cdf657fa9d8ab38d5e1609611f",
    //    Apikey: "45439242",
    //    UserType: "2"
    //}

    //$.ajax({
    //    url: '/api/GenerateSessionAndToken',
    //    dataType: "json",
    //    type: "POST",
    //    contentType: 'application/json; charset=utf-8',
    //    data: JSON.stringify(spl),
    //    async: false,
    //    processData: false,
    //    cache: false,
    //    success: function (data) {
    //        console.log(data);
    //        if (data != undefined && data.keys != undefined && data.keys.length > 0 && data.ReturnCode == 200) {
    //            patientName = data.keys[0].PatientName;
    //            connectserver("45439242", data.keys[0].SessionId, data.keys[0].Token);
    //        }
    //        else {
    //            alert(data.Message);
    //        }
    //    },
    //    error: function (xhr) {
    //        console.log(xhr);
    //    }
    //});

    patientName = patientname;
    connectserver("46201082", sessionid, token);
}

var client = function () {

    //rendering engines
    var engine = {
        ie: 0,
        edge: 0,
        gecko: 0,
        webkit: 0,
        khtml: 0,
        opera: 0,

        //complete version
        ver: null
    };

    //browsers
    var browser = {

        //browsers
        ie: 0,
        edge: 0,
        firefox: 0,
        safari: 0,
        konq: 0,
        opera: 0,
        chrome: 0,
        safari: 0,

        //specific version
        ver: null
    };


    //platform/device/OS
    var system = {
        win: false,
        mac: false,
        x11: false,

        //mobile devices
        iphone: false,
        ipod: false,
        nokiaN: false,
        winMobile: false,
        macMobile: false,

        //game systems
        wii: false,
        ps: false
    };

    //detect rendering engines/browsers
    var ua = navigator.userAgent;
    if (window.opera) {
        engine.ver = browser.ver = window.opera.version();
        engine.opera = browser.opera = parseFloat(engine.ver);
    }
    else if (/Edge\/([^;]+)/.test(ua)) { // IE11
        engine.ver = browser.ver = RegExp["$1"];
        engine.edge = browser.edge = parseFloat(engine.ver);
    } else if (/AppleWebKit\/(\S+)/.test(ua)) {
        engine.ver = RegExp["$1"];
        engine.webkit = parseFloat(engine.ver);

        //figure out if it's Chrome or Safari
        if (/Chrome\/(\S+)/.test(ua)) {
            browser.ver = RegExp["$1"];
            browser.chrome = parseFloat(browser.ver);
        } else if (/Version\/(\S+)/.test(ua)) {
            browser.ver = RegExp["$1"];
            browser.safari = parseFloat(browser.ver);
        } else {
            //approximate version
            var safariVersion = 1;
            if (engine.webkit < 100) {
                safariVersion = 1;
            } else if (engine.webkit < 312) {
                safariVersion = 1.2;
            } else if (engine.webkit < 412) {
                safariVersion = 1.3;
            } else {
                safariVersion = 2;
            }

            browser.safari = browser.ver = safariVersion;
        }
    } else if (/KHTML\/(\S+)/.test(ua) || /Konqueror\/([^;]+)/.test(ua)) {
        engine.ver = browser.ver = RegExp["$1"];
        engine.khtml = browser.konq = parseFloat(engine.ver);
    } else if (/rv:([^\)]+)\) Gecko\/\d{8}/.test(ua)) {
        engine.ver = RegExp["$1"];
        engine.gecko = parseFloat(engine.ver);

        //determine if it's Firefox
        if (/Firefox\/(\S+)/.test(ua)) {
            browser.ver = RegExp["$1"];
            browser.firefox = parseFloat(browser.ver);
        }
    } else if (/MSIE ([^;]+)/.test(ua)) { // IE <= 10
        engine.ver = browser.ver = RegExp["$1"];
        engine.ie = browser.ie = parseFloat(engine.ver);
    } else if (/Trident\/([^;]+)/.test(ua)) { // IE11
        engine.ver = RegExp["$1"];
        browser.ver = parseFloat(ua.split("rv:")[1]);
        engine.ie = parseFloat(browser.ver);
    }

    //detect browsers
    browser.ie = engine.ie;
    browser.opera = engine.opera;


    //detect platform
    var p = navigator.platform;
    system.win = p.indexOf("Win") == 0;
    system.mac = p.indexOf("Mac") == 0;
    system.x11 = (p == "X11") || (p.indexOf("Linux") == 0);

    //detect windows operating systems
    if (system.win) {
        if (/Win(?:dows )?([^do]{2})\s?(\d+\.\d+)?/.test(ua)) {
            if (RegExp["$1"] == "NT") {
                switch (RegExp["$2"]) {
                    case "5.0":
                        system.win = "2000";
                        break;
                    case "5.1":
                        system.win = "XP";
                        break;
                    case "6.0":
                        system.win = "Vista";
                        break;
                    default:
                        system.win = "NT";
                        break;
                }
            } else if (RegExp["$1"] == "9x") {
                system.win = "ME";
            } else {
                system.win = RegExp["$1"];
            }
        }
    }

    //mobile devices
    system.iphone = ua.indexOf("iPhone") > -1;
    system.ipod = ua.indexOf("iPod") > -1;
    system.nokiaN = ua.indexOf("NokiaN") > -1;
    system.winMobile = (system.win == "CE");
    system.macMobile = (system.iphone || system.ipod);

    //gaming systems
    system.wii = ua.indexOf("Wii") > -1;
    system.ps = /playstation/i.test(ua);

    //return it
    return {
        engine: engine,
        browser: browser,
        system: system
    };

}();

console.log(JSON.stringify({ browser: client.browser, engine: client.engine, system: client.system }, null, 2));
function connectserver(apikey, sessionid, tokenid) {

    apiKey = apikey;
    sessionId = sessionid;
    token = tokenid;

    OT.on("exception", exceptionHandler);

    // Un-comment the following to set automatic logging:
    OT.setLogLevel(OT.DEBUG);

    //alert(JSON.stringify({ browser: client.browser, engine: client.engine, system: client.system }, null, 2));

    if (!(OT.checkSystemRequirements())) {
        alert("You don't have the minimum requirements to run this application." + sessionId);
        //alert(OT.upgradeSystemRequirements());
    } else {
        session = OT.initSession(sessionId); // Initialize session
        session.connect(apiKey, token);
        // Add event listeners to the session
        session.on('sessionConnected', sessionConnectedHandler);
        session.on('sessionDisconnected', sessionDisconnectedHandler);
        session.on('connectionCreated', connectionCreatedHandler);
        session.on('connectionDestroyed', connectionDestroyedHandler);
        session.on('streamCreated', streamCreatedHandler);
        session.on('streamDestroyed', streamDestroyedHandler);
        session.on("signal", signalEventHandler);
    }
}

function disconnect() {
    stopPublishing();
    session.disconnect();
    hide('disconnectLink');
}

// Called when user wants to start publishing to the session
function startPublishing() {
    if (!publisher) {
        var name = patientName;// document.getElementById("txtName").value;
        var parentDiv = document.getElementById("myCamera");
        var publisherDiv = document.createElement('div'); // Create a div for the publisher to replace
        publisherDiv.setAttribute('id', 'opentok_publisher');
        parentDiv.appendChild(publisherDiv);
        var publisherProps = { width: PUB_VIDEO_WIDTH, height: PUB_VIDEO_HEIGHT, name: name };
        publisher = OT.initPublisher(apiKey, publisherDiv.id, publisherProps);  // Pass the replacement div id and properties
        session.publish(publisher);
        publisher.on("streamCreated", function (event) {  //access the self video 
            _selfstream = event.stream;
        });
    }
}

function stopPublishing() {
    if (publisher) {
        session.unpublish(publisher);
    }
    publisher = null;
}

function sessionConnectedHandler(event) {
    startPublishing();
    show('disconnectLink');
    hide('connectLink');
}

function streamCreatedHandler(event) {
    addButton(event.stream);   //add call button when a new user comes online
}

function addButton(selectedStream) {
    if (!document.getElementById("btn_" + selectedStream.streamId)) {
        var button = document.createElement("input");
        var buttonContainer = document.getElementById("onlineusers");
        button.setAttribute("id", "btn_" + selectedStream.streamId);//FF0066
        button.setAttribute("type", "button");
        button.setAttribute("style", "background-color:FF0066;height:40px;width:200px;");
        button.setAttribute("value", "Call " + selectedStream.name.toString());
        button.setAttribute("onclick", "beginCall(this)");
        buttonContainer.appendChild(button);
        //  buttonContainer.appendChild(document.createElement("br"));
        _streams[selectedStream.streamId] = selectedStream;
        hide("btn_" + selectedStream.streamId);
    }
}

function removeButton(selectedStream) {
    var btn = document.getElementById("btn_" + selectedStream.streamId)
    var buttonContainer = document.getElementById("onlineusers");
    delete _streams[selectedStream.streamId];
    if (btn) {
        buttonContainer.removeChild(btn);
    }
}

function removeAllButtons() {
    var buttonContainer = document.getElementById("onlineusers");
    buttonContainer.innerHTML = '';
}

function endCall(obj, label) {
    console.log("endcall called");
    console.log(obj.value);
    _stream = _streams[obj.id.replace("btn_", "")];
    obj.value = label;
    obj.setAttribute("onclick", "beginCall(this)");
    removeStream(_stream);
    session.signal({
        type: "endcall",
        to: _stream.connection,
        data: { streamId: _selfstream.streamId + "|" + _selfstream.name }
    },
        function (error) {
            if (error) {
                console.log("signal error: " + error.reason);
            } else {
                console.log("signal sent");
            }
        }
    );
}

function beginCall(obj) {
    console.log(obj.value);
    obj.setAttribute("onclick", "endCall(this,'" + obj.value + "')");
    obj.value = 'End Call';
    _stream = _streams[obj.id.replace("btn_", "")];
    var streamNames = _stream.name.toString();
    session.signal({
        type: "begincall",
        to: _stream.connection,
        data: { streamId: _selfstream.streamId + "|" + _selfstream.name }
    },
        function (error) {
            if (error) {
                console.log("signal error: " + error.reason);
            } else {
                console.log("signal sent: begincall:");
            }
        }
    );
}

function signalEventHandler(event) {
    if (event.type == "signal:begincall") {
        //***************************Call Begin*********************************//            
        data = event.data.streamId.toString().split('|');
        _streamId = data[0];
        _name = data[1];
        document.getElementById('acceptCallBox').style.display = 'block';
        document.getElementById('acceptCallLabel').innerHTML = 'Incomming call from ' + _name;
        //***************************Accept Call*************************************//
        document.getElementById('callAcceptButton').onclick = function () {
            document.getElementById('acceptCallBox').style.display = 'none';
            document.getElementById('acceptCallLabel').innerHTML = '';
            _btn = document.getElementById('btn_' + _streamId);
            _btn.setAttribute("onclick", "endCall(this,'" + _btn.value + "')");
            _btn.value = 'End Call';
            addStream(_streams[_streamId]);
            session.signal({
                type: "acceptcall",
                to: _streams[_streamId].connection,
                data: { callaccepted: _selfstream.streamId + "|" + _selfstream.name + "|yes" }
            },
                function (error) {
                    if (error) { console.log("signal error: " + error.reason); }
                    else { console.log("signal sent"); }
                }
            );
        }
        //***************************Accept Call*************************************//

        //***************************Reject Call*************************************//
        document.getElementById('callRejectButton').onclick = function () {
            document.getElementById('acceptCallBox').style.display = 'none';
            document.getElementById('acceptCallLabel').innerHTML = '';
            session.signal({
                type: "acceptcall",
                to: _streams[_streamId].connection,
                data: { callaccepted: _selfstream.streamId + "|" + _selfstream.name + "|no" }
            },
                function (error) {
                    if (error) { console.log("signal error: " + error.reason); }
                    else { console.log("signal sent"); }
                }
            );
        }
        //***************************Reject Call*************************************//
        //***************************Call Begin*********************************//
    }
    else if (event.type == "signal:acceptcall") {
        data = event.data.callaccepted.toString().split('|');
        _streamId = data[0];
        _name = data[1];
        _callaccepted = data[2];
        if (_callaccepted == 'yes') {
            addStream(_streams[_streamId]);
        }
        else
            if (_callaccepted == 'no') {
                alert('Call rejected by ' + _name);
                document.getElementById("btn_" + _streamId).click();
            }
    }
    else if (event.type == "signal:endcall") {
        data = event.data.streamId.toString().split("|");
        _streamId = data[0];
        removeStream(_streams[_streamId]);
        _btn = document.getElementById('btn_' + _streamId);
        _btn.setAttribute("onclick", "beginCall(this)");
        _btn.value = 'Call ' + data[1];
    }
    else if (event.type == "signal:disconnectcall") {
        window.location.href = "/Patient/PatientView/Dashboard";
        //alert("disconnect call");
    }
}

function streamDestroyedHandler(event) {
    // This signals that a stream was destroyed. Any Subscribers will automatically be removed.
    // This default behaviour can be prevented using event.preventDefault()
    removeButton(event.stream);
}

function sessionDisconnectedHandler(event) {
    // This signals that the user was disconnected from the Session. Any subscribers and publishers
    // will automatically be removed. This default behaviour can be prevented using event.preventDefault()
    alert("disconnect");
    session.off('sessionConnected', sessionConnectedHandler);
    session.off('streamCreated', streamCreatedHandler);
    session.off('streamDestroyed', streamDestroyedHandler);
    session.off('connectionCreated', connectionCreatedHandler);
    session.off("signal", signalEventHandler);
    OT.off("exception", exceptionHandler);
    session.off('sessionDisconnected', sessionDisconnectedHandler);
    publisher = null;
    removeAllButtons();
    show('connectLink');
    hide('disconnectLink');
}

function connectionDestroyedHandler(event) {
    // This signals that connections were destroyed
}

function connectionCreatedHandler(event) {
    // This signals new connections have been created.
}

/*
If you un-comment the call to OT.setLogLevel(), above, OpenTok automatically displays exception event messages.
*/
function exceptionHandler(event) {
    alert("Exception: " + event.code + "::" + event.message);
}

//--------------------------------------
//  HELPER METHODS
//--------------------------------------

function addStream(stream) {
    // Check if this is the stream that I am publishing, and if so do not publish.
    if (stream.connection.connectionId == session.connection.connectionId) {
        return;
    }
    var subscriberDiv = document.createElement('div'); // // Create a div for the subscriber to replace
    subscriberDiv.setAttribute('id', stream.streamId); // Give the replacement div the id of the stream as its id.
    document.getElementById("subscribers").appendChild(subscriberDiv);
    var subscriberProps = { subscribeToAudio: true, subscribeToVideo: true, width: VIDEO_WIDTH, height: VIDEO_HEIGHT };
    subscribers[stream.streamId] = session.subscribe(stream, subscriberDiv.id, subscriberProps);
}


function removeStream(stream) {
    session.unsubscribe(subscribers[stream.streamId]);
}
function show(id) {
    document.getElementById(id).style.display = 'block';
}

function hide(id) {
    document.getElementById(id).style.display = 'none';
}

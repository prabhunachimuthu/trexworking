var apiKey;
var sessionId;
var token;

var session;
var publisher;

var PUB_VIDEO_WIDTH = 200;
var PUB_VIDEO_HEIGHT = 180;
var button;
var VIDEO_WIDTH = 315;
var VIDEO_HEIGHT = 276;
var doctorName = "";
var appid = "";
var sessionid = "";
var uType = "";
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

function callmyserver(sessionid, token, doctorname) {
    connect(sessionid, token, doctorname);
    //  beginCall(button);
}

function connect(sessionid, token, doctorname) {
    sessionid = sessionid;
    //appid = getParameterByName("appid");
    //uType = getParameterByName("usertype");
    //sessionid = getParameterByName("sessionid");

    if (uType == 2) {
        document.getElementById('links').style.display = 'none';
    }
    //document.getElementById('links').style.display = 'none';
    //var spl = {
    //    SessionID: sessionid,
    //    AppointmentID: appid,
    //    UserType: uType
    //}
    //console.log(JSON.stringify(spl));

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
    //            if (uType == 2) {
    //                doctorName = data.keys[0].PatientName;
    //                document.getElementById("btn_offline").style.display = "none";
    //            }
    //            else {
    //                doctorName = data.keys[0].DoctorName;
    //                document.getElementById("btn_offline").style.display = "block";
    //            }
    //            connectserver(data.apikey, data.keys[0].SessionId, data.keys[0].Token);
    //        }
    //        else {
    //            alert(data.Message);
    //        }
    //    },
    //    error: function (xhr) {

    //        console.log(xhr);
    //    }
    //});
    doctorName = doctorname;
    connectserver("46201082", sessionid, token);
}

function connectserver(apikey, sessionid, tokenid) {

    apiKey = apikey;
    sessionId = sessionid;
    token = tokenid;
    OT.on("exception", exceptionHandler);

    // Un-comment the following to set automatic logging:
    OT.setLogLevel(OT.DEBUG);

    if (!(OT.checkSystemRequirements())) {
        alert("You don't have the minimum requirements to run this application.");
    } else {
        session = OT.initSession(apiKey, sessionId);
        // OT.registerScreenSharingExtension("chrome", "pfenkeaighbpcngncdngooglbingiemo");	// Initialize session
        session.connect(token);
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
    var transact = {
        SessionId: sessionId,
        Token: token,
        ExpiryTime: null,
        TherapistId: '',
        Status:2
    }

    console.log(JSON.stringify(transact));
    $.ajax({
        url: '/api/VideoCall/UpdateStatus',
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(transact),
        async: false,
        processData: false,
        cache: false,
        success: function (data) {
            console.log(data);
        },
        error: function (xhr) {
            console.log(xhr);
        }
    });
    stopPublishing();
    session.signal({
        type: "disconnectcall",
    },
        function (error) {
            if (error) {
                console.log("signal error: " + error.reason);
            } else {
                console.log("signal sent");
            }
        },
    );
    session.disconnect();
    hide('disconnectLink');
    show('connectLink');
    
    window.location.href = "/Therapist/Dashboard";
}

// Called when user wants to start publishing to the session
function startPublishing() {
    if (!publisher) {
        var name = doctorName;// document.getElementById("txtName").value;
        var parentDiv = document.getElementById("myCamera");
        //var publisherDiv = document.createElement('div'); // Create a div for the publisher to replace
        //publisherDiv.setAttribute('id', 'opentok_publisher');
        //parentDiv.appendChild(publisherDiv);
        var publisherProps = { width: PUB_VIDEO_WIDTH, height: PUB_VIDEO_HEIGHT, name: name };
        publisher = OT.initPublisher(apiKey, parentDiv, publisherProps);
        // Pass the replacement div id and properties
        //session.publish(publisher, function (error) {
        //    if (error) {
        //        alert('Could not share the screen: ' + error.message);
        //    }
        //    else
        //        alert('You can share the screen');
        //});
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
        document.getElementById("btn_offline").style.display = "none";
        button = document.createElement("input");
        var buttonContainer = document.getElementById("onlineusers");
        button.setAttribute("id", "btn_" + selectedStream.streamId);
        button.setAttribute("type", "button");
        if (uType == 1) {
            button.setAttribute("class", "pubbuttonbg");
        }
        if (uType == 2) {
            button.setAttribute("class", "subbuttonbg");
        }
        button.setAttribute("value", "Call " + selectedStream.name.toString());
        button.setAttribute("onclick", "beginCall(this)");
        buttonContainer.appendChild(button);
        // buttonContainer.appendChild(document.createElement("br"));
        _streams[selectedStream.streamId] = selectedStream;

    }
}

function removeButton(selectedStream) {
    var btn = document.getElementById("btn_" + selectedStream.streamId)
    var buttonContainer = document.getElementById("onlineusers");
    delete _streams[selectedStream.streamId];
    if (btn) {
        buttonContainer.removeChild(btn);
        document.getElementById("btn_offline").style.display = "block";
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
        },
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
            //OT.checkScreenSharingCapability(function (response) {
            //    if (!response.supported || response.extensionRegistered === false) {
            //        alert("does not support screen sharing");
            //    } else if (response.extensionInstalled === false) {
            //        alert("response.extensionRequired extension");
            //    } else {
            //        alert("support screen sharing");
            //    }
            //});
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
        _btn = document.getElementById('btn_' + _streamId);
        _btn.setAttribute("onclick", "beginCall(this)");
        _btn.value = 'Call ' + data[1];
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
    var subscribDiv = document.getElementById("subscribers");
    if (subscribDiv == null) {
        var subscriberDiv = document.createElement('div'); // Create a div for the subscriber to replace
        subscriberDiv.setAttribute('id', "subscribers"); // Give the replacement div the id of the stream as its id.
        subscriberDiv.setAttribute('class', "subscribersContainer");
        document.getElementById("mypage").appendChild(subscriberDiv);
    }
    subscribDiv = document.getElementById("subscribers");
    //alert(document.getElementById("subscribers"));
    //var subscriberDiv1 = document.createElement('div'); // Create a div for the subscriber to replace
    //subscriberDiv1.setAttribute('id', stream.streamId); // Give the replacement div the id of the stream as its id.
    //document.getElementById("subscribers").appendChild(subscriberDiv1);
    var subscriberProps = { subscribeToAudio: true, subscribeToVideo: true, audioVolume: 100, width: VIDEO_WIDTH, height: VIDEO_HEIGHT };
    subscribers[stream.streamId] = session.subscribe(stream, subscribDiv, subscriberProps);
    console.log(subscribers);
    $(subscribDiv).css({ position: "relative" });
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

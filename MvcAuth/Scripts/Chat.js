/// <reference path="jquery-1.12.3.min.js" />
/// <reference path="jquery.signalr-2.4.1.min.js" />

$(function ()
{
   
  
    //Connection To Class Called ChatHub
    var chat = $.connection.chatHub;
    chat.client.pushMessage = function (name, message) {
        $("#discuss").append("<li><strong>" + name + "</strong> says : <p>" + message + "</p></li>")
    }
   
   
    //Get Username and save to hidden input text
    $("#txtName").val(prompt("Please Enter Your name",""))

    //Set Focus on input text message
    $("#txtMessage").focus();
    //Recieve all message from all clients
   
    //Connection to hub is successfully 
    $.connection.hub.start().done(function () {
        $("#btnSend").click(function () {
            var name = $("#txtName").val();
            var message = $("#txtMessage").val();
            //call send message method at chat hub 
            chat.server.sendMessage(name, message);

            $("#txtMessage").val('').focus(); 
        });

    })
})
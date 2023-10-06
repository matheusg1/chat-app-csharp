"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
$("#send").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&").replace(/</g, "<").replace(/>/g, ">");
    var li = $("<li></li>").text(user + ": " + msg);
    li.addClass("list-group-item");
    $("#messagesList").append(li);
});

connection.start().then(function () {
    connection.invoke("GetConnectionId").then(function (id){
        document.getElementById("connectionId").innerText = id;
    })
    $("#send").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$("#send").on("click", function (event) {
    var user = $("#usuario").val();
    var message = $("#mensagem").val();
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

$("#sendPrivate").on("click", function (event) {
    let fromUser = $("#usuario").val();
    let toUser = $("#usuarioDestinatario").val();
    let message = $("#mensagem").val();
    let admin = $("#admin").val();

    connection.invoke("SendToUser", fromUser, toUser, message)
        .then(function () {
            connection.invoke("SendToUser", fromUser, admin, message).catch(function (err) {
                console.error(err.toString());
            });
        })
        .catch(function (err) {
            console.error(err.toString());
        });

    event.preventDefault();
});
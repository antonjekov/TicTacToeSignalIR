let connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/hub/chat")
        .build();

document.querySelector("#sendButton").addEventListener('click', (event) => {
    event.preventDefault();
    let messageInputValue = document.querySelector("#messageInput");
    connection.invoke("Send", messageInputValue.value);
    messageInputValue.value = '';
})

connection.on("NewMessage", (response) => {
    let divMessage = document.createElement('div');
    divMessage.textContent = `[${response.user}] ${response.text}`;
    document.querySelector('#messagesList').appendChild(divMessage);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
})
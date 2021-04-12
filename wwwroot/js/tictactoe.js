let connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/hub/tictactoe")
        .build();

connection.on("Message", (response) => {
    let messageElement = document.getElementById("Message");
    messageElement.innerHTML = response;
});

connection.on("GameStart", () => {    
    document.getElementById("game-screen").style.display = "block";
    document.getElementById("gaveUp").classList.replace("d-none", "d-block");
})

connection.on("ChangeGameBoard", (response) => {
    for (let i = 0; i < 3; i++) {
        for (let j = 0; j < 3; j++) {            
            document.getElementById(`cell${i}${j}`).textContent = response[`${i}${j}`];
        }
    }
});

connection.on("Finish", (responce) => {
    document.getElementById("playGame").classList.replace("d-none", "d-block");
    document.getElementById("giveUp").classList.replace("d-block","d-none");
})

connection.start().catch(function (err) {
    return console.error(err.toString());
})

document.querySelectorAll(".box").forEach(function (element) {
    let idInfo = element.id;
    let x = parseInt(idInfo.charAt(4));
    let y = parseInt(idInfo.charAt(5));

    element.addEventListener("click", function () {
        connection.invoke("Move", x, y);
    });
});

document.getElementById("giveUp").addEventListener("click", (event) => {
    event.preventDefault();
    location.reload();    
})

document.querySelector("#playGame").addEventListener("click", (event) => {
    event.preventDefault();
    event.target.classList.replace("d-block", "d-none");
    document.getElementById("game-screen").style.display = "none";
    document.getElementById("giveUp").classList.replace("d-none", "d-block");
    connection.invoke("Play");
})



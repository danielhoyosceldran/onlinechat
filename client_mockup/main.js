const socket = new WebSocket("ws://localhost:5000");
var pile = [];

socket.onopen = function(event) {
  console.log("Connexió oberta");
};

socket.onmessage = function(event) {
  console.log("Missatge del servidor: " + event.data);

  var container = document.getElementById("messageContainer");

  pile.push(`<p class="message">${event.data}</p>`);
  updateMessageContainer();
  
  setTimeout(() => {
    pile.shift();
    updateMessageContainer();
  }, 3000);
};

function updateMessageContainer() {
  var container = document.getElementById("messageContainer");
  container.innerHTML = ``;
  pile.forEach(element => {
    container.innerHTML += element;
  });
}

socket.onclose = function(event) {
  console.log("Connexió tancada");
};

function act(event) {
  event.preventDefault(); // per a no recarregar la pàgina la fer submit

  var input = document.getElementById("fmessage");
  socket.send(input.value);
  input.value = "";
  updateCounter();
}

function updateCounter() {
  var input = document.getElementById("fmessage");
  var messageLenght = input.value.length;
  var counter = document.getElementById("counter");
  counter.innerText = messageLenght;
}
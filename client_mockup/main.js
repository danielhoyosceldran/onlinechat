const socket = new WebSocket("ws://192.168.1.7:5000");
//const socket = new WebSocket("https://257d-37-116-8-119.ngrok-free.app ");
var pile = [];

socket.onopen = function(event) {
  console.log("Connexió oberta");
  var onlineBanner = document.getElementById("onlineBanner");
  onlineBanner.innerText = "online";
  onlineBanner.classList.remove("offline");
  onlineBanner.classList.add("online");
};

socket.onmessage = function(event) {
  console.log("Missatge del servidor: " + event.data);

  var container = document.getElementById("messageContainer");

  pile.push(`<p class="message">${event.data}</p>`);
  updateMessageContainer();
  
  setTimeout(() => {
    pile.shift();
    updateMessageContainer();
  }, 10000);
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
  onlineBanner.innerText = "Reload page to go online";
  onlineBanner.classList.remove("online");
  onlineBanner.classList.add("offline");
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
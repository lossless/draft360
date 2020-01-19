/* global dialogPolyfill, isTouch, QRCode */
function getDialog() {
  var dialog = document.getElementById("modal");
  if (!dialog.showModal) {
    dialogPolyfill.registerDialog(dialog);
  }
  dialog.style.top = "20vh";
  var buttonsElem = document.getElementById("modal-buttons");
  buttonsElem.style.maxHeight = "40vh";
  buttonsElem.style["overflow-y"] = "scroll";
  return dialog;
}

function showTextDialog(text) {
  var dialog = getDialog();
  dialog.showModal();
  document.getElementById("modal-text").innerHTML = text;
}

function showDialog(text, buttons) {
  if(!isTouch()) {
    document.getElementById("camera").components
      .mylookcontrols.movementMode = true;
  }
  var dialog = getDialog();
  dialog.showModal();
  document.getElementById("modal-text").innerHTML = text;
  var buttonsElem = document.getElementById("modal-buttons");
  buttonsElem.innerHTML = "";
  buttons.push(["Close", closeDialog]);
  buttons.forEach(([text, action]) => {
    var button = document.createElement("button");
    button.setAttribute("type", "button");
    button.setAttribute("class", "mdl-button");
    button.innerText = text;
    button.onclick = action;
    buttonsElem.appendChild(button);
  });
}

function showQRDialog(url) {
  if(!isTouch()) {
    document.getElementById("camera").components
      .mylookcontrols.movementMode = true;
  }
  var dialog = getDialog();
  dialog.showModal();
  document.getElementById("modal-text").innerText = "Scan QR Code to Share the Project";
  
  //var imageElem = document.getElementById("qr-code");
  // var img = document.createElement('img'); 
  // img.src = 'https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2FTeam009QRCode.png?v=1579278312151'; 
  // imageElem.appendChild(img);
  
  // the qr code has to be resized to be smaller so that the entire dialog
  // including the close button is visible on a laptop.
  // use another value if you want, but 256x256 is too big.
   var qrcode = new QRCode(document.getElementById("qr-code"), {
    text: url,
    width: 180,
    height: 180
  });
  
  var buttonsElem = document.getElementById("modal-buttons");
  
  var copyURLbutton = document.createElement("button");
  copyURLbutton.setAttribute("class", "mdl-button");
  copyURLbutton.setAttribute("type", "button");
  copyURLbutton.innerText = "Copy link";
  copyURLbutton.onclick = copyLink.bind(this, url);
  buttonsElem.appendChild(copyURLbutton);
  
  var closeButton = document.createElement("button");
  closeButton.setAttribute("class", "mdl-button");
  closeButton.setAttribute("type", "button");
  closeButton.innerText = "Close";
  closeButton.onclick = closeDialog;
  buttonsElem.appendChild(closeButton);
}

function closeDialog() {
  var imageElem = document.getElementById("qr-code");
  imageElem.innerHTML = "";
  var buttonsElem = document.getElementById("modal-buttons");
  buttonsElem.innerHTML = "";
  getDialog().close();
}

function copyLink(url) {
  navigator.clipboard.writeText(url)/* ¯\_(ツ)_/¯ .then(function() {
    // clipboard successfully set
  }, function() {
    // clipboard write failed
  });
  */
  //getDialog().close();
  //closeDialog();
}
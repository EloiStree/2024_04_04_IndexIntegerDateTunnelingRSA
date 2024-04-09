
// ==UserScript==
// @name         Integer Games Test RSA Sign
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://integergames.be
// @icon         https://www.google.com/s2/favicons?sz=64&domain=tampermonkey.net
// @require      http://code.jquery.com/jquery-1.8.3.min.js
// @require      https://cdnjs.cloudflare.com/ajax/libs/jsencrypt/3.3.2/jsencrypt.min.js
// @require      https://cdnjs.cloudflare.com/ajax/libs/crypto-js/4.2.0/crypto-js.min.js
// @grant        none

// ==/UserScript==
//TO LOOK LATER
//https://github.com/travist/jsencrypt
(function() {
    'use strict';


// Example usage
const privateKey = `-----BEGIN RSA PRIVATE KEY-----
MIICXQIBAAKBgQDlOJu6TyygqxfWT7eLtGDwajtNFOb9I5XRb6khyfD1Yt3YiCgQ
WMNW649887VGJiGr/L5i2osbl8C9+WJTeucF+S76xFxdU6jE0NQ+Z+zEdhUTooNR
aY5nZiu5PgDB0ED/ZKBUSLKL7eibMxZtMlUDHjm4gwQco1KRMDSmXSMkDwIDAQAB
AoGAfY9LpnuWK5Bs50UVep5c93SJdUi82u7yMx4iHFMc/Z2hfenfYEzu+57fI4fv
xTQ//5DbzRR/XKb8ulNv6+CHyPF31xk7YOBfkGI8qjLoq06V+FyBfDSwL8KbLyeH
m7KUZnLNQbk8yGLzB3iYKkRHlmUanQGaNMIJziWOkN+N9dECQQD0ONYRNZeuM8zd
8XJTSdcIX4a3gy3GGCJxOzv16XHxD03GW6UNLmfPwenKu+cdrQeaqEixrCejXdAF
z/7+BSMpAkEA8EaSOeP5Xr3ZrbiKzi6TGMwHMvC7HdJxaBJbVRfApFrE0/mPwmP5
rN7QwjrMY+0+AbXcm8mRQyQ1+IGEembsdwJBAN6az8Rv7QnD/YBvi52POIlRSSIM
V7SwWvSK4WSMnGb1ZBbhgdg57DXaspcwHsFV7hByQ5BvMtIduHcT14ECfcECQATe
aTgjFnqE/lQ22Rk0eGaYO80cc643BXVGafNfd9fcvwBMnk0iGX0XRsOozVt5Azil
psLBYuApa66NcVHJpCECQQDTjI2AQhFc1yRnCU/YgDnSpJVm1nASoRUnU8Jfm3Oz
uku7JUXcVpt08DFSceCEX9unCuMcT72rAQlLpdZir876
-----END RSA PRIVATE KEY-----`;


    const publicKey = `-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDlOJu6TyygqxfWT7eLtGDwajtN
FOb9I5XRb6khyfD1Yt3YiCgQWMNW649887VGJiGr/L5i2osbl8C9+WJTeucF+S76
xFxdU6jE0NQ+Z+zEdhUTooNRaY5nZiu5PgDB0ED/ZKBUSLKL7eibMxZtMlUDHjm4
gwQco1KRMDSmXSMkDwIDAQAB
-----END PUBLIC KEY-----`;

    //console.log(publicKey);
    //console.log(privateKey);





                console.log('Code Start ');
var socket = new WebSocket('ws://localhost:3615');
var isConnectionValide=false;


   function PushMessageToServerRandomInteger(){
       if(!isConnectionValide)
           return;
       const randomInt = Math.floor(Math.random() * 1000000000) + 1;
       PushMessageToServerInteger(randomInt)

   }
    function PushMessageToServerInteger(integer){
       if(!isConnectionValide)
           return;

       //socket.send("i|"+integer);

         var value =integer;
        // Get the current UTC time in milliseconds
        const currentTimeMillis = Date.now();

        // Convert to an unsigned long (assuming 64-bit)
        const ulongVar = BigInt(currentTimeMillis);

        // Create a byte array of length 12
        const byteArray = new Uint8Array(12);
        // Set the first 4 bytes of the array from the value in little-endian format
        byteArray[0] = value & 0xFF;
        byteArray[1] = (value >> 8) & 0xFF;
        byteArray[2] = (value >> 16) & 0xFF;
        byteArray[3] = (value >> 24) & 0xFF;

        // Set the next 8 bytes of the array from ulongVar in little-endian format
        const view = new DataView(byteArray.buffer);
        view.setBigUint64(4, ulongVar, true);
        socket.send(byteArray);
   }



// Event listener for when the connection is established
socket.addEventListener('open', () => {
    console.log('WebSocket connection established');

    // Send a message to the server
    socket.send('Hello '+publicKey);
});

// Event listener for incoming messages
socket.addEventListener('message', (event) => {
    console.log('Received message from server:', event.data);
      if (event.data.startsWith("SIGNIN:")) {
          var toReturn = event.data.substring(7);
          var sign = new JSEncrypt();
          sign.setPrivateKey(privateKey);
          var signature = sign.sign(toReturn, CryptoJS.SHA256, "sha256");
          console.log("Received:"+toReturn);
          console.log("Signed:"+signature);
          socket.send("SIGNED:"+signature);
      }
    if (event.data.startsWith("RSA:Verified")) {
        isConnectionValide=true
        console.log("Connection establish :)");
    }
});

// Event listener for when the connection is closed
socket.addEventListener('close', () => {
    console.log('WebSocket connection closed');
    isConnectionValide=false

});

// Event listener for errors
socket.addEventListener('error', (error) => {
    console.error('WebSocket error:', error);
});


                console.log('Code Websocket end');
    function Test(){



            var message= "Hello World !";

            var sign = new JSEncrypt();
            sign.setPrivateKey(privateKey);
            var signature = sign.sign(message, CryptoJS.SHA256, "sha256");

            var verify = new JSEncrypt();
            verify.setPublicKey(publicKey);
            var verified = verify.verify(message, signature, CryptoJS.SHA256);

            // Now a simple check to see if the round-trip worked.
            if (verified) {
                console.log('ok '+verified+' '+signature);
            }
            else {
                console.log('Wrong '+verified+' '+signature);
            }
        }

    setInterval(PushMessageToServerRandomInteger, 3000);
    setTimeout(Test, 1000);
                console.log('Code end reach');

})();
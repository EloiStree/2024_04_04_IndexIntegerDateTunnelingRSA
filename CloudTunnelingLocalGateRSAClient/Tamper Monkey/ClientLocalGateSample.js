
// ==UserScript==
// @name         Integer Games Local WS
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://integergames.be
// @icon         https://www.google.com/s2/favicons?sz=64&domain=tampermonkey.net
// @require      http://code.jquery.com/jquery-1.8.3.min.js

// @grant        none

// ==/UserScript==
//TO LOOK LATER
//https://github.com/travist/jsencrypt
(function() {
    'use strict';


console.log('Code Start ');
var socket ;//= new WebSocket('ws://localhost:5012');
var isConnectionValide=false;


   function PushMessageToServerRandomInteger(){
       if(!isConnectionValide){
           return;
       }
       const randomInt = Math.floor(Math.random() * 1000000000) + 1;
       PushMessageToServerInteger(randomInt)

   }
    function PushMessageToServerInteger(integer){
       if(!isConnectionValide){return;}

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




var server_is_offline=false;
function ReconnectIfOffline(){

    if (socket && socket.readyState === WebSocket.OPEN) {
    }
    else{
        isConnectionValide=false
        try{
            socket = new WebSocket('ws://localhost:5012');
            // Event listener for when the connection is established
            socket.addEventListener('open', () => {
                console.log('WebSocket connection established');
                isConnectionValide=true
            });

            // Event listener for incoming messages
            socket.addEventListener('message', (event) => {
                console.log('Received message from server:', event.data);

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
            server_is_offline=false;
            console.log("Server Online");
        }catch(Exception){
            server_is_offline=true;
        }
    }
}


    setInterval(PushMessageToServerRandomInteger, 3000);
    setInterval(ReconnectIfOffline, 1000);
      console.log('Code end reach');

})();
/**
 * @module cpp
 * @description This module is not a real module, it is a simple object that exposes C++ functions.
 * To add new functions you also need to add them to the `cpp.js` file in the `public` folder.
*/
export default {
    connect: function(adress, port) {
        return connect(adress, parseInt(port)) 
    },
    host: function(port) {
        return host(parseInt(port))
    },
    sendMessage: function(username, message) {
        return send_message(username, message) 
    },
}
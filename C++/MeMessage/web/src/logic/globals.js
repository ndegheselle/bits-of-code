import { notification, connection, messages } from "./store";

export function init() {
    // From C++ to JS
    globalThis.notification = function (message, type)
    {
        notification.notify(message, type);
    }

    globalThis.connected = function(error)
    {
        if (error)
            notification.notify("Error during the connection.", "is-danger");
        else
            notification.notify("Connection successfull.", "is-success");
        connection.connected(error);
    }

    globalThis.receivedMessage = function(username, message)
    {
        messages.received(username, message);
    }
}
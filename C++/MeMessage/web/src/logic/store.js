import { writable } from 'svelte/store';
import cpp from "./cpp";

function createNotificationStore()
{
    const delay = 5000;
    let counter = 0;
    const { set, update, subscribe } = writable(null);

    function notify(message, type) {
        counter++;
        update(notification => {
            return {
                message: message,
                type: type
            };
        });

        setTimeout(() => {
            hide();
        }, delay);
    }

    function hide() {
        counter--;
        if (counter > 0)
            return;

        update(notification => {
            return null;
        });
    }

    return {
        subscribe,
        notify,
        hide
    }
}

function createConnectionStore() {
    const { set, update, subscribe } = writable({
        username: "",
        adress: "",
        status: {
            pending: false,
            success: false,
            error: ""
        }
    });

    function connect(adress, username) {
        update(connection => {
            return {
                username: username,
                adress: adress,
                status: {
                    pending: true,
                    success: false,
                    error: ""
                }
            };
        });
        cpp.connect(adress.split(":")[0], adress.split(":")[1] || 8080);
    }

    function host(adress, username) {
        update(connection => {
            return {
                username: username,
                adress: adress,
                status: {
                    pending: true,
                    success: false,
                    error: ""
                }
            };
        });
        cpp.host(adress.split(":")[1] || 8080);
    }

    function connected(error) {
        update(connection => {
            return {
                username: connection.username,
                adress: connection.adress,
                status: {
                    pending: false,
                    success: !error,
                    error: error
                }
            };
        });
    }

    return {
        subscribe,
        connect,
        host,
        connected
    };
}

function createMessagesStore() {
    const { set, update, subscribe } = writable([]);

    function send(username, message) {
        cpp.sendMessage(username, message);
        received(username, message);
    }

    function received(username, message) {
        update(messages => {
            return [
                {
                    content: message,
                    username: username,
                    date: new Date()
                },
                ...messages,
            ];
        });
    }

    return {
        subscribe,
        send,
        received
    };
}

export const notification = createNotificationStore();
export const connection = createConnectionStore();
export const messages = createMessagesStore();
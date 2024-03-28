import { writable } from 'svelte/store';

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
        // Call C++ function to connect to server
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
    }

    function host(adress, username) {
        // Call C++ function to host  to server
        update(connection => {
            return {
                username: username,
                adress: adress,
                status: {
                    pending: true,
                    success: true,
                    error: ""
                }
            };
        });
    }

    return {
        subscribe,
        connect,
        host
    };
}

function createMessagesStore() {
    const { set, update, subscribe } = writable([]);

    function send(message, username) {
        // Call C++ function to send message
        update(messages => {
            return [
                ...messages,
                {
                    content: message,
                    username: username,
                    date: new Date()
                }
            ];
        });
    }

    return {
        subscribe,
        send
    };
}

export const notification = createNotificationStore();
export const connection = createConnectionStore();
export const messages = createMessagesStore();
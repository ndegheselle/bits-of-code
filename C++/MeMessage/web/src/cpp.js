import { notification, connection, messages  } from "./lib/store";

window.store = {
    notification,
    connection,
    messages
}

/**
 * @module cpp
 * @description This module is not a real module, it is a simple object that exposes C++ functions.
 * To add new functions you also need to add them to the `cpp.js` file in the `public` folder.
*/
export default {
    test: () => Test()
}
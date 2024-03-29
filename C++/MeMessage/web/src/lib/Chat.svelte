<script>
    import { messages, connection } from "./store.js";

    function formatDate(date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? "pm" : "am";
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? "0" + minutes : minutes;
        var strTime = hours + ":" + minutes + " " + ampm;
        return (
            date.getMonth() +
            1 +
            "/" +
            date.getDate() +
            "/" +
            date.getFullYear() +
            " " +
            strTime
        );
    }

    function handleKeyDown(event) {
        if (event.key === "Enter") {
            send();
        }
    }

    function isAutor(username) {
        return username === $connection.username;
    }

    function send() {
        if (!messageContent) return;
        messages.send(messageContent, $connection.username);
        messageContent = "";
    }

    let messageContent = "";
</script>

<div class="chat">
    <div class="messages">
        {#each $messages as message}
            <div
                class="chat-message m-1"
                class:is-autor={isAutor(message.username)}
            >
                <p
                    class="body {isAutor(message.username)
                        ? 'has-background-primary-light'
                        : 'has-background-light'}"
                >
                    {message.content}
                </p>
                <div class="content is-small has-text-grey">
                    <span class="mr-1">{message.username}</span>
                    <span>{formatDate(message.date)}</span>
                </div>
            </div>
        {/each}
    </div>

    <div class="field has-addons m-1">
        <div class="control is-expanded">
            <input
                class="input"
                type="text"
                placeholder="..."
                bind:value={messageContent}
                on:keydown={handleKeyDown}
            />
        </div>
        <div class="control">
            <button class="button" on:click={send}> Send </button>
        </div>
    </div>
</div>

<style>
    .chat {
        display: flex;
        flex-direction: column;
        height: 100%;
    }

    .messages {
        margin-top: auto;
        overflow-y: auto;
        display: flex;
        flex-direction: column-reverse;
    }

    .chat-message .body {
        display: inline-block;
        padding: 0.4rem 0.8rem;
        border-radius: 0.6rem;
    }
    .chat-message .content {
        padding-left: 0.4rem;
    }

    .chat-message.is-autor {
        text-align: right;
    }
</style>

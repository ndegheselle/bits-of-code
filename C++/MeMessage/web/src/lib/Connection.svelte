<script>
    import { connection, notification } from "./store";
    import cpp from "../cpp";
    import { onMount } from 'svelte';

    function connect() {
        // connection.connect(adress, username);
    }

    function host() {
        cpp.test();
        // connection.host(adress, username);
    }

    let adress = "test";
    let username = "";
</script>

<div class="columns is-centered">
    <div class="column is-half">
        <div class="box">
            <p>{username}</p>
            <div class="field">
                <p class="label">Adress</p>
                <div class="control">
                    <input
                        class="input"
                        type="text"
                        placeholder="127.0.0.1:8080"
                        bind:value={adress}
                    />
                </div>
                {#if $connection.status.error}
                <p class="help is-danger">{$connection.status.error}</p>
                {/if}
            </div>
            <div class="field">
                <label class="label">Username</label>
                <div class="control">
                    <input
                        class="input"
                        type="text"
                        placeholder="username"
                        bind:value={username}
                    />
                </div>
            </div>
            <div class="field is-grouped">
                <div class="control">
                    <button
                        class="button is-link"
                        class:is-loading={$connection.status.pending}
                        on:click={connect}>Connect</button
                    >
                </div>
                <div class="control ml-2">
                    <button
                        class="button is-link is-light"
                        class:is-loading={$connection.status.pending}
                        on:click={host}>Host</button
                    >
                </div>
            </div>
        </div>
    </div>
</div>

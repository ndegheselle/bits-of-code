<script>
    import { connection } from "../logic/store";

    function connect() {
        connection.connect(adress, username);
    }

    function host() {
        connection.host(adress, username);
    }

    let adress = "127.0.0.1:8080";
    let username = "";
</script>

<div class="columns is-centered">
    <div class="column is-half">
        <div class="box">
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
                        class="button is-primary"
                        class:is-loading={$connection.status.pending}
                        on:click={connect}>Connect</button
                    >
                </div>
                <div class="control ml-2">
                    <button
                        class="button is-primary is-light"
                        class:is-loading={$connection.status.pending}
                        on:click={host}>Host</button
                    >
                </div>
            </div>
        </div>
    </div>
</div>

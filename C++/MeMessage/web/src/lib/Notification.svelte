<script>
    import {notification} from "../logic/store";
</script>

{#if $notification}
<div class="notification {$notification.type}">
    <button class="delete" on:click={notification.hide}></button>
    {$notification.message}
</div>
{/if}

<style>
    .notification {
        position: fixed;
        top: 0;
        right: 0;
        left: 0;
        z-index: 1000;
        margin: 0.2rem;
    }

    .notification .delete {
        right: 1rem;
    }
</style>
<script>
  import "bulma/css/bulma.min.css";
  import Notification from "./lib/Notification.svelte";
  import Connection from "./lib/Connection.svelte";
  import Chat from "./lib/Chat.svelte";

  import {notification, connection} from "./lib/store.js";
</script>

<Notification />
<main>
  {#if !$connection.status.success}
    <Connection />
    {:else}
    <Chat />
  {/if}
</main>
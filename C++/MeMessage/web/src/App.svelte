<script>
  import "bulma/css/bulma.min.css";
  import Notification from "./lib/Notification.svelte";
  import Connection from "./lib/Connection.svelte";
  import Chat from "./lib/Chat.svelte";

  import { init } from "./logic/globals.js";
  import { connection } from "./logic/store.js";
  import { onMount } from "svelte";

  
  onMount(() => {
    init();
  });
</script>

<Notification />
<main>
  {#if !$connection.status.success}
    <Connection />
    {:else}
    <Chat />
  {/if}
</main>
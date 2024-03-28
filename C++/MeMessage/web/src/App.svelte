<script>
  import "bulma/css/bulma.min.css";
  import Notification from "./lib/Notification.svelte";
  import Connection from "./lib/Connection.svelte";

  import {notification, connection} from "./lib/store.js";
</script>

<Notification />
<main class="container">
  <Connection />
</main>

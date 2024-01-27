<template>
  <q-page class="flex-container">
    <div class="row">
      <pre class="text-white ascii">
   _         _                 _          _        ___           _                  
  /_\   __ _| |__   __ _ _ __ (_)_ __ ___( )__    / __\_ _ _ __ | |_ __ _ ___ _   _ 
 //_\\ / _` | '_ \ / _` | '_ \| | '_ ` _ \/ __|  / _\/ _` | '_ \| __/ _` / __| | | |
/  _  \ (_| | | | | (_| | | | | | | | | | \__ \ / / | (_| | | | | || (_| \__ \ |_| |
\_/ \_/\__, |_| |_|\__,_|_| |_|_|_| |_| |_|___/ \/   \__,_|_| |_|\__\__,_|___/\__, |
       |___/                                                                  |___/ 
                                                                                    
</pre>
    </div>
    <div class="row">
      <h3 class="text-white">Welcome to the NADCL fantasy draft</h3>
    </div>
    <div class="row">
      <p class="text-white">
        The about page will detail the scoring/rules, and the fantasy page will allow you to set up your draft for the
        upcoming league.
      </p>
    </div>
  </q-page>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'IndexPage',
});
</script>

<style scoped>
.ascii {
  width: 588px;
}

.flex-container {
  display: flex;
  flex-flow: row wrap;
  max-width: 100%;
  padding-left: 20px;
  padding-right: 20px;
}

.flex-break {
  flex: 1 0 100% !important
}

.row {
  width: 100%;
}

.row .flex-break {
  height: 0 !important
}

.column .flex-break {
  width: 0 !important
}
</style>
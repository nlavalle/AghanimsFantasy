<template>
    <q-dialog class="alert-dialog">
        <q-card dark>
            <q-card-section>
                <span class="text-h6">Success!</span>
            </q-card-section>

            <q-card-section>
                <p>Your Fantasy Draft was updated.</p>
            </q-card-section>

            <q-card-actions align="right">
                <q-btn flat label="OK" color="white" v-close-popup></q-btn>
            </q-card-actions>
        </q-card>
    </q-dialog>
</template>
<style>
.alert-dialog {
    background-color:#192038;
    z-index:1000;
}
</style>
<template>
    <q-dialog class="alert-dialog">
        <q-card>
            <q-card-section>
                <span class="text-h6">Success!</span>
            </q-card-section>

            <q-card-section>
                <p>Your Fantasy Draft was updated.</p>
            </q-card-section>

            <q-card-actions align="right">
                <q-btn flat label="OK" color="white" v-close-popup @click="onOKClick"></q-btn>
            </q-card-actions>
        </q-card>
    </q-dialog>
</template>
<script>
export default {
    emits: ['ok'],
    methods: {
        onOKClick() {
            // on OK, it is REQUIRED to
            // emit "ok" event (with optional payload)
            // before hiding the QDialog
            this.$emit('ok')
            // or with payload: this.$emit('ok', { ... })

            // // then hiding dialog
            // this.hide()
        },
    }
}
</script>
<style>
.alert-dialog {
    background-color: #192038;
    z-index: 1000;
}
</style>
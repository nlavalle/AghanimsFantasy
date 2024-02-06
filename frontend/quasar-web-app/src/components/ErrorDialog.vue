<template>
    <q-dialog class="alert-dialog">
        <q-card dark>
            <q-card-section>
                <span class="text-h6">Error</span>
            </q-card-section>

            <q-card-section>
                <p>There was an issue updating the fantasy draft. Error details: {{ error }}</p>
            </q-card-section>

            <q-card-actions align="right">
                <q-btn flat label="OK" color="white" v-close-popup @click="onOKClick"></q-btn>
            </q-card-actions>
        </q-card>
    </q-dialog>
</template>
<script>
export default {
    props: {
        error: {
        }
    },
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
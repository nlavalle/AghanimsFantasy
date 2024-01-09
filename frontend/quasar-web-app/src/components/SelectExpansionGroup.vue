<template>
    <q-select filled :modelValue="modelValue" @update:model-value="updateSelectedOption" :options="selectOptions" option-label="name" dark
        :label="selectLabel ?? ''" color="teal" clearable options-selected-class="text-deep-orange-8">
        <template v-slot:option="scope">
            <q-expansion-item expand-separator :default-opened="hasChild(scope)"
                header-class="text-weight-bold" :label="scope.opt.label">
                <template v-for="child in scope.opt.options" :key="child.label">
                    <q-item clickable v-ripple v-close-popup @click="changeOption(child)"
                        :class="{ 'bg-light-blue-10': modelValue === child }">
                        <q-item-section>
                            <q-item-label class="q-ml-md">{{ child.name }}</q-item-label>
                        </q-item-section>
                    </q-item>
                </template>
            </q-expansion-item>
        </template>
    </q-select>
</template>

<script>

export default {
    name: 'SelectExpansionGroup',
    props: {
        modelValue: {
            type: Object,
        },
        defaultOpened: {
            type: Boolean,
            required: false,
        },
        selectLabel: {
            type: String,
            required: false,
        },
        selectOptions: {
            type: Array,
            required: true,
        },
    },
    emits: ['update:modelValue'],
    methods: {
        hasChild(scope) {
            return scope.opt.options.some(c => c === this.modelValue)
        },
        changeOption(option) {
            this.$emit('update:modelValue', option);
        },
        updateSelectedOption(event) {
            this.$emit('update:modelValue', event);
        }
    }
};
</script>
<style>
.login-container {
    box-sizing: border-box;
    border: 2px solid gray;
    border-radius: 10px;
    margin: auto;
    align-items: center;
}

.log-btn {
    margin: 2px;
    border-radius: 8px;
}

.welcome {
    color: white;
    margin: 5px;
}
</style>
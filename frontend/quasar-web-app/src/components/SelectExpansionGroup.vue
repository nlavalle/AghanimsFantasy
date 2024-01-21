<template>
    <q-select ref="expandSelect" use-input filled input-debounce="500" :modelValue="modelValue"
        @update:model-value="updateSelectedOption" :options="options" @filter="filterFn" option-label="name" dark
        :label="selectLabel ?? ''" color="teal" clearable>
        <template v-slot:option="scope">
            <q-expansion-item expand-separator :default-opened="true" header-class="text-weight-bold"
                :label="scope.opt.label">
                <template v-for="child in scope.opt.options" :key="child.label">
                    <q-item clickable @click="changeOption(child)">
                        <q-item-section>
                            <q-item-label class="q-ml-md">{{ child.name }}</q-item-label>
                        </q-item-section>
                    </q-item>
                </template>
            </q-expansion-item>
        </template>
    </q-select>
    <!-- <q-select ref="expandSelect" use-input filled input-debounce="500" :modelValue="modelValue"
        @update:model-value="updateSelectedOption" :options="options" remove-selected @filter="filterFn" option-label="name" :label="selectLabel ?? ''"
        color="teal" clearable/> -->
</template>

<script>
import { ref } from 'vue';

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
    setup(props) {
        const options = ref([])

        return {
            options,

            // filterFn(val, update) {
            //     const filterCriteria = val.toLowerCase()
            //     update(() => {
            //         options.value = props.selectOptions.filter(option => option.name.toLowerCase().includes(filterCriteria))
            //     })
            // }
            filterFn(val, update) {
                const filterCriteria = val.toLowerCase()
                update(() => {
                    options.value = props.selectOptions.map(team => {
                        return {
                            label: team.label,
                            options: team.options.filter(option => option.name.toLowerCase().includes(filterCriteria))
                        }
                    })
                })
            }
        }
    },
    emits: ['update:modelValue'],
    methods: {
        changeOption(option) {
            this.$refs.expandSelect.updateInputValue('', true);
            this.$emit('update:modelValue', option);
            this.$refs.expandSelect.hidePopup(); // v-close-popup on the q-item freaks out with the dropdown options changing so we need to call it here
        },
        updateSelectedOption(option) {
            this.$emit('update:modelValue', option);
            this.$refs.expandSelect.hidePopup();
        },
    }
};
</script>
<style scoped>
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
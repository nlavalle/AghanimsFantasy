<template>
    <q-select ref="expandSelect" class="draft-selector" use-input filled input-debounce="500" :modelValue="modelValue"
        @update:model-value="updateSelectedOption" :options="options" @filter="filterFn" option-label="name"
        :label="selectLabel ?? ''" color="nadcl-accent" clearable>
        <template v-slot:option="scope">
            <q-expansion-item expand-separator :default-opened="true" header-class="text-weight-bold"
                :label="scope.opt.label">
                <template v-for="child in scope.opt.options" :key="child.label">
                    <q-item clickable @click="changeOption(child)">
                        <q-item-section>
                            <q-item-label class="q-ml-md">
                                <q-img :src=getPositionIcon(child.position) height="25px" width="25px"/>
                                {{ child.name }} 
                            </q-item-label>
                        </q-item-section>
                    </q-item>
                </template>
            </q-expansion-item>
        </template>
    </q-select>
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
        getPositionIcon(positionInt) {
            return `icons/pos_${positionInt}.png`
        },
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
.draft-selector {
    background-color: var(--nadcl-main-3);
}
</style>
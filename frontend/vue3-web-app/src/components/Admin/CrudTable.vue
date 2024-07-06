<template>
    <v-data-table class="league-table" :items="tableItems" :headers="headersWithActions" density="compact"
        :items-per-page="props.itemsPerPage" :search="search">
        <template v-slot:top>
            <v-toolbar flat>
                <v-text-field v-model="search" label="Search" variant="outlined" hide-details
                    single-line></v-text-field>
                <v-divider class="mx-4" inset vertical></v-divider>
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog" max-width="500px">
                    <template v-slot:activator="{ props }">
                        <v-btn class="mb-2" dark v-bind="props">
                            New Item
                        </v-btn>
                    </template>
                    <v-card>
                        <v-card-title>
                            <span class="text-h5">{{ formTitle }}</span>
                        </v-card-title>

                        <v-card-text>
                            <v-container>
                                <v-row>
                                    <textarea v-model="jsonInput" rows="10" cols="50"
                                        placeholder="Paste your JSON here..."></textarea>
                                </v-row>
                            </v-container>
                        </v-card-text>

                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="blue-darken-1" variant="text" @click="close">
                                Cancel
                            </v-btn>
                            <v-btn color="blue-darken-1" variant="text" @click="save">
                                Save
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
                <v-dialog v-model="dialogEdit" max-width="1200px">
                    <v-card>
                        <v-card-title>
                            <span class="text-h5">{{ formTitle }}</span>
                        </v-card-title>

                        <v-card-text>
                            <v-container>
                                <v-row>
                                    <textarea v-model="jsonInputEdit" rows="20" cols="256"
                                        placeholder="Paste your JSON here..."></textarea>
                                </v-row>
                            </v-container>
                        </v-card-text>

                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="blue-darken-1" variant="text" @click="closeEdit">
                                Cancel
                            </v-btn>
                            <v-btn color="blue-darken-1" variant="text" @click="edit">
                                Save
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
                <v-dialog v-model="dialogDelete" max-width="500px">
                    <v-card>
                        <v-card-title class="text-h5">Are you sure you want to delete this item?</v-card-title>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="blue-darken-1" variant="text" @click="closeDelete">Cancel</v-btn>
                            <v-btn color="blue-darken-1" variant="text" @click="deleteItemConfirm">OK</v-btn>
                            <v-spacer></v-spacer>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-toolbar>
        </template>
        <template v-slot:item.actions="{ item }">
            <font-awesome-icon :icon="faPencil" class="me-2" @click="editItem(item)" />
            <font-awesome-icon :icon="faDeleteLeft" class="me-2" @click="deleteItem(item)" />
        </template>
    </v-data-table>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick, watch } from 'vue';

import { VDataTable } from 'vuetify/components';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faPencil, faDeleteLeft } from '@fortawesome/free-solid-svg-icons';

const props = defineProps(
    {
        tableItems: {
            type: Array<any>,
            required: true
        },
        tableColumns: {
            type: Array<any>,
            required: true,
        },
        itemsPerPage: {
            type: Number,
            required: false,
            default: 10
        },
        defaultItemSpecified: {
            type: Object,
            required: false,
        }
    })

const emit = defineEmits(['save', 'edit', 'delete']);

const dialog = ref(false);
const dialogEdit = ref(false);
const dialogDelete = ref(false);
const search = ref('');

const jsonInput = ref<string>('');
const jsonInputEdit = ref<string>('');
const jsonObject = ref<any>(null);

const headersWithActions = computed(() => {
    return [...props.tableColumns, {
        title: 'Actions',
        value: 'actions',
        sortable: false
    }]
})

const formTitle = computed(() => {
    return editedIndex.value === -1 ? 'New Item' : 'Edit Item'
})

const editedIndex = ref(-1);
const editedItem = ref({});
const defaultItem = ref({});

onMounted(() => {
    if (props.tableItems.length > 0 && !props.defaultItemSpecified) {
        defaultItem.value = Object.assign({}, props.tableItems[0]);
    } else {
        defaultItem.value = props.defaultItemSpecified ? props.defaultItemSpecified : {};
    }

    if (props.tableItems.length > 0) {
        editedItem.value = Object.assign({}, props.tableItems[0]);
    }

    if (props.defaultItemSpecified) {
        jsonInput.value = JSON.stringify(props.defaultItemSpecified, null, 2);
    }
})

const editItem = (item: any) => {
    editedIndex.value = props.tableItems.indexOf(item)
    editedItem.value = Object.assign({}, item)
    jsonInputEdit.value = JSON.stringify(editedItem.value, null, 2);
    dialogEdit.value = true
};

const deleteItem = (item: any) => {
    editedIndex.value = props.tableItems.indexOf(item)
    editedItem.value = Object.assign({}, item)
    dialogDelete.value = true
};

const deleteItemConfirm = () => {
    emit('delete', editedItem.value)
    closeDelete()
};

const close = () => {
    dialog.value = false
    nextTick(() => {
        editedItem.value = Object.assign({}, defaultItem.value)
        editedIndex.value = -1
    })
};

const closeDelete = () => {
    dialogDelete.value = false
    nextTick(() => {
        editedItem.value = Object.assign({}, defaultItem.value)
        editedIndex.value = -1
    })
};

const closeEdit = () => {
    dialogEdit.value = false
    nextTick(() => {
        editedItem.value = Object.assign({}, defaultItem.value)
        editedIndex.value = -1
    })
};

const edit = () => {
    try {
        jsonObject.value = JSON.parse(jsonInputEdit.value);
        emit('edit', jsonObject.value)
    } catch (e) {
        alert('Invalid JSON input');
    }

    closeEdit()
};

const save = () => {
    try {
        jsonObject.value = JSON.parse(jsonInput.value);
        emit('save', jsonObject.value)
    } catch (e) {
        alert('Invalid JSON input');
    }

    close()
};

watch(
    () => props.defaultItemSpecified,
    () => {
        jsonInput.value = JSON.stringify(props.defaultItemSpecified, null, 2);
    }
)

watch(
    () => dialog,
    (val: any) => {
        val || close()
    }
)

watch(
    () => dialogDelete,
    (val: any) => {
        val || closeDelete()
    }
)

</script>

<style scoped>
.v-data-table ::v-deep(thead) {
    background-color: var(--tf2gg-main-2);
}
</style>
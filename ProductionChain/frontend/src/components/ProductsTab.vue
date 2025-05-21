<template>
    <v-card title="Продкуция"
            flat>
        <template v-slot:text>
            <v-text-field v-model="search"
                          label="Найти"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line></v-text-field>
        </template>

        <v-data-table :headers="headers"
                      :items="desserts"
                      :search="search"
                      hide-default-footer
                      :items-per-page="contactsPerPage"
                      no-data-text="Список контактов пуст"></v-data-table>
    </v-card>
</template>
<script setup>
    import { ref } from "vue";

    const contactsPerPage = 5;
    const search = ref("");
    const headers = [        
        { value: "id", title: "№" },
        { value: "name", title: "Название" },
        { value: "model", title: "Модель" },
    ]
    const desserts = [
        {
            id: 1,
            name: "БП 1000",
            model: 159
        },
        {
            id: 2,
            name: "БП 2000",
            model: 237
        },
        {
            id: 3,
            name: "БС 1000",
            model: 262
        }
    ]
</script>
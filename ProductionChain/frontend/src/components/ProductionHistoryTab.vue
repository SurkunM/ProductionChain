<template>
    <v-card title="История"
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
        { value: "orderId", title: "Id заказа" },
        { value: "lastName", title: "Сотрудник" },
        { value: "product", title: "Продукция" },
    ]
    const desserts = [
        {
            id: 1,
            lastName: "Астахов А.В",
            product: "БП 3000",
            orderId: 2,
        },
        {
            id: 2,
            lastName: "Иванова Н.Н",
            product: "БП 2000",
            orderId: 1,
        },
        {
            id: 3,
            lastName: "Борисов Г.С",
            product: "БС 1000",
            orderId: 1,
        }
    ]
</script>
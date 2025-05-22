<template>
    <v-card title="Склад готовой продкуции"
            flat>
        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Найти"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line></v-text-field>
        </template>

        <v-data-table :headers="headers"
                      :items="items"
                      :search="term"
                      hide-default-footer
                      :items-per-page="productsPerPage"
                      no-data-text="Список пуст"></v-data-table>
    </v-card>
</template>
<script>
    export default {
        data() {
            return {
                productsPerPage: 5,
                term: "",
                headers: [
                    { value: "id", title: "№" },
                    { value: "name", title: "Название" },
                    { value: "model", title: "Модель" },
                    { value: "count", title: "шт" }
                ],
                items: [
                    {
                        id: 1,
                        name: "БП 1000",
                        model: 159,
                        count: 25
                    },
                    {
                        id: 2,
                        name: "БП 2000",
                        model: 237,
                        count: 25
                    },
                    {
                        id: 3,
                        name: "БС 1000",
                        model: 262,
                        count: 25
                    }
                ]
            }
        }
    }
</script>
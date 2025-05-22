<template>
    <v-card title="Склад компонентов"
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
                    { value: "name", title: "Название компонента" },
                    { value: "productName", title: "Название изделия" },
                    { value: "productModel", title: "Модель" },
                    { value: "count", title: "шт" }
                ],
                items: [
                    {
                        id: 1,
                        name: "Печатная плата",
                        productName: "БП 1000",
                        productModel: 159,
                        count: 25
                    },
                    {
                        id: 2,
                        name: "Диодная плата",
                        productName: "БП 1000",
                        productModel: 237,
                        count: 25
                    },
                    {
                        id: 3,
                        name: "Радиатор",
                        productName: "БС 1000",
                        productModel: 262,
                        count: 25
                    },
                    {
                        id: 3,
                        name: "Корпус",
                        productName: "БС 1000",
                        productModel: 262,
                        count: 25
                    }
                ]
            }
        }
    }
</script>
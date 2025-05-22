<template>
    <v-card title="История"
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
                      :items="tasks"
                      :search="term"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст"></v-data-table>
    </v-card>
</template>
<script>
    export default {
        data() {
            return {
                itemsPerPage: 5,
                term: "",
                headers: [
                    { value: "id", title: "№" },
                    { value: "orderId", title: "Id заказа" },
                    { value: "lastName", title: "Сотрудник" },
                    { value: "product", title: "Продукция" },
                ],
                tasks: [
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
            }
        }
    }
</script>
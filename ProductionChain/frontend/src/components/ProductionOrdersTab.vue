<template>
    <v-card title="Nutrition"
            flat>
        <template v-slot:text>
            <v-text-field v-model="search"
                          label="Search"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line></v-text-field>
        </template>

        <v-data-table :headers="headers"
                      :items="desserts"
                      :search="search">

            <template v-slot:[`item.status`]="{ value }">
                <v-chip :border="`${getColor(value)} thin opacity-25`"
                        :color="getColor(value)"
                        :text="value"
                        size="small"></v-chip>
            </template>

            <template v-slot:[`item.actions`]="{ item }">
                <div>
                    <template v-if="!item.inProgress">
                        <v-btn size="small" color="info" @click="edit(item.id)" class="me-2">Начать</v-btn>
                    </template>
                </div>
            </template>
        </v-data-table>
    </v-card>
</template>
<script setup>
    import { ref } from "vue"

    const search = ref("")
    const headers = [
        { value: "id", title: "№" , width: "15%"},
        { value: "name", title: "Изделие", width: "25%" },
        { value: "count", title: "шт" , width: "15%"},
        { value: "status", title: "Статус", width: "20%" },
        { value: "actions", title: "" , width: "25%" },

    ]
    const desserts = [
        {
            id: 1,
            name: "БП 1000",
            count: 159,
            status: "в очереди",
            inProgress: false
        },
        {
            id: 9,
            name: "БП 2000",
            count: 237,
            status: "в очереди",
            inProgress: false
        },
        {
            id: 3,
            name: "БП 3000",
            count: 262,
            status: "в очереди",
            inProgress: false
        },
        {
            id: 4,
            name: "БС 1000",
            count: 305,
            status: "выполняется",
            inProgress: true
        },
        {
            id: 16,
            name: "БС 1000",
            count: 356,
            status: "завершен",
            inProgress: true
        }
    ]

    function getColor(state) {
        if (state === "в очереди") {
            return "error";
        }
        else if (state === "выполняется") {
            return "warning"
        }
        else {
            return "success";
        }
    }

    function edit(id) {
        console.log(id);

    }
</script>
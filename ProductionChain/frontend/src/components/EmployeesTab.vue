<template>
    <v-card title="Сотрудники"
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
                      no-data-text="Список контактов пуст">

            <template v-slot:[`item.state`]="{ value }">
                <v-chip :border="`${getColor(value)} thin opacity-25`"
                        :color="getColor(value)"
                        :text="value"
                        size="small"></v-chip>
            </template>
        </v-data-table>
    </v-card>
</template>
<script setup>
    import { ref } from "vue";

    const contactsPerPage = 5;
    const search = ref("");
    const headers = [
        { value: "id", title: "№" },
        { value: "lastName", title: "Фамилия" },
        { value: "firstName", title: "Имя" },
        { value: "middlename", title: "Отчество" },
        { value: "position", title: "Должность" },
        { value: "state", title: "Состояние" }
    ]
    const desserts = [
        {
            id: 1,
            lastName: "Астахов",
            firstName: "Николай",
            middlename: "иванович",
            position: "упаковщик",
            state: "свободен",
        },
        {
            id: 2,
            lastName: "Иванова",
            firstName: "Анна",
            middlename: "Николаевна",
            position: "пайщик",
            state: "занят",
        },
        {
            id: 3,
            lastName: "Борисов",
            firstName: "Петр",
            middlename: "",
            position: "монтажник РЭА",
            state: "отгул",
        }
    ]

    function getColor(state) {
        if (state === "отгул") {
            return "error";
        }
        else if (state === "занят") {
            return "warning"
        }
        else {
            return "success";
        }
    }
</script>
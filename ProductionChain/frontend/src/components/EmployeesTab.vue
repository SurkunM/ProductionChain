<template>
    <v-card title="Сотрудники"
            flat>

        <v-progress-linear v-if="isLoading"
                           indeterminate
                           color="primary"
                           height="4">
        </v-progress-linear>

        <v-alert type="success" variant="outlined" v-show="isShowSuccessAlert">
            <template v-slot:text>
                <span v-text="alertText"></span>
            </template>
        </v-alert>
        <v-alert type="error" variant="outlined" v-show="isShowErrorAlert">
            <template v-slot:text>
                <span v-text="alertText"></span>
            </template>
        </v-alert>

        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Найти"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line></v-text-field>
        </template>

        <v-data-table :headers="headers"
                      :items="employees"
                      :search="term"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

            <template v-slot:[`item.state`]="{ value }">
                <v-chip :border="`${getColor(value)} thin opacity-25`"
                        :color="getColor(value)"
                        :text="value"
                        size="small"></v-chip>
            </template>
        </v-data-table>

        <v-pagination v-model="currentPage"
                      :length="pagesCount"
                      @update:modelValue="switchPage"
                      circle
                      color="primary">
        </v-pagination>
    </v-card>
</template>

<script>
    export default {
        data() {
            return {
                term: "",
                currentPage: 1,

                sortByColumn: "lastName",
                sortDesc: false,

                headers: [
                    { value: "id", title: "№" },
                    { value: "lastName", title: "Фамилия" },
                    { value: "firstName", title: "Имя" },
                    { value: "middlename", title: "Отчество" },
                    { value: "position", title: "Должность" },
                    { value: "state", title: "Состояние" }
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            this.$store.dispatch("loadEmployees")
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить список сотрудников.");
                });
        },

        computed: {
            employees() {
                return this.$store.getters.employees;
            },

            itemsPerPage() {
                return this.$store.getters.pageSize;
            },

            pagesCount() {
                return Math.ceil(this.$store.getters.pageItemsCount / this.itemsPerPage);
            },

            isLoading() {
                return this.$store.getters.isLoading;
            }
        },

        methods: {
            getColor(state) {
                if (state === "отгул") {
                    return "error";
                }

                if (state === "занят") {
                    return "warning"
                }

                if (state === "свободен") {
                    return "success";
                }
            },

            switchPage(nextPage) {
                this.$store.dispatch("navigateToPage", nextPage);
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;

                setTimeout(() => {
                    this.alertText = "";
                    this.isShowSuccessAlert = false;
                }, 2000);
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;

                setTimeout(() => {
                    this.alertText = "";
                    this.isShowErrorAlert = false;
                }, 2000);
            }
        }
    }
</script>
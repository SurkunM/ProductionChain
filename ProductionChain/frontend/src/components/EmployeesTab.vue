<template>
    <v-card title="Сотрудники"
            flat>

        <v-progress-linear v-if="isLoading"
                           indeterminate
                           color="primary"
                           height="4">
        </v-progress-linear>

        <v-snackbar v-model="isShowSuccessAlert"
                    :timeout="2000"
                    color="success">
            {{alertText}}
        </v-snackbar>
        <v-snackbar v-model="isShowErrorAlert"
                    :timeout="2000"
                    color="error">
            {{alertText}}
        </v-snackbar>

        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Найти"
                          autocomplete="off"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line
                          @keyup.enter="search">
                <template v-slot:append-inner>
                    <v-btn icon
                           @click="search"
                           color="primary"
                           size="small">
                        <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-icon @click="cancelSearch"
                            style="cursor: pointer;"
                            size="x-large"
                            class="ms-1 me-2">
                        mdi-close-circle
                    </v-icon>
                </template>
            </v-text-field>
        </template>

        <v-data-table :headers="headers"
                      :items="employees"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

            <template v-slot:[`header.lastName`]="{ column }">
                <button @click="sortBy(column)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.firstName`]="{ column }">
                <button @click="sortBy(column)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.middleName`]="{ column }">
                <button @click="sortBy(column)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.position`]="{ column }">
                <button @click="sortBy(column)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.status`]="{ column }">
                <button @click="sortBy(column)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`item.status`]="{ value }">
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
                isSearchMode: false,
                currentPage: 1,

                sortByColumn: "lastName",
                sortDesc: false,

                headers: [
                    { value: "index", title: "№" },
                    { value: "lastName", title: "Фамилия" },
                    { value: "firstName", title: "Имя" },
                    { value: "middleName", title: "Отчество" },
                    { value: "position", title: "Должность" },
                    { value: "status", title: "Состояние" }
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            this.$store.commit("setSearchParameters", this.term);

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
            search() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = true;

                this.$store.dispatch("loadEmployees")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список сотрудников.");
                    });
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = false;

                this.$store.dispatch("loadEmployees")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список сотрудников.");
                    });
            },

            sortBy(column) {
                if (this.sortByColumn === column.value) {
                    this.sortDesc = !this.sortDesc;
                } else {
                    this.sortDesc = false;
                    this.sortByColumn = column.value;
                }

                this.$store.commit("setSortingParameters", {
                    sortBy: this.sortByColumn,
                    isDesc: this.sortDesc
                });

                this.$store.dispatch("loadEmployees")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список сотрудников.");
                    });
            },

            getColor(status) {
                if (status === "OnLeave") {
                    return "error";
                }

                if (status === "Busy") {
                    return "warning"
                }

                if (status === "Available") {
                    return "success";
                }
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadEmployees")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список сотрудников.");
                    });
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;
            }
        }
    }
</script>
<template>
    <v-card title="Задачи"
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
                      :items="tasks"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

            <template v-slot:[`header.product`]="{ column }">
                <button @click="sortBy(`product.Name`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.employee`]="{ column }">
                <button @click="sortBy(`employee.LastName`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.productsCount`]="{ column }">
                <button @click="sortBy(`productsCount`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.startTime`]="{ column }">
                <button @click="sortBy(`startTime`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`item.actions`]="{ item }">
                <div>
                    <template v-if="!item.inProgress">
                        <v-btn size="small" color="info" @click="completeTask(item)" class="me-2">Заверишть задачу</v-btn>
                    </template>
                </div>
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

                sortByColumn: "product.Name",
                sortDesc: false,

                headers: [
                    { value: "index", title: "№", align: 'center' },
                    { value: "product", title: "Изделие" },
                    { value: "employee", title: "Сотрудник" },
                    { value: "productsCount", title: "шт" },
                    { value: "startTime", title: "Начало" },
                    { value: "actions", title: "" }
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            this.$store.commit("setSearchParameters", this.term);

            this.$store.dispatch("loadProductionTasks")
                .catch(error => {
                    if (error.status === 401) {
                        this.showErrorAlert("Ошибка! Вы не авторизованы.");
                        this.$store.commit("setIsShowLoginModal", true);
                    }
                    else if (error.status === 403) {
                        this.showErrorAlert("У вас нет прав для получения данной информации.");
                    }
                    else {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список задачи.");
                    }
                });
        },

        computed: {
            tasks() {
                return this.$store.getters.tasks;
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
            completeTask(task) {
                const parameters = {
                    id: task.id,
                    productionOrderId: task.productionOrderId,
                    employeeId: task.employeeId,
                    productId: task.productId,
                    productsCount: task.productsCount
                };

                this.$store.dispatch("deleteProductionTask", parameters)
                    .then(() => {
                        this.showSuccessAlert("Задача успешно завершена.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось завершить задачу.");
                    });
            },

            search() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = true;

                this.$store.dispatch("loadProductionTasks")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список задачи.");
                    });
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = false;

                this.$store.dispatch("loadProductionTasks")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список задачи.");
                    });
            },

            sortBy(column) {
                if (this.sortByColumn === column) {
                    this.sortDesc = !this.sortDesc;
                } else {
                    this.sortDesc = false;
                    this.sortByColumn = column;
                }

                this.$store.commit("setSortingParameters", {
                    sortBy: this.sortByColumn,
                    isDesc: this.sortDesc
                });

                this.$store.dispatch("loadProductionTasks")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список задачи.");
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadProductionTasks")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список задачи.");
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
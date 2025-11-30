<template>
    <v-card title="Задачи" flat>
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

        <template v-if="isAuthorized">
            <v-data-table :headers="headers"
                          :items="tasks"
                          hide-default-footer
                          :loading="isLoading"
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
        </template>

        <template v-else>
            <v-container class="fill-height" fluid>
                <v-row align="center" justify="center">
                    <v-col cols="12" sm="8" md="6" lg="4">
                        <v-card class="text-center pa-8">
                            <v-icon size="64" color="grey-lighten-1" class="mb-4">
                                mdi-account-lock
                            </v-icon>
                            <v-card-title class="text-h5 justify-center">
                                Требуется авторизация
                            </v-card-title>
                            <v-card-text>
                                <p class="text-body-1 mb-4">
                                    Для просмотра этой страницы необходимо войти в систему
                                </p>
                                <v-btn color="primary" @click="showLoginModal()">
                                    Войти
                                </v-btn>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </v-row>
            </v-container>
        </template>

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
                ]
            }
        },

        created() {
            if (!this.isAuthorized) {
                return;
            }

            this.loadData();
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
            },

            isAuthorized() {
                return this.$store.getters.isAuthorized;
            }
        },

        methods: {
            loadData() {
                this.$store.commit("setSearchParameters", this.term);

                this.$store.dispatch("loadProductionTasks")
                    .catch(error => {
                        if (error.status === 401) {
                            this.$store.commit("setAlertMessage", "Ошибка! Вы не авторизованы.");
                            this.$store.commit("isShowErrorAlert", true);
                        }
                        else if (error.status === 403) {
                            this.$store.commit("setAlertMessage", "У вас нет прав для получения данной информации.");
                            this.$store.commit("isShowErrorAlert", true);
                        }
                        else {
                            this.$store.commit("setAlertMessage", "Ошибка! Не удалось загрузить список задачи.");
                            this.$store.commit("isShowErrorAlert", true);
                        }
                    });
            },

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
                        this.$store.commit("setAlertMessage", "Задача успешно завершена.");
                        this.$store.commit("isShowSuccessAlert", true);
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось завершить задачу.");
                        this.$store.commit("isShowErrorAlert", true);
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
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось загрузить список задачи.");
                        this.$store.commit("isShowErrorAlert", true);
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
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось загрузить список задачи.");
                        this.$store.commit("isShowErrorAlert", true);
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
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось загрузить список задачи.");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadProductionTasks")
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось загрузить список задачи.");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true)
            }
        }
    }
</script>
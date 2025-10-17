<template>
    <v-card title="Очередь производства"
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
                    location="bottom right"
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

        <template v-if="isAuthorized">
            <v-data-table :headers="headers"
                          :items="productionOrders"
                          hide-default-footer
                          :items-per-page="itemsPerPage"
                          no-data-text="Список пуст">

                <template v-slot:[`header.name`]="{ column }">
                    <button @click="sortBy(`product.Name`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.inProgressCount`]="{ column }">
                    <button @click="sortBy(`inProgressProductsCount`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.completedCount`]="{ column }">
                    <button @click="sortBy(`completedProductsCount`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.totalCount`]="{ column }">
                    <button @click="sortBy(`totalProductsCount`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.status`]="{ column }">
                    <button @click="sortBy(column.status)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`item.status`]="{ value, item }">
                    <v-chip :border="`${getColor(value)} thin opacity-25`"
                            :color="getColor(value)"
                            :text="item.statusMap"
                            size="small"></v-chip>
                </template>

                <template v-slot:[`item.actions`]="{ item }">
                    <template v-if="item.completedCount < item.totalCount">
                        <v-btn size="small" color="info" @click="showTaskCreateModal(item)" class="mt-2">Создать задачу</v-btn>
                    </template>

                    <v-btn size="small" color="error" @click="endProductionOrder(item)" class="my-2">Завершить заказ</v-btn>
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

    <template>
        <production-task-create-modal ref="productionTaskCreateModal" @save="createTask"></production-task-create-modal>
    </template>
</template>
<script>
    import ProductionTaskCreateModal from "./modals/ProductionTaskCreateModal.vue";

    export default {
        components: {
            ProductionTaskCreateModal
        },

        data() {
            return {
                term: "",
                isSearchMode: false,
                currentPage: 1,

                sortByColumn: "product.Name",
                sortDesc: false,

                headers: [
                    { value: "index", title: "№", width: "10%" },
                    { value: "name", title: "Изделие", width: "20%" },

                    { value: "inProgressCount", title: "в работе (шт)", width: "15%" },
                    { value: "completedCount", title: "собрано (шт)", width: "15%" },
                    { value: "totalCount", title: "всего (шт)", width: "15%" },

                    { value: "status", title: "Статус", width: "10%" },
                    { value: "actions", title: "", width: "15%" }
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            if (!this.isAuthorized) {
                return;
            }

            this.loadData();
        },

        computed: {
            productionOrders() {
                return this.$store.getters.productionOrders;
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

                this.$store.dispatch("loadProductionOrders")
                    .catch(error => {
                        if (error.status === 401) {
                            this.showErrorAlert("Ошибка! Вы не авторизованы.");
                        }
                        else if (error.status === 403) {
                            this.showErrorAlert("У вас нет прав для получения данной информации.");
                        }
                        else {
                            this.showErrorAlert("Ошибка! Не удалось загрузить список производственных заказов.");
                        }
                    });
            },

            createTask(newTask) {
                this.$store.dispatch("createProductionTask", newTask)
                    .then(() => {
                        this.$refs.productionTaskCreateModal.hide();
                        this.showSuccessAlert("Задача успешно создана.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось создать задачу.");
                    });
            },

            showTaskCreateModal(productionOrder) {
                this.$refs.productionTaskCreateModal.show(productionOrder);
            },

            endProductionOrder(productionOrder) {
                this.$store.dispatch("deleteProductionOrder", productionOrder.id)
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось завершить производственную задачу. Возможно остались не завершенные задачи.");
                    });
            },

            search() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = true;

                this.$store.dispatch("loadProductionOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список производственных заказов.");
                    });
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = false;

                this.$store.dispatch("loadProductionOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список производственных заказов.");
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

                this.$store.dispatch("loadProductionOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список производственных заказов.");
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadProductionOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список производственных заказов.");
                    });
            },

            getColor(state) {
                if (state === "Pending") {
                    return "error";
                }
                else if (state === "InProgress") {
                    return "secondary";
                }
                else {
                    return "success";
                }
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;
            },

            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true)
            }
        }
    }
</script>
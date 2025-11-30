<template>
    <v-card title="Заказы" flat>
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
                          :items="orders"
                          hide-default-footer
                          :loading="isLoading"
                          :items-per-page="itemsPerPage"
                          no-data-text="Список пуст">

                <template v-slot:[`header.customer`]="{ column }">
                    <button @click="sortBy(`customer`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.productName`]="{ column }">
                    <button @click="sortBy(`product.Name`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.orderedProductsCount`]="{ column }">
                    <button @click="sortBy(`orderedProductsCount`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.availableProductsCount`]="{ column }">
                    <button @click="sortBy(`availableProductsCount`)">{{column.title}}</button>
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
                    <div>
                        <template v-if="isStatusPending(item.status)">
                            <v-btn size="small" color="info" @click="createProductionOrder(item)">Начать</v-btn>
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

                sortByColumn: "customer",
                sortDesc: false,

                headers: [
                    { value: "index", title: "№" },
                    { value: "customer", title: "Заказчик", width: "20%" },
                    { value: "productName", title: "Изделие", width: "20%" },
                    { value: "orderedProductsCount", title: "Заказано (шт)" },
                    { value: "availableProductsCount", title: "В налиии (шт)" },
                    { value: "status", title: "Статус" },
                    { value: "actions", title: "" },
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
            orders() {
                return this.$store.getters.orders;
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

                this.$store.dispatch("loadOrders")
                    .catch(error => {
                        if (error.status === 401) {
                            this.$store.commit("setAlertMessage", "Вы не авторизованы");
                            this.$store.commit("isShowErrorAlert", true);
                        }
                        else if (error.status === 403) {
                            this.$store.commit("setAlertMessage", "У вас нет прав для получения данной информации");
                            this.$store.commit("isShowErrorAlert", true);
                        }
                        else {
                            this.$store.commit("setAlertMessage", "Не удалось загрузить список заказов");
                            this.$store.commit("isShowErrorAlert", true);
                        }
                    });
            },

            createProductionOrder(order) {
                const parameters = {
                    orderId: order.id,
                    productId: order.productId,
                    productsCount: order.productsCount,
                    availableCount: order.availableCount
                }

                this.$store.dispatch("createProductionOrder", parameters)
                    .then(() => {
                        this.$store.commit("setAlertMessage", "Производственная задача успешно создана.");
                        this.$store.commit("isShowSuccessAlert", true);
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось создать производственную задачу");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            search() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = true;

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось загрузить список заказов");
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

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось загрузить список заказов");
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

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось загрузить список заказов");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось загрузить список заказов");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            getColor(status) {
                if (status === "Pending") {
                    return "error";
                }

                if (status === "InProgress") {
                    return "warning";
                }

                if (status === "Done") {
                    return "success";
                }
            },

            isStatusPending(status) {
                if (status === "Pending") {
                    return true;
                }

                return false;
            },

            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true)
            }
        }
    }
</script>
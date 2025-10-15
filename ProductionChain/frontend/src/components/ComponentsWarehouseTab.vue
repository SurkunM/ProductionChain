<template>
    <v-card title="Склад компонентов"
            flat>
        <v-progress-linear v-if="isLoading"
                           indeterminate
                           color="primary"
                           height="4">
        </v-progress-linear>

        <v-snackbar v-model="isShowSuccessAlert"
                    :timeout="2000"
                    location="bottom right"
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
                          :items="items"
                          hide-default-footer
                          :items-per-page="itemsPerPage"
                          no-data-text="Список пуст">

                <template v-slot:[`header.productName`]="{ column }">
                    <button @click="sortBy(`product.name`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.productModel`]="{ column }">
                    <button @click="sortBy(`product.model`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
                </template>

                <template v-slot:[`header.productsCount`]="{ column }">
                    <button @click="sortBy(`componentsCount`)">{{column.title}}</button>
                    <v-icon v-if="sortByColumn === column.value">
                        {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                    </v-icon>
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

                sortByColumn: "product.name",
                sortDesc: false,

                headers: [
                    { value: "index", title: "№" },
                    { value: "componentType", title: "Название компонента" },
                    { value: "productName", title: "Название изделия" },
                    { value: "productModel", title: "Модель" },
                    { value: "productsCount", title: "шт" }
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
            items() {
                return this.$store.getters.componentsWarehouseItems;
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

                this.$store.dispatch("loadComponentsWarehouseItems")
                    .catch(error => {
                        if (error.status === 401) {
                            this.showErrorAlert("Ошибка! Вы не авторизованы.");
                        } else {
                            this.showErrorAlert("Ошибка! Не удалось загрузить список компонентов.");
                        }
                    });
            },

            search() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = true;

                this.$store.dispatch("loadComponentsWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список компонентов.");
                    });
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = false;

                this.$store.dispatch("loadComponentsWarehouseItems")
                    .catch(error => {
                        if (error.status === 401) {
                            this.showErrorAlert("Ошибка! Вы не авторизованы.");
                        } else {
                            this.showErrorAlert("Ошибка! Не удалось загрузить список компонентов.");
                        }
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

                this.$store.dispatch("loadComponentsWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список компонентов.");
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadComponentsWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список компонентов.");
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
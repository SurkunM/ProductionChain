<template>
    <v-card title="Склад готовой продкуции"
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

        <v-data-table :headers="headers"
                      :items="items"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

            <template v-slot:[`header.name`]="{ column }">
                <button @click="sortBy(`product.Name`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.model`]="{ column }">
                <button @click="sortBy(`product.Model`)">{{column.title}}</button>
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
                    { value: "index", title: "№" },
                    { value: "name", title: "Название" },
                    { value: "model", title: "Модель" },
                    { value: "productsCount", title: "шт" }
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            this.$store.commit("setSearchParameters", this.term);

            this.$store.dispatch("loadAssemblyWarehouseItems")
                .catch(error => {
                    if (error.status === 401) {
                        this.showErrorAlert("Ошибка! Вы не авторизованы.");
                        this.$store.commit("setIsShowLoginModal", true);
                    } else {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список собранных изделий.");
                    }
                });
        },

        computed: {
            items() {
                return this.$store.getters.assemblyWarehouseItems;
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

                this.$store.dispatch("loadAssemblyWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список собранных изделий.");
                    });
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = false;

                this.$store.dispatch("loadAssemblyWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список собранных изделий.");
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

                this.$store.dispatch("loadAssemblyWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список собранных изделий.");
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadAssemblyWarehouseItems")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список собранных изделий.");
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
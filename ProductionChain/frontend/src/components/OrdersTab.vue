<template>
    <v-card title="Заказы"
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

        <v-data-table :headers="headers"
                      :items="orders"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

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

                sortByColumn: "name",
                sortDesc: false,

                headers: [
                    { value: "id", title: "№", width: "15%" },
                    { value: "name", title: "Изделие", width: "25%" },
                    { value: "count", title: "шт", width: "15%" },
                    { value: "status", title: "Статус", width: "20%" },
                    { value: "actions", title: "", width: "25%" },
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            this.$store.dispatch("loadOrders")
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить список заказов.");
                });
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
            }
        },

        methods: {
            getColor(state) {
                if (state === "в очереди") {
                    return "error";
                }
                else if (state === "выполняется") {
                    return "warning";
                }
                else {
                    return "success";
                }
            },

            edit(id) {
                console.log(id);
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
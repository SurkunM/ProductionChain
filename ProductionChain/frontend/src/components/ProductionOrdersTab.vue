<template>
    <v-card title="Очередь производства"
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
                      :items="productionOrders"
                      :search="term"
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
                <template v-if="item.completedCount < item.totalCount">
                    <v-btn size="small" color="info" @click="showTaskCreateModal(item)" class="mt-2">Создать задачу</v-btn>
                </template>

                <v-btn size="small" color="error" @click="endProductionOrder(item)" class="my-2">Завершить заказ</v-btn>
            </template>
        </v-data-table>

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
    import ProductionTaskCreateModal from "./ProductionTaskCreateModal.vue";

    export default {
        components: {
            ProductionTaskCreateModal
        },

        data() {
            return {
                term: "",
                currentPage: 1,

                sortByColumn: "",
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
            this.$store.dispatch("loadProductionOrders")
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить список производственных заказов.");
                });
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
            }
        },

        methods: {
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

            createTask(newTask) {
                this.$store.dispatch("createProductionTask", newTask)
                    .then(() => this.$refs.productionTaskCreateModal.hide())
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось создать задачу.");
                    });
            },

            switchPage(nextPage) {
                this.$store.dispatch("navigateToPage", nextPage);
            },

            showTaskCreateModal(productionOrder) {
                this.$refs.productionTaskCreateModal.show(productionOrder);
            },

            endProductionOrder(productionOrder) {
                this.$store.dispatch("deleteProductionOrder", productionOrder.id)
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось завершить производственную задачу.");
                    });
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
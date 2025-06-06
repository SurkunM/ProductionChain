<template>
    <v-card title="Задачи"
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
                      :items="tasks"
                      :search="term"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

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
                currentPage: 1,

                sortByColumn: "",
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
            this.$store.dispatch("loadProductionTasks")
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить список задачи.");
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
            switchPage(nextPage) {
                this.$store.dispatch("navigateToPage", nextPage);
            },

            completeTask(task) {
                const parameters = {
                    id: task.id,
                    productionOrderId: task.productionOrderId,
                    employeeId: task.employeeId,
                    productId: task.productId,
                    productsCount: task.productsCount
                };

                this.$store.dispatch("deleteProductionTask", parameters);
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
<template>
    <v-card title="Очередь на получение задач"
            flat>
        <v-progress-linear v-if="isLoading"
                           indeterminate
                           color="primary"
                           height="4">
        </v-progress-linear>

        <template v-if="!isAuthorized">
            <v-data-table :headers="headers"
                          :items="taskQueue"
                          hide-default-footer
                          no-data-text="Список пуст">

                <template v-slot:[`header.employee`]="{ column }">
                    {{column.title}}
                </template>

                <template v-slot:[`header.productsCount`]="{ column }">
                    {{column.title}}
                </template>

                <template v-slot:[`header.date`]="{ column }">
                    {{column.title}}
                </template>

                <template v-slot:[`item.actions`]="{ item }">
                    <div>
                        <template>
                            <v-btn size="small" color="info" @click="showTaskCreateModal(item)" class="mt-2">Создать задачу</v-btn>
                        </template>

                        <template>
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
                headers: [
                    { value: "index", title: "№", align: 'center' },
                    { value: "employee", title: "Сотрудник" },
                    { value: "productsCount", title: "шт" },
                    { value: "date", title: "Дата" },
                    { value: "actions", title: "" }
                ]
            }
        },

        created() {
            if (!this.isAuthorized) {
                return;
            }
        },

        computed: {
            taskQueue() {
                return this.$store.getters.tasks;
            },

            isLoading() {
                return this.$store.getters.isLoading;
            },

            isAuthorized() {
                return this.$store.getters.isAuthorized;
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
                        this.$store.commit("setAlertMessage", "Задача успешно завершена.");
                        this.$store.commit("isShowSuccessAlert", true);
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось завершить задачу.");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            showTaskCreateModal(productionOrder) {
                this.$refs.productionTaskCreateModal.show(productionOrder);
            },

            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true)
            }
        }
    }
</script>
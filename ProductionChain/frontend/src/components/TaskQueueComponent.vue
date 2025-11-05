<template>
    <v-card title="Очередь на получение задач"
            flat>
        <v-progress-linear v-if="isLoading"
                           indeterminate
                           color="primary"
                           height="4">
        </v-progress-linear>

        <template v-if="isAuthorized">
            <v-data-table :headers="headers"
                          :items="taskQueue"
                          hide-default-footer
                          no-data-text="Список пуст">

                <template v-slot:[`header.employeeFullName`]="{ column }">
                    {{column.title}}
                </template>

                <template v-slot:[`header.createDate`]="{ column }">
                    {{column.title}}
                </template>

                <template v-slot:[`item.actions`]="{ item }">
                    <v-btn size="small" color="info" @click="showTaskCreateModal" class="me-4 mt-2">Выдать задачу</v-btn>
                    <v-btn size="small" color="error" @click="removeToTaskQueue(item)" class="mt-2">Удалить из очереди</v-btn>
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
</template>
<script>
    export default {
        data() {
            return {
                headers: [
                    { value: "index", title: "№", align: 'center' },
                    { value: "employeeFullName", title: "Сотрудник" },
                    { value: "createDate", title: "Дата" },
                    { value: "actions", title: "", width: "35%" }
                ]
            }
        },

        created() {
            if (!this.isAuthorized) {
                return;
            }

            this.$store.dispatch("loadTaskQueue");
        },

        computed: {
            taskQueue() {
                return this.$store.getters.taskQueue;
            },

            isLoading() {
                return this.$store.getters.isLoading;
            },

            isAuthorized() {
                return this.$store.getters.isAuthorized;
            }
        },

        methods: {
            removeToTaskQueue(task) {
                this.$store.dispatch("removeFromTaskQueue", task.employeeId)
                    .then(() => {
                        this.$store.commit("setAlertMessage", "Сотрудник удален из очереди");
                        this.$store.commit("isShowSuccessAlert", true);
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Ошибка! Не удалось удалить сотрудника из очереди");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            showTaskCreateModal() {
                this.$store.commit("isShowTaskCreateModal", true);
            },

            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true)
            }
        }
    }
</script>
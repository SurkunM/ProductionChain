<template>
    <v-card id="app">
        <v-layout>
            <v-app-bar>
                <h3 class="ms-3"> {{ userData.userName }} </h3>
            </v-app-bar>

            <v-snackbar v-model="isShowSuccessAlert"
                        :timeout="2000"
                        location="bottom right"
                        color="success"
                        @update:model-value="hideSuccessAlert">
                {{getAlertText}}
            </v-snackbar>
            <v-snackbar v-model="isShowErrorAlert"
                        :timeout="2000"
                        location="bottom right"
                        color="error"
                        @update:model-value="hideErrorAlert">
                {{getAlertText}}
            </v-snackbar>

            <v-navigation-drawer>
                <v-list density="compact"
                        nav>
                    <v-list-item prepend-icon="mdi-home"
                                 to="/"
                                 title="Главная"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 prepend-icon="mdi-layers-triple"
                                 to="/orders"
                                 title="Заказы"></v-list-item>

                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 prepend-icon="mdi-cog-play"
                                 to="/productionOrders"
                                 title="Производство"></v-list-item>

                    <v-list-item v-if="isManagerOrAdmin(userData.roles)"
                                 prepend-icon="mdi-cog-transfer"
                                 to="/task"
                                 title="Задачи"></v-list-item>

                    <v-list-item v-else prepend-icon="mdi-cog-transfer"
                                 v-show="isEmployee(userData.roles)"
                                 to="/task"
                                 title="Мои задачи"></v-list-item>

                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 :disabled="!isAuthorized"
                                 prepend-icon="mdi-inbox-multiple"
                                 to="/task">
                        <v-list-item-title class="d-flex justify-space-between align-center w-100">
                            Очередь на задачи
                            <v-badge v-if="taskQueueCount > 0"
                                     :content="taskQueueCount"
                                     color="primary"
                                     inline
                                     class="mr-2"></v-badge>
                        </v-list-item-title>
                    </v-list-item>

                    <v-list-item prepend-icon="mdi-bell-ring"
                                 :disabled="!isAuthorized"
                                 @click="showNewTaskModal"
                                 v-show="isEmployee(userData.roles)"
                                 title="Получить задачу"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-home-silo"
                                 to="/warehouse"
                                 title="Склад КП"></v-list-item>
                    <v-list-item prepend-icon="mdi-home-silo"
                                 to="/assemblywarehouse"
                                 title="Склад ГП"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 prepend-icon="mdi-account-hard-hat-outline"
                                 to="/employees"
                                 title="Сотрудники"></v-list-item>
                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 prepend-icon="mdi-clipboard-edit-outline"
                                 to="/products"
                                 title="Продукция"></v-list-item>
                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 prepend-icon="mdi-av-timer"
                                 to="/history"
                                 title="История задач"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item v-show="isManagerOrAdmin(userData.roles)"
                                 :disabled="!isAuthorized"
                                 prepend-icon="mdi-file-document-edit-outline"
                                 @click="showRegisterModal"
                                 title="Создать аккаунт"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-login"
                                 @click="showLoginModal"
                                 title="Войти"></v-list-item>
                    <v-list-item prepend-icon="mdi-logout"
                                 @click="showLogoutModal"
                                 title="Выйти"></v-list-item>
                </v-list>
            </v-navigation-drawer>

            <v-main>
                <router-view />
            </v-main>
        </v-layout>
    </v-card>

    <template>
        <login-modal ref="loginModal"></login-modal>
    </template>

    <template>
        <logout-modal ref="logoutModal"></logout-modal>
    </template>

    <template>
        <register-modal ref="registerModal"></register-modal>
    </template>
</template>

<script>
    import LoginModal from "./components/modals/LoginModal.vue";
import LogoutModal from "./components/modals/LogoutModal.vue";
import RegisterModal from "./components/modals/RegisterModal.vue";

    export default {
        components: {
            LoginModal,
            LogoutModal,
            RegisterModal
        },

        created() {
            this.$store.dispatch("initializeSignalR");
        },

        computed: {
            userData() {
                return this.$store.getters.getUserData;
            },

            taskQueueCount() {
                return this.$store.getters.taskQueueCount;
            },

            isAuthorized() {
                return this.$store.getters.isAuthorized;
            },

            isSignalRConnected() {
                return this.$store.getters.isSignalRConnected;
            },

            isShowSuccessAlert() {
                return this.$store.getters.isShowSuccessAlert;
            },

            isShowErrorAlert() {
                return this.$store.getters.isShowErrorAlert;
            },

            getAlertText() {
                return this.$store.getters.getAlertText;
            }
        },

        methods: {
            showLoginModal() {
                if (this.isAuthorized) {
                    this.$store.commit("isShowErrorAlert", "Вы уже авотризованы! Необходимо выйти из сессии.");
                    return;
                }

                this.$store.commit("setIsShowLoginModal", true);
            },

            showLogoutModal() {
                this.$store.commit("setIsShowLogoutModal", true);
            },

            showRegisterModal() {
                this.$store.commit("setIsShowRegisterModal", true);
            },

            showNewTaskModal() {
                this.$store.dispatch("addToTaskQueue", this.userData.userId)
                    .then(() => {
                        this.$store.commit("setAlertMessage", "Вы добавдены в очередь на получение задачи.");
                        this.$store.commit("isShowSuccessAlert", true);
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось добавить в очередь на получение задачи.");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            hideSuccessAlert() {
                this.$store.commit("setAlertMessage", "");
                this.$store.commit("isShowSuccessAlert", false);
            },

            hideErrorAlert() {
                this.$store.commit("setAlertMessage", "");
                this.$store.commit("isShowErrorAlert", false);
            },

            isManagerOrAdmin(userRole) {
                const allowedRoles = ["Manager", "Admin"];

                return userRole.some(r => allowedRoles.includes(r));
            },

            isEmployee(userRole) {
                return userRole.some(r => r === "Employee");
            }
        }
    }
</script>
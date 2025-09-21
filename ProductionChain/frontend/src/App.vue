<template>
    <v-card id="app">
        <v-layout>
            <v-navigation-drawer>
                <!--expand-on-hover
                rail-->

                <v-list density="compact" nav>
                    <v-list-item prepend-icon="mdi-home" to="/" title="Главная"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-layers-triple" to="/orders" title="Заказы"></v-list-item>

                    <v-list-item prepend-icon="mdi-cog-play" to="/productionOrders" title="Производство"></v-list-item>
                    <v-list-item prepend-icon="mdi-cog-transfer" to="/task" title="Задачи"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-home-silo" to="/warehouse" title="Склад КП"></v-list-item>
                    <v-list-item prepend-icon="mdi-home-silo" to="/assemblywarehouse" title="Склад ГП"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-account-hard-hat-outline" to="/employees" title="Сотрудники"></v-list-item>
                    <v-list-item prepend-icon="mdi-clipboard-edit-outline" to="/products" title="Продукция"></v-list-item>
                    <v-list-item prepend-icon="mdi-av-timer" to="/history" title="История задач"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-login" @click="showLoginModal" title="Войти"></v-list-item>
                    <v-list-item prepend-icon="mdi-file-document-edit-outline" @click="showRegisterModal" title="Создать аккаунт"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-logout" @click="logout" title="Выйти"></v-list-item>
                </v-list>
            </v-navigation-drawer>

            <v-main>
                <router-view />
            </v-main>
        </v-layout>
    </v-card>

    <template>
        <login-modal ref="loginModal" @save="createTask"></login-modal>
    </template>

    <template>
        <register-modal ref="registerModal" @save="createTask"></register-modal>
    </template>
</template>

<script>
    import LoginModal from "./components/LoginModal.vue";
    import RegisterModal from "./components/RegisterModal.vue";

    export default {
        components: {
            LoginModal,
            RegisterModal
        },

        methods: {
            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true);
            },

            showRegisterModal() {
                this.$store.commit("setIsShowRegisterModal", true);
            },

            logout() {
                this.$store.dispatch("logout")
                    .then(() => alert("logout success"))
                    .catch(() => alert("logout error"));
            }
        }
    }
</script>
<template>
    <v-dialog v-model="isShowLogoutModal"
              persistent max-width="500px"
              @keydown.esc="hide"
              @keydown.enter="logout">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Выход из аккаунта</v-toolbar-title>
                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-card-text class="d-flex justify-center align-center flex-column py-8">
                Вы действительно хотите выйти?
            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="success" @click="logout">Выйти</v-btn>
                <v-btn color="error" @click="hide">Отмена</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        data() {
            return {}
        },

        computed: {
            isShowLogoutModal() {
                return this.$store.getters.isShowLogoutModal;
            }
        },

        methods: {
            show() {
                this.$store.commit("setIsShowLogoutModal", true);
            },

            hide() {
                this.$store.commit("setIsShowLogoutModal", false);
            },

            logout() {
                this.$store.dispatch("logout")
                    .then(() => {
                        this.$store.commit("showSuccessAlert", "Вы вышли из системы.");
                    })
                    .catch(() => {
                        this.$store.commit("showErrorAlert", "Не удалось выйти.");
                    })
                    .finally(() => this.hide());
            }
        }
    }
</script>
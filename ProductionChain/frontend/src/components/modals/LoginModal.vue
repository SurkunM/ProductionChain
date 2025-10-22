<template>
    <v-dialog v-model="isShowLoginModal" persistent max-width="500px" @keydown.esc="hide">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Войти в аккаунт</v-toolbar-title>
                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-card-text>
                <v-form @submit.prevent="submitForm">
                    <v-text-field v-model="accountData.username"
                                  label="Логин"
                                  prepend-icon="mdi-account"
                                  :error-messages="errors.username"
                                  autocomplete="off"
                                  class="mt-4"></v-text-field>

                    <v-text-field v-model="accountData.password"
                                  label="Пароль"
                                  prepend-icon="mdi-lock"
                                  :type="showPassword ? 'text' : 'password'"
                                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                  :error-messages="errors.password"
                                  @click:append-inner="showPassword = !showPassword"></v-text-field>

                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" type="submit">Вход</v-btn>
                        <v-btn color="error" @click="hide">Закрыть</v-btn>
                    </v-card-actions>
                </v-form>
            </v-card-text>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        data() {
            return {
                showPassword: false,

                accountData: {
                    username: "",
                    password: ""
                },

                errors: {
                    username: "",
                    password: ""
                }
            }
        },

        computed: {
            isShowLoginModal() {
                return this.$store.getters.isShowLoginModal;
            }
        },

        methods: {
            show() {
                this.$store.commit("setIsShowLoginModal", true);
            },

            hide() {
                this.resetForm();
                this.$store.commit("setIsShowLoginModal", false);
            },

            submitForm() {
                this.clearErrors();

                if (!this.accountData.username) {
                    this.errors.username = "Некорректный логин";
                    return;
                }

                if (!this.accountData.password || this.accountData.password.length < 6) {
                    this.errors.password = "Некорректный пароль";
                    return;
                }

                this.$store.dispatch("login", this.accountData)
                    .then(() => {
                        this.$store.commit("setAlertMessage", "Вы вошли в систему.");

                        this.$store.commit("isShowSuccessAlert", true);
                        this.hide();
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось войти");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            clearErrors() {
                this.errors = {
                    username: "",
                    password: ""
                };
            },

            resetForm() {
                this.accountData = {
                    username: "",
                    password: ""
                }
            }
        }
    }
</script>
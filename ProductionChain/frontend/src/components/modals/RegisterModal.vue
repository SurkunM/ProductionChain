<template>
    <v-dialog v-model="isShwoRegisterModal" persistent max-width="600px" @keydown.esc="hide">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Создать аккаунт сотрудника</v-toolbar-title>
                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-form @submit.prevent="submitForm">
                <v-card-text>
                    <v-text-field v-model="accountData.login"
                                  label="Логин"
                                  prepend-icon="mdi-account"
                                  :error-messages="errors.login"
                                  autocomplete="off"></v-text-field>

                    <v-text-field v-model="accountData.password"
                                  label="Пароль"
                                  prepend-icon="mdi-lock"
                                  :type="showPassword ? 'text' : 'password'"
                                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                  autocomplete="new-password"
                                  :error-messages="errors.password"
                                  @click:append-inner="showPassword = !showPassword"></v-text-field>

                    <v-autocomplete v-model="accountData.employeeId"
                                    :items="employees"
                                    item-title="lastName"
                                    item-value="id"
                                    label="Выбрать сотрудника"
                                    prepend-icon="mdi-account-tie"
                                    v-model:search="employeeSearch"
                                    :error-messages="errors.employee"
                                    autocomplete="off"
                                    class="mt-4"></v-autocomplete>

                    <v-select v-model="accountData.role"
                              :items="roles"
                              label="Выбрать роль"
                              :error-messages="errors.role"
                              prepend-icon="mdi-account-key"></v-select>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="error" @click="hide">Отменить</v-btn>
                    <v-btn color="primary" type="submit">Создать</v-btn>
                </v-card-actions>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        data() {
            return {
                isShow: false,
                showPassword: false,
                employeeSearch: "",

                accountData: {
                    employeeId: null,
                    role: null,
                    login: "",
                    password: ""
                },

                errors: {
                    employee: "",
                    login: "",
                    password: "",
                    role: ""
                },

                roles: [
                    "Manager",
                    "Employee"
                ]
            }
        },

        computed: {
            employees() {
                return this.$store.getters.employees;
            },

            isShwoRegisterModal() {
                return this.$store.getters.isShowRegisterModal;
            }
        },

        methods: {
            hide() {
                this.resetForm();
                this.$store.commit("setIsShowRegisterModal", false);
            },

            submitForm() {

                if (!this.accountData.login) {
                    this.errors.login = "Некорректный логин";

                    return;
                }

                if (!this.accountData.password || this.accountData.password.length < 6) {
                    this.errors.password = "Некорректный парль";

                    return;
                }

                if (!this.accountData.employeeId) {
                    this.errors.employee = "Не выбран сотрудник";

                    return;
                }

                if (!this.accountData.role) {
                    this.errors.role = "Не выбрана роль";

                    return;
                }

                this.$store.dispatch("register", this.accountData)
                    .then(() => {
                        this.$store.commit("setAlertMessage", "Аккаунт успешно создан");
                        this.$store.commit("isShowSuccessAlert", true);
                        this.hide();
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось создать аккаунт");
                        this.$store.commit("isShowErrorAlert", true);
                    });
            },

            filterEmployees() {
                if (!this.employeeSearch) {
                    return this.employees;
                }

                const searchTerm = this.employeeSearch.toLowerCase();

                return this.employees.filter(e =>
                    e.fullName.toLowerCase().includes(searchTerm) || e.position.toLowerCase().includes(searchTerm)
                );
            },

            resetForm() {
                this.accountData = {
                    employeeId: null,
                    role: null,
                    login: "",
                    password: ""
                }
            }
        }
    }
</script>
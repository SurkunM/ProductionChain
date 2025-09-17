<template>
    <v-dialog v-model="isShow" persistent max-width="500px">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>{{ showRegistration ? 'Создать аккаунт сотрудника' : 'Войти в аккаунт' }}</v-toolbar-title>
                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <!-- Компонент входа -->
            <v-card-text v-if="!showRegistration">
                <v-form ref="loginForm" @submit.prevent="handleLogin">
                    <v-text-field v-model="loginCredentials.username"
                                  label="Логин"
                                  prepend-icon="mdi-account"
                                  required
                                  :rules="usernameRules"
                                  class="mt-4"></v-text-field>

                    <v-text-field v-model="loginCredentials.password"
                                  label="Пароль"
                                  prepend-icon="mdi-lock"
                                  :type="showPassword ? 'text' : 'password'"
                                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                  required
                                  :rules="passwordRules"
                                  @click:append-inner="showPassword = !showPassword"></v-text-field>
                </v-form>
            </v-card-text>

            <!-- Компонент регистрации -->
            <v-card-text v-if="showRegistration">
                <v-form ref="registrationForm" @submit.prevent="handleRegistration">
                    <v-select v-model="registrationData.employee"
                              :items="employees"
                              item-title="name"
                              item-value="id"
                              label="Выбрать сотрудника"
                              prepend-icon="mdi-account-tie"
                              :rules="employeeRules"
                              class="mt-4"></v-select>

                    <v-select v-model="registrationData.role"
                              :items="roles"
                              label="Выбрать роль"
                              prepend-icon="mdi-account-key"
                              :rules="roleRules"></v-select>

                    <v-text-field v-model="registrationData.username"
                                  label="Логин"
                                  prepend-icon="mdi-account"
                                  :rules="usernameRules"></v-text-field>

                    <v-text-field v-model="registrationData.password"
                                  label="Пароль"
                                  prepend-icon="mdi-lock"
                                  :type="showRegPassword ? 'text' : 'password'"
                                  :append-inner-icon="showRegPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                  :rules="passwordRules"
                                  @click:append-inner="showRegPassword = !showRegPassword"></v-text-field>
                </v-form>
            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>

                <!-- Кнопки для режима входа -->
                <template v-if="!showRegistration">
                    <v-btn color="primary" @click="handleLogin">Вход</v-btn>
                    <v-btn color="secondary" @click="clearLoginForm">Очистить</v-btn>
                    <v-btn variant="text" color="primary" @click="showRegistration = true">
                        Создать аккаунт
                    </v-btn>
                </template>

                <!-- Кнопки для режима регистрации -->
                <template v-if="showRegistration">
                    <v-btn color="primary" @click="handleRegistration">Создать</v-btn>
                    <v-btn color="secondary" @click="cancelRegistration">Отменить</v-btn>
                </template>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        name: 'LoginModal',
        props: {
            //isShow: {
            //    type: Boolean,
            //    default: false
            //}
        },
        data() {
            return {
                isShow: true,
                showRegistration: false,
                showPassword: false,
                showRegPassword: false,
                loginCredentials: {
                    username: '',
                    password: ''
                },
                registrationData: {
                    employee: null,
                    role: null,
                    username: '',
                    password: ''
                },
                employees: [
                    { id: 1, name: 'Иванов И.И.' },
                    { id: 2, name: 'Петров П.П.' },
                    { id: 3, name: 'Сидоров С.С.' }
                ],
                roles: [
                    'Администратор',
                    'Менеджер',
                    'Оператор',
                    'Рабочий'
                ],
                usernameRules: [
                    v => !!v || 'Логин обязателен для заполнения',
                    v => (v && v.length >= 3) || 'Логин должен содержать минимум 3 символа'
                ],
                passwordRules: [
                    v => !!v || 'Пароль обязателен для заполнения',
                    v => (v && v.length >= 6) || 'Пароль должен содержать минимум 6 символов'
                ],
                employeeRules: [
                    v => !!v || 'Необходимо выбрать сотрудника'
                ],
                roleRules: [
                    v => !!v || 'Необходимо выбрать роль'
                ]
            }
        },
        methods: {
            show() {
                this.isShow = true;
            },

            hide() {
                this.isShow = false;
            },

            handleLogin() {
                if (this.$refs.loginForm.validate()) {
                    console.log('Вход с данными:', this.loginCredentials);
                    this.$emit('login', this.loginCredentials);
                    this.hide();
                }
            },

            clearLoginForm() {
                this.loginCredentials = {
                    username: '',
                    password: ''
                };
                this.$refs.loginForm.resetValidation();
            },

            handleRegistration() {
                if (this.$refs.registrationForm.validate()) {
                    console.log('Регистрация с данными:', this.registrationData);
                    this.$emit('register', this.registrationData);
                    this.hide();
                }
            },

            cancelRegistration() {
                this.showRegistration = false;
                this.registrationData = {
                    employee: null,
                    role: null,
                    username: '',
                    password: ''
                };
                if (this.$refs.registrationForm) {
                    this.$refs.registrationForm.resetValidation();
                }
            },

            resetForms() {
                this.showRegistration = false;
                this.clearLoginForm();
                this.cancelRegistration();
            }
        },
        watch: {
            isShow(newVal) {
                if (!newVal) {
                    this.resetForms();
                }
            }
        }
    }
</script>
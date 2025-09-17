<template>
    <v-dialog v-model="isShow" persistent max-width="600px">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Создать аккаунт сотрудника</v-toolbar-title>
                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-form ref="registrationForm" @submit.prevent="handleSubmit">
                <v-card-text>
                    <v-autocomplete v-model="formData.employee"
                                    :items="employees"
                                    item-title="fullName"
                                    item-value="id"
                                    label="Выбрать сотрудника"
                                    prepend-icon="mdi-account-tie"
                                    :rules="employeeRules"
                                    v-model:search="employeeSearch"
                                    clearable
                                    class="mt-4"></v-autocomplete>

                    <v-select v-model="formData.role"
                              :items="roles"
                              label="Выбрать роль"
                              prepend-icon="mdi-account-key"
                              :rules="roleRules"
                              clearable></v-select>

                    <v-text-field v-model="formData.username"
                                  label="Логин"
                                  prepend-icon="mdi-account"
                                  :rules="usernameRules"
                                  autocomplete="off"></v-text-field>

                    <v-text-field v-model="formData.password"
                                  label="Пароль"
                                  prepend-icon="mdi-lock"
                                  :type="showPassword ? 'text' : 'password'"
                                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                  :rules="passwordRules"
                                  autocomplete="new-password"
                                  @click:append-inner="showPassword = !showPassword"></v-text-field>

                    <v-alert v-if="errorMessage" type="error" class="mt-3">
                        {{ errorMessage }}
                    </v-alert>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="primary" type="submit" :loading="isLoading">
                        Создать
                    </v-btn>
                    <v-btn color="error" @click="hide">Отменить</v-btn>
                </v-card-actions>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        name: 'RegistrationModal',
        props: {
            //isShow: {
            //    type: Boolean,
            //    default: false
            //}
        },
        data() {
            return {
                isShow: true,
                isLoading: false,
                showPassword: false,
                employeeSearch: '',
                errorMessage: '',
                formData: {
                    employee: null,
                    role: null,
                    username: '',
                    password: ''
                },
                employees: [
                    { id: 1, fullName: 'Иванов Иван Иванович', position: 'Менеджер' },
                    { id: 2, fullName: 'Петров Петр Петрович', position: 'Разработчик' },
                    { id: 3, fullName: 'Сидорова Анна Владимировна', position: 'Дизайнер' },
                    { id: 4, fullName: 'Кузнецов Дмитрий Сергеевич', position: 'Тестировщик' },
                    { id: 5, fullName: 'Николаева Екатерина Александровна', position: 'Аналитик' }
                ],
                roles: [
                    'Администратор',
                    'Менеджер',
                    'Разработчик',
                    'Тестировщик',
                    'Аналитик',
                    'Дизайнер',
                    'Оператор'
                ],
                usernameRules: [
                    v => !!v || 'Логин обязателен для заполнения',
                    v => (v && v.length >= 3) || 'Логин должен содержать минимум 3 символа',
                    v => /^[a-zA-Z0-9_]+$/.test(v) || 'Логин может содержать только буквы, цифры и подчеркивания'
                ],
                passwordRules: [
                    v => !!v || 'Пароль обязателен для заполнения',
                    v => (v && v.length >= 6) || 'Пароль должен содержать минимум 6 символов',
                    v => /[A-Z]/.test(v) || 'Пароль должен содержать хотя бы одну заглавную букву',
                    v => /[0-9]/.test(v) || 'Пароль должен содержать хотя бы одну цифру'
                ],
                employeeRules: [
                    v => !!v || 'Необходимо выбрать сотрудника'
                ],
                roleRules: [
                    v => !!v || 'Необходимо выбрать роль'
                ]
            }
        },
        watch: {
            //isShow(newVal) {
            //    if (!newVal) {
            //        this.resetForm();
            //    }
            //}
        },
        methods: {
            show() {
                this.isShow = true;
            },
            hide() {
                this.isShow = false;
            },

            async handleSubmit() {
                if (this.$refs.registrationForm.validate()) {
                    this.isLoading = true;
                    this.errorMessage = '';

                    try {
                        // Здесь будет вызов API для регистрации
                        console.log('Данные регистрации:', this.formData);

                        // Имитация запроса
                        await new Promise(resolve => setTimeout(resolve, 1000));

                        this.$emit('registered', this.formData);
                        this.hide();

                        this.$emit('show-notification', {
                            type: 'success',
                            message: 'Аккаунт успешно создан'
                        });

                    } catch (error) {
                        this.errorMessage = 'Ошибка при создании аккаунта: ' + error.message;
                    } finally {
                        this.isLoading = false;
                    }
                }
            },

            resetForm() {
                this.formData = {
                    employee: null,
                    role: null,
                    username: '',
                    password: ''
                };
                this.errorMessage = '';
                this.showPassword = false;
                this.employeeSearch = '';

                if (this.$refs.registrationForm) {
                    this.$refs.registrationForm.resetValidation();
                }
            },

            filterEmployees() {
                if (!this.employeeSearch) return this.employees;

                const searchTerm = this.employeeSearch.toLowerCase();
                return this.employees.filter(employee =>
                    employee.fullName.toLowerCase().includes(searchTerm) ||
                    employee.position.toLowerCase().includes(searchTerm)
                );
            }
        }
    }
</script>
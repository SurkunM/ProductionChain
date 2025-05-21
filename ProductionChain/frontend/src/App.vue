<template>
    <v-card  id="app">
        <v-layout>
            <v-navigation-drawer>
                <!--expand-on-hover
                rail-->

                <v-list density="compact" nav>
                    <v-list-item prepend-icon="mdi-home" to="/" title="Главная"></v-list-item>
                    <v-divider></v-divider>

                    <v-list-item prepend-icon="mdi-layers-triple" to="/orders" title="Заказы"></v-list-item>
                    <v-list-item prepend-icon="mdi-home-silo" to="/warehouse" title="Склад"></v-list-item>
                    <v-list-item prepend-icon="mdi-cog-play" to="/task" title="Задачи"></v-list-item>
                    <v-list-item prepend-icon="mdi-cog-transfer" to="/productionOrders" title="Очередь задач"></v-list-item>
                    <v-list-item prepend-icon="mdi-av-timer" to="/history" title="История задач"></v-list-item>

                    <v-divider></v-divider>
                    <v-list-item prepend-icon="mdi-account-hard-hat-outline" to="/employees" title="Сотрудники"></v-list-item>
                    <v-list-item prepend-icon="mdi-clipboard-edit-outline" to="/products" title="Продукция"></v-list-item>
                </v-list>
            </v-navigation-drawer>

            <v-main>
                <router-view />
            </v-main>
        </v-layout>
    </v-card>
</template>
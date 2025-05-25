<template>
    <v-container fluid>
        <v-row>
            <v-col cols="12" md="4">
                <v-card title="Новые заказы" subtitle="За сегодня" text="12" prepend-icon="mdi-cart-plus"></v-card>
            </v-col>
            <v-col cols="12" md="4">
                <v-card title="Активные задачи" subtitle="В работе" text="5" prepend-icon="mdi-checkbox-marked-circle"></v-card>
            </v-col>
            <v-col cols="12" md="4">
                <v-card title="На складе" subtitle="Готовой продукции" text="42" prepend-icon="mdi-package-variant"></v-card>
            </v-col>
        </v-row>

        <v-row class="mt-5">
            <v-col cols="12" md="6">
                <v-card title="Последние заказы">
                    <v-table>
                        <thead>
                            <tr>
                                <th>№</th>
                                <th>Клиент</th>
                                <th>Статус</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="order in recentOrders" :key="order.id">
                                <td>{{ order.id }}</td>
                                <td>{{ order.client }}</td>
                                <td><v-chip :color="getStatusColor(order.status)">{{ order.status }}</v-chip></td>
                            </tr>
                        </tbody>
                    </v-table>
                </v-card>
            </v-col>

            <v-col cols="12" md="6">
                <v-card title="Активные задачи">
                    <v-timeline side="end">
                        <v-timeline-item v-for="task in activeTasks"
                                         :key="task.id"
                                         :dot-color="task.urgent ? 'red' : 'primary'"
                                         size="small">
                            <v-alert :color="task.urgent ? 'red-lighten-2' : 'primary-lighten-2'">
                                {{ task.title }} ({{ task.deadline }})
                            </v-alert>
                        </v-timeline-item>
                    </v-timeline>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    export default {
        data: () => ({
            recentOrders: [
                { id: 1001, client: "ООО Ромашка", status: "В производстве" },
                { id: 1000, client: "ИП Иванов", status: "Новый" },
                { id: 999, client: "ЗАО Вектор", status: "Отгружен" },
            ],
            activeTasks: [
                { id: 1, title: "Подготовить заказ #1001", deadline: "до 15:00", urgent: true },
                { id: 2, title: "Заказать материалы", deadline: "до конца дня", urgent: false },
            ],
        }),
        methods: {
            getStatusColor(status) {
                const colors = {
                    "Новый": "blue",
                    "В производстве": "orange",
                    "Отгружен": "green"
                };
                return colors[status] || "grey";
            }
        }
    }
</script>
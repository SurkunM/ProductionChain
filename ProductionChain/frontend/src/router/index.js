import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../components/HomeView.vue";

const routes = [
    {
        path: "/",
        name: "home",
        component: HomeView
    },
    {
        path: "/orders",
        name: "orders",
        component: () => import("../components/OrdersTab.vue")
    },
    {
        path: "/employees",
        name: "employees",
        component: () => import("../components/EmployeesTab.vue")
    },
    {
        path: "/products",
        name: "products",
        component: () => import("../components/ProductsTab.vue")
    },
    {
        path: "/warehouse",
        name: "warehouse",
        component: () => import("../components/WarehouseTab.vue")
    },
    {
        path: "/productionOrders",
        name: "productionOrders",
        component: () => import("../components/ProductionOrdersTab.vue")
    },
    {
        path: "/task",
        name: "task",
        component: () => import("../components/ProductionTaskTab.vue")
    },
    {
        path: "/history",
        name: "history",
        component: () => import("../components/ProductionHistoryTab.vue")
    }
]

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
});

export default router

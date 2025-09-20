import { createRouter, createWebHistory } from "vue-router";
import HomeTab from "../components/HomeTab.vue";

const routes = [
    {
        path: "/",
        name: "home",
        component: HomeTab
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
        path: "/orders",
        name: "orders",
        component: () => import("../components/OrdersTab.vue")
    },
    {
        path: "/warehouse",
        name: "warehouse",
        component: () => import("../components/ComponentsWarehouseTab.vue")
    },
    {
        path: "/assemblywarehouse",
        name: "assemblywarehouse",
        component: () => import("../components/AssemblyWarehouseTab.vue")
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
    },
    {
        path: "/login",
        name: "login",
        component: () => import("../components/LoginModal.vue")
    },
    {
        path: "/register",
        name: "register",
        component: () => import("../components/RegisterModal.vue")
    }
]

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
});

export default router

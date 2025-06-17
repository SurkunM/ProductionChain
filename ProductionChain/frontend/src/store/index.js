import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        employees: [],

        products: [],
        orders: [],

        tasks: [],
        histories: [],
        productionOrders: [],

        assemblyWarehouseItems: [],
        componentsWarehouseItems: [],

        isLoading: false,
        term: "",

        pageItemsCount: 0,
        pageNumber: 1,
        pageSize: 10,

        sortByColumn: "",
        isDescending: false,
    },

    getters: {
        isLoading(state) {
            return state.isLoading;
        },

        employees(state) {
            return state.employees;
        },

        products(state) {
            return state.products;
        },

        orders(state) {
            return state.orders;
        },

        tasks(state) {
            return state.tasks;
        },

        histories(state) {
            return state.histories;
        },

        productionOrders(state) {
            return state.productionOrders;
        },

        assemblyWarehouseItems(state) {
            return state.assemblyWarehouseItems;
        },

        assemblyWarehouseItemsCount(state) {
            return state.assemblyWarehouseItems.reduce((sum, item) => sum + item.productsCount, 0);
        },

        componentsWarehouseItems(state) {
            return state.componentsWarehouseItems;
        },

        pageSize(state) {
            return state.pageSize;
        },

        pageItemsCount(state) {
            return state.pageItemsCount;
        },
    },

    mutations: {
        setEmployees(state, employees) {
            state.employees = employees;
        },

        setProducts(state, products) {
            state.products = products;
        },

        setOrders(state, orders) {
            orders.forEach(o => {
                o.productName = `${o.productName || ""} (${o.productModel || ""})`;
            });

            state.orders = orders;
        },

        setTasks(state, tasks) {
            tasks.forEach(t => {
                t.employee = `${t.employeeLastName} ${t.employeeFirstName[0]}.${t.employeeMiddleName[0] || ""}.`;
                t.product = `${t.productName} (${t.productModel})`;
                t.startTime = new Date(t.startTime).toLocaleString("ru-RU", {
                    year: "numeric",
                    month: "long",
                    day: "numeric",
                    hour: "numeric",
                    minute: "numeric"
                });
            });

            state.tasks = tasks;
        },

        setHistories(state, histories) {
            state.histories = histories;
        },

        setProductionOrders(state, productionOrders) {
            productionOrders.forEach(po => {
                po.name = `${po.productName || ""} (${po.productModel || ""})`;
            });

            state.productionOrders = productionOrders;
        },

        setAssemblyWarehouseItems(state, items) {
            state.assemblyWarehouseItems = items;
        },

        setComponentsWarehouseItems(state, items) {
            items.forEach(c => {
                if (c.componentType === "CircuitBoard") {
                    c.componentType = "Печатная плата";
                }
                else if (c.componentType === "DiodeBoard") {
                    c.componentType = "Диодная плата";
                }
                else if (c.componentType === "Enclosure") {
                    c.componentType = "Корпус";
                } else {
                    c.componentType = "Радиатор";
                }
            });

            state.componentsWarehouseItems = items;
        },

        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setSearchParameters(state, value) {
            state.term = value;
            state.pageNumber = 1;
        },

        setResponseItemsIndex(state, items) {
            if (items.length === 0) {
                return;
            }

            items.forEach((c, i) => {
                c.index = state.pageNumber + i;
            });
        },

        setTerm(state, value) {
            state.term = value;
        },

        setPageNumber(state, value) {
            state.pageNumber = value;
        },

        setPageItemsCount(state, count) {
            state.pageItemsCount = count;
        },

        setSortingParameters(state, payload) {
            const { sortBy, isDesc } = payload;

            state.sortByColumn = sortBy;
            state.isDescending = isDesc;
        },
    },

    actions: {
        loadEmployees({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/Catalog/GetEmployees", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.employees);
                commit("setEmployees", response.data.employees);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadProducts({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/Catalog/GetProducts", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.products);
                commit("setProducts", response.data.products);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadOrders({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/Catalog/GetOrders", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.orders);
                commit("setOrders", response.data.orders);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadProductionTasks({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/ProductionAssembly/GetProductionTasks", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.tasks);
                commit("setTasks", response.data.tasks);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadHistories({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/ProductionAssembly/GetProductionHistory", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.histories);
                commit("setHistories", response.data.histories);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadProductionOrders({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/ProductionAssembly/GetProductionOrders", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.productionOrders);
                commit("setProductionOrders", response.data.productionOrders);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadAssemblyWarehouseItems({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/ProductionAssembly/GetAssemblyWarehouseItems", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.assemblyWarehouseItems);
                commit("setAssemblyWarehouseItems", response.data.assemblyWarehouseItems);
                commit("setPageItemsCount", response.data.totalCount);
            })
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить список компонентов.");
                }).finally(() => {
                    commit("setIsLoading", false);
                });
        },

        loadComponentsWarehouseItems({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/ProductionAssembly/GetComponentsWarehouseItems", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setResponseItemsIndex", response.data.componentsWarehouseItems);
                commit("setComponentsWarehouseItems", response.data.componentsWarehouseItems);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        createProductionOrder({ commit, dispatch }, parameters) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/CreateProductionOrder", parameters)
                .then(() => {
                    dispatch("loadOrders");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        createProductionTask({ commit, dispatch }, task) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/CreateProductionTask", task)
                .then(() => {
                    dispatch("loadProductionOrders");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteProductionTask({ commit, dispatch }, parameters) {
            commit("setIsLoading", true);

            return axios.delete("/api/ProductionAssembly/DeleteProductionTask", {
                data: parameters,
                headers: { "Content-Type": "application/json" }
            }).then(() => {
                dispatch("loadProductionTasks");
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        deleteProductionOrder({ commit, dispatch }, id) {
            commit("setIsLoading", true);

            return axios.delete("/api/ProductionAssembly/DeleteProductionOrder", {
                headers: { "Content-Type": "application/json" },
                data: id
            }).then(() => {
                dispatch("loadProductionOrders");
            }).finally(() => {
                commit("setIsLoading", false);
            });
        }
    }
})

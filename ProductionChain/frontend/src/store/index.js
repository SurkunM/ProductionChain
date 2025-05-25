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

        componentsWarehouseTabItems(state) {
            return state.componentsWarehouseTabItems;
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
            state.orders = orders;
        },

        setTasks(state, tasks) {
            state.tasks = tasks;
        },

        setHistories(state, histories) {
            state.histories = histories;
        },

        setProductionOrders(state, productionOrders) {
            state.productionOrders = productionOrders;
        },

        setAssemblyWarehouseItems(state, items) {
            state.assemblyWarehouseItems = items;
        },

        setComponentsWarehouseItems(state, items) {
            state.componentsWarehouseItems = items;
        },

        setIsLoading(state, value) {
            state.isLoading = value;
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
                commit("setEmployees", response.data.employees);
                commit("setPageItemsCount", response.data.totalCount);
            }).catch(response => console.log(response))
                .finally(() => {
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
                commit("setOrders", response.data.orders);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        loadTasks({ commit, state }) {
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
                commit("setAssemblyWarehouseItems", response.data.assemblyWarehouseItems);
                commit("setPageItemsCount", response.data.totalCount);
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
                commit("setComponentsWarehouseItems", response.data.componentsWarehouseItems);
                commit("setPageItemsCount", response.data.totalCount);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        createProductionOrder({ commit, dispatch }, order) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/CreateProductionOrder", order)
                .then(() => {
                    dispatch("loadProductionOrders");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        createTask({ commit, dispatch }, task) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/CreateProductionTask", task)
                .then(() => {
                    dispatch("loadTasks");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        createHistory({ commit, dispatch }, task) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/CreateProductionHistory", task)
                .then(() => {
                    dispatch("loadHistories");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        updateAssemblyWarehouseItems({ commit, dispatch }, item) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/UpdateAssemblyWarehouseItem", item)
                .then(() => {
                    dispatch("loadAssemblyWarehouseItems");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteTask({ commit, dispatch }, id) {
            commit("setIsLoading", true);

            return axios.delete("/api/ProductionAssembly/DeleteProductionTask", {
                headers: { "Content-Type": "application/json" },
                data: id
            }).then(() => {
                dispatch("loadTasks");
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

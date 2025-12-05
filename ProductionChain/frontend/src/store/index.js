import * as signalR from "@microsoft/signalr";
import axios from "axios";
import { createStore } from "vuex";

export default createStore({
    state: {
        employees: [],

        products: [],
        orders: [],

        tasks: [],
        taskQueue: [],

        histories: [],
        productionOrders: [],

        assemblyWarehouseItems: [],
        componentsWarehouseItems: [],

        isLoading: false,
        isAuthorized: false,

        term: "",
        userData: { userId: 0, userName: "", roles: ["Initial"] },
        productionOrderData: { id: 0, productId: 0, productName: "" },

        pageItemsCount: 0,
        pageNumber: 1,
        pageSize: 10,

        sortByColumn: "",
        isDescending: false,

        isShowRegisterModal: false,
        isShowLoginModal: false,
        isShowLogoutModal: false,
        isShowTaskCreateModal: false,

        signalRConnection: null,
        isSignalRConnected: false,

        isShowSuccessAlert: false,
        isShowErrorAlert: false,
        alertText: ""
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

        taskQueue(state) {
            return state.taskQueue;
        },

        taskQueueCount(state) {
            return state.taskQueue.length;
        },

        histories(state) {
            return state.histories;
        },

        productionOrders(state) {
            return state.productionOrders;
        },

        getProductionOrderData(state) {
            return state.productionOrderData;
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

        isShowLoginModal(state) {
            return state.isShowLoginModal;
        },

        isShowLogoutModal(state) {
            return state.isShowLogoutModal;
        },

        isShowTaskCreateModal(state) {
            return state.isShowTaskCreateModal;
        },

        isShowRegisterModal(state) {
            return state.isShowRegisterModal;
        },

        getUserData(state) {
            return state.userData;
        },

        isAuthorized(state) {
            return state.isAuthorized;
        },

        isSignalRConnected(state) {
            return state.isSignalRConnected;
        },

        signalRConnection(state) {
            return state.signalRConnection;
        },

        isShowSuccessAlert(state) {
            return state.isShowSuccessAlert;
        },

        isShowErrorAlert(state) {
            return state.isShowErrorAlert;
        },

        getAlertText(state) {
            return state.alertText;
        }
    },

    mutations: {
        setEmployeesMapping(state, employees) {
            employees.forEach(e => {
                if (e.status === "Available") {
                    e.statusMap = "Свободен";
                }
                else if (e.status === "OnLeave") {
                    e.statusMap = "Отсудствует";
                } else {
                    e.statusMap = "Занят";
                }

                if (e.position === "AssemblyREA") {
                    e.positionMap = "Монтажник РЭА";
                }
                else if (e.position === "SolderPCB") {
                    e.positionMap = "Пайщик";
                }
                else if (e.position === "TechnicianQA") {
                    e.positionMap = "Настройщик";
                } else {
                    e.positionMap = "Упаковщик";
                }
            });

            state.employees = employees;
        },

        setProducts(state, products) {
            state.products = products;
        },

        setOrdersMapping(state, orders) {
            orders.forEach(o => {
                o.productName = `${o.productName || ""} (${o.productModel || ""})`;

                if (o.status === "InProgress") {
                    o.statusMap = "В работе";
                }
                else if (o.status === "Pending") {
                    o.statusMap = "В очереди";
                } else {
                    o.statusMap = "Выполнено";
                }
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

        setProductionOrdersMapping(state, productionOrders) {
            productionOrders.forEach(po => {
                po.name = `${po.productName || ""} (${po.productModel || ""})`;

                if (po.status === "InProgress") {
                    po.statusMap = "В работе";
                }
                else if (po.status === "Pending") {
                    po.statusMap = "В очереди";
                } else {
                    po.statusMap = "Выполнено";
                }
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

        setSearchParameters(state, value) {//Сломан поиск в Tab-х(CompnetnsWarehouseTab). Нету поиска по названию!
            state.term = value;
            state.pageNumber = 1;
        },

        setResponseItemsIndex(state, items) {
            if (items.length === 0) {
                return;
            }

            items.forEach((item, i) => {
                item.index = (state.pageNumber - 1) * state.pageSize + i + 1;
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

        setIsShowLoginModal(state, isShow) {
            state.isShowLoginModal = isShow;
        },

        setIsShowLogoutModal(state, isShow) {
            state.isShowLogoutModal = isShow;
        },

        setIsShowRegisterModal(state, isShow) {
            state.isShowRegisterModal = isShow;
        },

        setUser(state, authUser) {
            state.userData = authUser
        },

        setIsAuthorized(state, isAuthorized) {
            state.isAuthorized = isAuthorized;
        },

        setSignalRIsConnected(state, isConnected) {
            state.isSignalRConnected = isConnected;
        },

        setSignalRConnection(state, connection) {
            state.signalRConnection = connection;
        },

        isShowSuccessAlert(state, value) {
            state.isShowSuccessAlert = value;
        },

        isShowErrorAlert(state, value) {
            state.isShowErrorAlert = value;
        },

        setAlertMessage(state, text) {
            state.alertText = text;
        },

        isShowTaskCreateModal(state, value) {
            state.isShowTaskCreateModal = value;
        },

        setProductionOrderData(state, productionOrder) {
            state.productionOrderData = productionOrder;
        },

        resetProductionOrderData(state) {
            state.productionOrderData = { id: 0, productId: 0, productName: "" };
        },

        setTaskQueue(state, taskQueue) {

            taskQueue.forEach((tq, i) => {
                tq.createDate = new Date(tq.createDate).toLocaleString("ru-RU", {
                    hour: "2-digit",
                    minute: "2-digit",
                    day: "2-digit",
                    month: "2-digit",
                    year: "numeric"
                }).replace(',', '');

                tq.index = i + 1;
            });

            state.taskQueue = taskQueue;
        },

        addEmployeeToTaskQueue(state, employee) {
            employee.createDate = new Date(employee.createDate).toLocaleString("ru-RU", {
                hour: "2-digit",
                minute: "2-digit",
                day: "2-digit",
                month: "2-digit",
                year: "numeric"
            }).replace(',', '');

            employee.index = state.taskQueue.length + 1;

            state.taskQueue.push(employee);
        }
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
                commit("setEmployeesMapping", response.data.employees);
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
            })
                .finally(() => {
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
                commit("setOrdersMapping", response.data.orders);
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
                commit("setProductionOrdersMapping", response.data.productionOrders);
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

        loadTaskQueue({ commit }) {
            commit("setIsLoading", true);

            return axios.get("/api/ProductionAssembly/GetTaskQueue")
                .then(response => {
                    commit("setTaskQueue", response.data);
                })
                .finally(() => {
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
        },

        addToTaskQueue({ commit }, id) {
            commit("setIsLoading", true);

            return axios.post("/api/ProductionAssembly/AddToTaskQueue", id, {
                headers: { "Content-Type": "application/json" }
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        removeFromTaskQueue({ commit, dispatch }, id) {
            commit("setIsLoading", true);

            return axios.delete("/api/ProductionAssembly/RemoveFromTaskQueue", {
                headers: { "Content-Type": "application/json" },
                data: id
            }).then(() => {
                dispatch("loadTaskQueue");
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        login({ commit, dispatch }, { username, password }) {
            commit("setIsLoading", true);

            return axios.post("/api/Authentication/Login", {
                UserLogin: username,
                Password: password
            }).then(response => {
                if (!response.data?.token) {
                    throw new Error("Токен аутентификации не получен");
                }

                localStorage.setItem("authToken", response.data.token);
                axios.defaults.headers.common["Authorization"] = `Bearer ${response.data.token}`;

                dispatch("startSignalRConnection");
                commit("setUser", response.data.userData);
                commit("setIsAuthorized", true);
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        logout({ dispatch }) {
            return axios.post("/api/Authentication/Logout")
                .finally(() => {
                    dispatch("clearAuthData");
                    dispatch("stopSignalRConnection");
                });
        },

        clearAuthData({ commit }) {
            localStorage.removeItem("authToken");
            delete axios.defaults.headers.common["Authorization"];

            commit("setUser", { userId: 0, userName: "", roles: ["Initial"] });
            commit("setIsAuthorized", false);
        },

        register({ commit }, user) {
            commit("setIsLoading", true);

            return axios.post("/api/Authorization/AccountRegister", {
                EmployeeId: user.employeeId,
                Login: user.login,
                Password: user.password,
                Role: user.role
            }).finally(() => commit("setIsLoading", false));
        },

        initializeSignalR({ commit, dispatch, state }) {
            if (state.signalRConnection) {
                state.signalRConnection.stop();
            }

            const connection = new signalR.HubConnectionBuilder()
                .withUrl("https://my-pet-project.ru/TaskQueueNotificationHub", {
                    accessTokenFactory: () => {
                        return localStorage.getItem("authToken");
                    }
                })
                .withAutomaticReconnect()
                .build();

            connection.on("NotifyManagers", (response) => {//Оповещять по accounId а не по employeeId!
                commit("addEmployeeToTaskQueue", response);
                dispatch("loadTaskQueue");
                commit("setAlertMessage", `Сотрудик ${response.fullName} добавлен в очередь на получени задчи.`);
                commit("isShowSuccessAlert", true);
            });

            connection.on("NotifyEmployees", (response) => {
                // commit("addTaskQueue", response);

                if (response.hasNewTaskIssued) {
                    commit("setAlertMessage", `NotifyEmployees ${response.ProductName}`);
                    dispatch("loadProductionTasks");
                } else {
                    commit("setAlertMessage", "Вы удалены из очереди задач");
                }

                commit("isShowSuccessAlert", true);
            });

            connection.onclose(() => {
                commit("setSignalRIsConnected", false);
            });

            commit("setSignalRConnection", connection);

            if (state.isAuthorized) {
                return dispatch("startSignalRConnection");
            }

            return Promise.resolve();
        },

        startSignalRConnection({ commit, state }) {
            if (!state.signalRConnection || state.isSignalRConnected) {
                return Promise.resolve();
            }

            return state.signalRConnection.start()
                .then(() => {
                    commit("setSignalRIsConnected", true);
                })
                .catch(() => {
                    commit("setSignalRIsConnected", false);

                    throw new Error("SignalR connection start is failed.");
                });
        },

        stopSignalRConnection({ commit, state }) {
            if (state.signalRConnection) {
                return state.signalRConnection.stop()
                    .then(() => {
                        commit("setSignalRConnected", false);
                    });
            }

            return Promise.resolve();
        }
    }
})

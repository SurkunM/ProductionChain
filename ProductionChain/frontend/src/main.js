import { createApp } from "vue";
import vuetify from "./plugins/vuetify";
import { loadFonts } from "./plugins/webfontloader";

import router from "./router";
import store from "./store";
import App from "./App.vue";

import axios from "axios";

const authToken = localStorage.getItem("authToken");

if (authToken) {
    axios.defaults.headers.common["Authorization"] = `Bearer ${authToken}`;
}

loadFonts();

createApp(App)
    .use(store)
    .use(router)
    .use(vuetify)
    .mount("#app");

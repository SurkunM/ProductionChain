﻿import { createVuetify } from "vuetify"
import * as components from "vuetify/components"
import * as directives from "vuetify/directives"
import "@mdi/font/css/materialdesignicons.min.css"
import "vuetify/styles"

export default createVuetify({
    components,
    directives,
    theme: {
        defaultTheme: "dark",
    },
});
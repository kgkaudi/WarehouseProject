import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    host: "0.0.0.0",
    port: 5173,
    proxy: {
      "/api": {
        // backend container's published port; override with API_PROXY_TARGET if needed
        target: process.env.API_PROXY_TARGET || "http://localhost:5266",
        changeOrigin: true,
      },
    },
  },
});
import axios from "axios";
import { useAuth } from "../context/AuthContext";
import { useSnackbar } from "../context/SnackbarContext";

// Create axios instance
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5266/api"
});

// Attach interceptors
export function setupInterceptors(logout, showSnackbar) {
  // REQUEST interceptor
  api.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem("token");
      if (token) config.headers.Authorization = `Bearer ${token}`;
      return config;
    },
    (error) => Promise.reject(error)
  );

  // RESPONSE interceptor
  api.interceptors.response.use(
    (response) => response,
    (error) => {
      const status = error.response?.status;

      if (status === 401) {
        const isLoginRequest = error.config?.url?.includes("/auth/login");

        if (!isLoginRequest) {
          showSnackbar("Session expired. Please log in again.", "error");
          logout();
          window.location.href = "/login";
        }
      }

      if (status >= 500) {
        showSnackbar("Server error. Try again later.", "error");
      }

      if (status === 403) {
        showSnackbar("You do not have permission to perform this action.", "warning");
      }

      return Promise.reject(error);
    }
  );
}

export default api;

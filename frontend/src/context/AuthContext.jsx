import { createContext, useContext, useEffect, useState } from "react";
import axios from "axios";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem("token"));
  const [user, setUser] = useState(() => {
    try {
      const stored = localStorage.getItem("user");
      if (!stored || stored === "undefined") return null;
      return JSON.parse(stored);
    } catch {
      return null;
    }
  });

  const isLoggedIn = !!token;
  const role = user?.role || null;

  // ---------------------------------------------------------
  // LOGIN
  // ---------------------------------------------------------
  const login = (jwt, userData) => {
    setToken(jwt);
    setUser(userData);

    localStorage.setItem("token", jwt);
    localStorage.setItem("user", JSON.stringify(userData));
  };

  // ---------------------------------------------------------
  // LOGOUT
  // ---------------------------------------------------------
  const logout = () => {
    setToken(null);
    setUser(null);

    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  // ---------------------------------------------------------
  // AUTO‑LOGOUT WHEN TOKEN EXPIRES
  // ---------------------------------------------------------
  useEffect(() => {
    if (!token) return;

    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      const exp = payload.exp * 1000;
      const now = Date.now();

      const timeout = exp - now;

      if (timeout <= 0) {
        logout();
        return;
      }

      const timer = setTimeout(() => logout(), timeout);
      return () => clearTimeout(timer);
    } catch {
      logout();
    }
  }, [token]);

  // ---------------------------------------------------------
  // AXIOS INTERCEPTOR (with cleanup)
  // ---------------------------------------------------------
  useEffect(() => {
    const interceptor = axios.interceptors.request.use((config) => {
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    return () => {
      axios.interceptors.request.eject(interceptor);
    };
  }, [token]);

  return (
    <AuthContext.Provider
      value={{
        token,
        user,
        role,
        login,
        logout,
        isLoggedIn,
        isAdmin: role === "admin"
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
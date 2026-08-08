import { createContext, useContext, useEffect, useState } from "react";
import { setupInterceptors } from "../api/axios";
import { useSnackbar } from "./SnackbarContext";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const { showSnackbar } = useSnackbar();

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
  // INSTALL AXIOS INTERCEPTORS (after login/logout exist)
  // ---------------------------------------------------------
  useEffect(() => {
    setupInterceptors(logout, showSnackbar);
  }, [logout, showSnackbar]);

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
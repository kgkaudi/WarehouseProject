import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { CssBaseline, Container, Box } from "@mui/material";
import { ThemeProvider, createTheme } from "@mui/material/styles";

import { SnackbarProvider } from "./context/SnackbarContext.jsx";
import { AuthProvider, useAuth } from "./context/AuthContext.jsx";

import { useState } from "react";

import Navbar from "./components/Navbar.jsx";
import MobileDrawer from "./components/MobileDrawer.jsx";

import AuthPage from "./pages/AuthPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import AccountPage from "./pages/AccountPage.jsx";
import VerifyEmailPage from "./pages/VerifyEmailPage.jsx";
import ResetPasswordPage from "./pages/ResetPasswordPage.jsx";
import CreateProductPage from "./pages/CreateProductPage.jsx";
import AdminPage from "./pages/AdminPage.jsx";

function AppContent() {
  const { isLoggedIn } = useAuth();
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [darkMode, setDarkMode] = useState(() => {
    return localStorage.getItem("darkMode") === "true";
  });

  const theme = createTheme({
    palette: {
      mode: darkMode ? "dark" : "light",
      background: {
        default: darkMode ? "#121212" : "#f5f5f5",
        paper: darkMode ? "#1e1e1e" : "#ffffff"
      }
    }
  });

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />

      <Box
        sx={{
          minHeight: "100vh",
          backgroundColor: "background.default",
          transition: "background-color 0.3s ease"
        }}
      >
        <Router>
          <Navbar
            darkMode={darkMode}
            setDarkMode={setDarkMode}
            setDrawerOpen={setDrawerOpen}
          />

          <MobileDrawer
            drawerOpen={drawerOpen}
            setDrawerOpen={setDrawerOpen}
          />

          <Container sx={{ mt: 4 }}>
            <Routes>
              {!isLoggedIn && (
                <>
                  <Route path="/login" element={<AuthPage />} />
                  <Route path="/verify-email" element={<VerifyEmailPage />} />
                  <Route path="/reset-password" element={<ResetPasswordPage />} />
                  <Route path="*" element={<Navigate to="/login" replace />} />
                </>
              )}

              {isLoggedIn && (
                <>
                  <Route path="/products" element={<ProductsPage />} />
                  <Route path="/products/create" element={<CreateProductPage />} />
                  <Route path="/account" element={<AccountPage />} />
                  <Route path="/admin" element={<AdminPage />} />
                  <Route path="*" element={<Navigate to="/products" replace />} />
                </>
              )}
            </Routes>
          </Container>
        </Router>
      </Box>
    </ThemeProvider>
  );
}

export default function App() {
  return (
    <SnackbarProvider>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </SnackbarProvider>
  );
}
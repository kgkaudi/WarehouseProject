import { BrowserRouter as Router, Routes, Route, Navigate, Link } from "react-router-dom";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  Container,
  IconButton,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Box,
  Avatar
} from "@mui/material";

import MenuIcon from "@mui/icons-material/Menu";
import Brightness4Icon from "@mui/icons-material/Brightness4";
import Brightness7Icon from "@mui/icons-material/Brightness7";
import { CssBaseline } from "@mui/material";

import { ThemeProvider, createTheme } from "@mui/material/styles";
import { SnackbarProvider } from "./context/SnackbarContext.jsx";
import { AuthProvider, useAuth } from "./context/AuthContext.jsx";

import { useState } from "react";

import AuthPage from "./pages/AuthPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import AccountPage from "./pages/AccountPage.jsx";
import VerifyEmailPage from "./pages/VerifyEmailPage.jsx";
import ResetPasswordPage from "./pages/ResetPasswordPage.jsx";
import CreateProductPage from "./pages/CreateProductPage.jsx";
import AdminPage from "./pages/AdminPage.jsx";

function AppContent() {
  const { isLoggedIn, logout, role, user } = useAuth();
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

  const navItems = [
    { label: "Products", to: "/products" },
    { label: "Account", to: "/account" }
  ];

  const isActive = (path) => window.location.pathname === path;

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
          {/* NAVBAR */}
          <AppBar position="static">
            <Toolbar>
              {isLoggedIn && (
                <IconButton
                  color="inherit"
                  edge="start"
                  sx={{ mr: 2, display: { xs: "flex", md: "none" } }}
                  onClick={() => setDrawerOpen(true)}
                >
                  <MenuIcon />
                </IconButton>
              )}

              <Typography variant="h6" sx={{ flexGrow: 1 }}>
                Warehouse App
              </Typography>

              {/* DARK MODE */}
              <IconButton
                color="inherit"
                onClick={() => {
                  const newMode = !darkMode;
                  setDarkMode(newMode);
                  localStorage.setItem("darkMode", newMode);
                }}
              >
                {darkMode ? <Brightness7Icon /> : <Brightness4Icon />}
              </IconButton>

              {/* DESKTOP NAV */}
              {isLoggedIn && (
                <Box sx={{ display: { xs: "none", md: "flex" }, gap: 2 }}>
                  {navItems.map((item) => (
                    <Button
                      key={item.to}
                      color="inherit"
                      component={Link}
                      to={item.to}
                      sx={{
                        borderBottom: isActive(item.to) ? "2px solid white" : "none",
                        borderRadius: 0
                      }}
                    >
                      {item.label}
                    </Button>
                  ))}

                  {role === "admin" && (
                    <Button
                      color="inherit"
                      component={Link}
                      to="/admin"
                      sx={{
                        borderBottom: isActive("/admin") ? "2px solid white" : "none",
                        borderRadius: 0
                      }}
                    >
                      Admin
                    </Button>
                  )}

                  <Button color="inherit" onClick={logout}>
                    Logout
                  </Button>

                  <Avatar sx={{ bgcolor: "secondary.main", ml: 2 }}>
                    {user?.username?.[0]?.toUpperCase() || "U"}
                  </Avatar>
                </Box>
              )}
            </Toolbar>
          </AppBar>

          {/* MOBILE DRAWER */}
          <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
            <Box sx={{ width: 250 }}>
              <List>
                {navItems.map((item) => (
                  <ListItem key={item.to} disablePadding>
                    <ListItemButton
                      component={Link}
                      to={item.to}
                      selected={isActive(item.to)}
                      onClick={() => setDrawerOpen(false)}
                    >
                      <ListItemText primary={item.label} />
                    </ListItemButton>
                  </ListItem>
                ))}

                {role === "admin" && (
                  <ListItem disablePadding>
                    <ListItemButton
                      component={Link}
                      to="/admin"
                      selected={isActive("/admin")}
                      onClick={() => setDrawerOpen(false)}
                    >
                      <ListItemText primary="Admin" />
                    </ListItemButton>
                  </ListItem>
                )}

                <ListItem disablePadding>
                  <ListItemButton onClick={logout}>
                    <ListItemText primary="Logout" />
                  </ListItemButton>
                </ListItem>
              </List>
            </Box>
          </Drawer>

          {/* ROUTES */}
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
    <AuthProvider>
      <SnackbarProvider>
        <AppContent />
      </SnackbarProvider>
    </AuthProvider>
  );
}

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

import { useState } from "react";

import AuthPage from "./pages/AuthPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import AccountPage from "./pages/AccountPage.jsx";
import VerifyEmailPage from "./pages/VerifyEmailPage.jsx";
import ResetPasswordPage from "./pages/ResetPasswordPage.jsx";
import CreateProductPage from "./pages/CreateProductPage.jsx";

export default function App() {
  const [loggedIn, setLoggedIn] = useState(!!localStorage.getItem("token"));
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [darkMode, setDarkMode] = useState(false);

  const theme = createTheme({
    palette: {
      mode: darkMode ? "dark" : "light",
      background: {
        default: darkMode ? "#121212" : "#f5f5f5",
        paper: darkMode ? "#1e1e1e" : "#ffffff"
      }
    }
  });

  const logout = () => {
    localStorage.removeItem("token");
    setLoggedIn(false);
    setDrawerOpen(false);
  };

  const navItems = [
    { label: "Products", to: "/products" },
    { label: "Account", to: "/account" }
  ];

  const isActive = (path) => window.location.pathname === path;

  return (
    <SnackbarProvider>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {/* GLOBAL BACKGROUND WRAPPER */}
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
                {loggedIn && (
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

                {/* DARK MODE TOGGLE */}
                <IconButton color="inherit" onClick={() => setDarkMode(!darkMode)}>
                  {darkMode ? <Brightness7Icon /> : <Brightness4Icon />}
                </IconButton>

                {/* DESKTOP NAV */}
                {loggedIn && (
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

                    <Button color="inherit" onClick={logout}>
                      Logout
                    </Button>

                    {/* USER AVATAR */}
                    <Avatar sx={{ bgcolor: "secondary.main", ml: 2 }}>
                      {localStorage.getItem("username")?.[0]?.toUpperCase() || "U"}
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
                {!loggedIn && (
                  <>
                    <Route
                      path="/login"
                      element={<AuthPage onLoggedIn={() => setLoggedIn(true)} />}
                    />
                    <Route path="/verify-email" element={<VerifyEmailPage />} />
                    <Route path="/reset-password" element={<ResetPasswordPage />} />
                    <Route path="*" element={<Navigate to="/login" replace />} />
                  </>
                )}

                {loggedIn && (
                  <>
                    <Route path="/products" element={<ProductsPage />} />
                    <Route path="/products/create" element={<CreateProductPage />} />
                    <Route path="/account" element={<AccountPage />} />
                    <Route path="*" element={<Navigate to="/products" replace />} />
                  </>
                )}
              </Routes>
            </Container>
          </Router>
        </Box>
      </ThemeProvider>
    </SnackbarProvider>
  );
}

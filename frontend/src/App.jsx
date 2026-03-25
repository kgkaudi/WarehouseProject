import { BrowserRouter as Router, Routes, Route, Navigate, Link } from "react-router-dom";
import { AppBar, Toolbar, Typography, Button, Container } from "@mui/material";
import { useState } from "react";

import AuthPage from "./pages/AuthPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import AccountPage from "./pages/AccountPage.jsx";
import VerifyEmailPage from "./pages/VerifyEmailPage.jsx";
import ResetPasswordPage from "./pages/ResetPasswordPage.jsx";
import CreateProductPage from "./pages/CreateProductPage.jsx";

export default function App() {
  const [loggedIn, setLoggedIn] = useState(!!localStorage.getItem("token"));

  const logout = () => {
    localStorage.removeItem("token");
    setLoggedIn(false);
  };

  return (
    <Router>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            Warehouse App
          </Typography>

          {loggedIn && (
            <>
              <Button color="inherit" component={Link} to="/products">
                Products
              </Button>
              <Button color="inherit" component={Link} to="/account">
                Account
              </Button>
              <Button color="inherit" onClick={logout}>
                Logout
              </Button>
            </>
          )}
        </Toolbar>
      </AppBar>

      <Container sx={{ mt: 4 }}>
        <Routes>
          {/* Public routes */}
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

          {/* Private routes */}
          {loggedIn && (
            <>
              <Route path="/products" element={<ProductsPage />} />
              <Route path="/account" element={<AccountPage />} />
              <Route path="*" element={<Navigate to="/products" replace />} />
              <Route path="/products/create" element={<CreateProductPage />} />
            </>
          )}
        </Routes>
      </Container>
    </Router>
  );
}

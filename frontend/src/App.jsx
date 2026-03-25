import { useState } from "react";
import { AppBar, Toolbar, Typography, Button, Container } from "@mui/material";
import AuthPage from "./pages/AuthPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import AccountPage from "./pages/AccountPage.jsx";

export default function App() {
  const [view, setView] = useState("auth");
  const [loggedIn, setLoggedIn] = useState(!!localStorage.getItem("token"));

  const logout = () => {
    localStorage.removeItem("token");
    setLoggedIn(false);
    setView("auth");
  };

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            Warehouse App
          </Typography>
          {loggedIn && (
            <>
              <Button color="inherit" onClick={() => setView("products")}>
                Products
              </Button>
              <Button color="inherit" onClick={() => setView("account")}>
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
        {!loggedIn && (
          <AuthPage
            onLoggedIn={() => {
              setLoggedIn(true);
              setView("products");
            }}
          />
        )}
        {loggedIn && view === "products" && <ProductsPage />}
        {loggedIn && view === "account" && <AccountPage />}
      </Container>
    </>
  );
}

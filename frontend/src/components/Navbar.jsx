import { Link } from "react-router-dom";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  IconButton,
  Box,
  Avatar
} from "@mui/material";

import MenuIcon from "@mui/icons-material/Menu";
import Brightness4Icon from "@mui/icons-material/Brightness4";
import Brightness7Icon from "@mui/icons-material/Brightness7";

import { useAuth } from "../context/AuthContext.jsx";

export default function Navbar({ darkMode, setDarkMode, setDrawerOpen }) {
  const { isLoggedIn, logout, role, user } = useAuth();

  const navItems = [
    { label: "Products", to: "/products" },
    { label: "Account", to: "/account" }
  ];

  const isActive = (path) => window.location.pathname === path;

  return (
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
  );
}

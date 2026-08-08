import { Link } from "react-router-dom";
import {
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  Box
} from "@mui/material";

import { useAuth } from "../context/AuthContext.jsx";

export default function MobileDrawer({ drawerOpen, setDrawerOpen }) {
  const { logout, role } = useAuth();

  const navItems = [
    { label: "Products", to: "/products" },
    { label: "Account", to: "/account" }
  ];

  const isActive = (path) => window.location.pathname === path;

  return (
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
  );
}
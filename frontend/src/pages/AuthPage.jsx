import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";
import api from "../api/axios";

import {
  TextField,
  IconButton,
  InputAdornment,
  Box,
  Button,
  Typography,
  Paper,
  Stack,
  Snackbar,
  Alert
} from "@mui/material";

import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";

export default function AuthPage() {
  const { login: authLogin } = useAuth();
  const navigate = useNavigate();

  const [mode, setMode] = useState("login");
  const [verificationToken, setVerificationToken] = useState("");

  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
    companyName: "",
    companyAddress: "",
  });

  const [showPassword, setShowPassword] = useState(false);

  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success"
  });

  const showSnackbar = (message, severity = "success") => {
    setSnackbar({ open: true, message, severity });
  };

  const handleChange = (field) => (e) =>
    setForm({ ...form, [field]: e.target.value });

  // ---------------------------------------------------------
  // REGISTER
  // ---------------------------------------------------------
  const register = async () => {
    try {
      const res = await api.post("/auth/register", {
        username: form.username,
        email: form.email,
        password: form.password,
        companyName: form.companyName,
        companyAddress: form.companyAddress
      });

      setVerificationToken(res.data.verificationToken);
      showSnackbar("Registered successfully! Your verification token is shown below.");
    } catch {
      showSnackbar("Registration failed. Check your details.", "error");
    }
  };

  // ---------------------------------------------------------
  // LOGIN
  // ---------------------------------------------------------
  const login = async () => {
    try {
      const res = await api.post("/auth/login", {
        identifier: form.username,
        password: form.password
      });

      const { token, username, role, companyName, companyAddress } = res.data;

      const userData = {
        username,
        role,
        companyName,
        companyAddress
      };

      authLogin(token, userData);

      showSnackbar("Login successful!");
      navigate("/products");
    } catch {
      showSnackbar("Invalid username or password.", "error");
    }
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 500, mx: "auto" }}>
      <Stack spacing={2}>
        <Typography variant="h5">
          {mode === "login" ? "Login" : "Register"}
        </Typography>

        {/* REGISTER MODE */}
        {mode === "register" && (
          <>
            <TextField label="Username" value={form.username} onChange={handleChange("username")} />
            <TextField label="Email" value={form.email} onChange={handleChange("email")} />

            <TextField
              label="Password"
              type={showPassword ? "text" : "password"}
              value={form.password}
              onChange={handleChange("password")}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setShowPassword(!showPassword)} edge="end">
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />

            <TextField label="Company Name" value={form.companyName} onChange={handleChange("companyName")} />
            <TextField label="Company Address" value={form.companyAddress} onChange={handleChange("companyAddress")} />

            <Button variant="contained" onClick={register}>Register</Button>

            {verificationToken && (
              <Box sx={{ p: 2, mb: 2, borderRadius: 1, bgcolor: "#e3f2fd", border: "1px solid #90caf9" }}>
                <Typography variant="subtitle1" sx={{ mb: 1 }}>
                  Your verification token
                </Typography>

                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                  <TextField value={verificationToken} fullWidth InputProps={{ readOnly: true }} />
                  <Button variant="contained" onClick={() => navigator.clipboard.writeText(verificationToken)}>
                    Copy
                  </Button>
                </Box>
              </Box>
            )}

            <Button onClick={() => setMode("login")}>Already have an account? Login</Button>
          </>
        )}

        {/* LOGIN MODE */}
        {mode === "login" && (
          <>
            <TextField
              label="Username / Email"
              value={form.username}
              onChange={handleChange("username")}
            />

            <TextField
              label="Password"
              type={showPassword ? "text" : "password"}
              value={form.password}
              onChange={handleChange("password")}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setShowPassword(!showPassword)} edge="end">
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />

            <Button variant="contained" onClick={login}>Login</Button>
            <Button onClick={() => setMode("register")}>
              Need an account? Register
            </Button>
          </>
        )}

        <Button onClick={() => navigate("/verify-email")}>Verify Email</Button>
        <Button onClick={() => navigate("/reset-password")}>Forgot Password?</Button>
      </Stack>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={3000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackbar({ ...snackbar, open: false })}
          severity={snackbar.severity}
          sx={{ width: "100%" }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Paper>
  );
}
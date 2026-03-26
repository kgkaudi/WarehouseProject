import { useState } from "react";
import { useNavigate } from "react-router-dom";

import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  Stack,
  Snackbar,
  Alert
} from "@mui/material";
import api from "../api";

export default function AuthPage({ onLoggedIn }) {
  const [verificationToken, setVerificationToken] = useState("");
  const [mode, setMode] = useState("login");

  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success"
  });

  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
    companyName: "",
    companyAddress: "",
    resetEmail: "",
    resetToken: "",
    newPassword: ""
  });

  const navigate = useNavigate();

  const handleChange = (field) => (e) =>
    setForm({ ...form, [field]: e.target.value });

  const showSnackbar = (message, severity = "success") => {
    setSnackbar({ open: true, message, severity });
  };

  const switchMode = (newMode) => {
    setMode(newMode);
  };

  // -----------------------------
  // REGISTER
  // -----------------------------
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
      showSnackbar("Registered successfully! Your verification token is shown below.", "success");

    } catch {
      showSnackbar("Registration failed. Check your details.", "error");
    }
  };

  // -----------------------------
  // LOGIN
  // -----------------------------
  const login = async () => {
    try {
      const res = await api.post("/auth/login", {
        username: form.username,
        password: form.password
      });

      showSnackbar("Login successful!", "success");
      localStorage.setItem("token", res.data.token);
      localStorage.setItem("username", res.data.username);
      onLoggedIn();

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
            <TextField label="Password" type="password" value={form.password} onChange={handleChange("password")} />
            <TextField label="Company Name" value={form.companyName} onChange={handleChange("companyName")} />
            <TextField label="Company Address" value={form.companyAddress} onChange={handleChange("companyAddress")} />

            <Button variant="contained" onClick={register}>Register</Button>

            {/* VERIFICATION TOKEN BOX */}
            {verificationToken && (
              <Box
                sx={{
                  p: 2,
                  mb: 2,
                  borderRadius: 1,
                  bgcolor: "#e3f2fd",
                  border: "1px solid #90caf9"
                }}
              >
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

            <Button onClick={() => switchMode("login")}>Already have an account? Login</Button>
          </>
        )}

        {/* LOGIN MODE */}
        {mode === "login" && (
          <>
            <TextField label="Username" value={form.username} onChange={handleChange("username")} />
            <TextField label="Password" type="password" value={form.password} onChange={handleChange("password")} />

            <Button variant="contained" onClick={login}>Login</Button>
            <Button onClick={() => switchMode("register")}>Need an account? Register</Button>
          </>
        )}

        <Button onClick={() => navigate("/verify-email")}>Verify Email</Button>
        <Button onClick={() => navigate("/reset-password")}>Forgot Password?</Button>
      </Stack>

      {/* SNACKBAR */}
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

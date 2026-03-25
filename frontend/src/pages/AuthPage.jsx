import { useState } from "react";
import {
  Box,
  TextField,
  Button,
  Typography,
  Paper,
  Stack
} from "@mui/material";
import api from "../api";

export default function AuthPage({ onLoggedIn }) {
  const [mode, setMode] = useState("login");
  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
    companyName: "",
    companyAddress: "",
    verificationToken: "",
    resetEmail: "",
    resetToken: "",
    newPassword: ""
  });

  const handleChange = (field) => (e) =>
    setForm({ ...form, [field]: e.target.value });

  const register = async () => {
    const res = await api.post("/auth/register", {
      username: form.username,
      email: form.email,
      password: form.password,
      companyName: form.companyName,
      companyAddress: form.companyAddress
    });
    alert(
      "Registered. Verification token:\n" + res.data.verificationToken
    );
  };

  const verifyEmail = async () => {
    await api.post(`/auth/verify-email?token=${form.verificationToken}`);
    alert("Email verified. You can now log in.");
  };

  const login = async () => {
    const res = await api.post("/auth/login", {
      username: form.username,
      password: form.password
    });
    localStorage.setItem("token", res.data.token);
    onLoggedIn();
  };

  const requestReset = async () => {
    const res = await api.post("/auth/request-password-reset", {
      email: form.resetEmail
    });
    alert("Reset token:\n" + res.data.resetToken);
  };

  const resetPassword = async () => {
    await api.post("/auth/reset-password", {
      token: form.resetToken,
      newPassword: form.newPassword
    });
    alert("Password reset. You can log in with new password.");
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 500, mx: "auto" }}>
      <Stack spacing={2}>
        <Typography variant="h5">
          {mode === "login" ? "Login" : "Register"}
        </Typography>

        {mode === "register" && (
          <>
            <TextField
              label="Username"
              value={form.username}
              onChange={handleChange("username")}
            />
            <TextField
              label="Email"
              value={form.email}
              onChange={handleChange("email")}
            />
            <TextField
              label="Password"
              type="password"
              value={form.password}
              onChange={handleChange("password")}
            />
            <TextField
              label="Company Name"
              value={form.companyName}
              onChange={handleChange("companyName")}
            />
            <TextField
              label="Company Address"
              value={form.companyAddress}
              onChange={handleChange("companyAddress")}
            />
            <Button variant="contained" onClick={register}>
              Register
            </Button>
            <Button onClick={() => setMode("login")}>
              Already have an account? Login
            </Button>
          </>
        )}

        {mode === "login" && (
          <>
            <TextField
              label="Username"
              value={form.username}
              onChange={handleChange("username")}
            />
            <TextField
              label="Password"
              type="password"
              value={form.password}
              onChange={handleChange("password")}
            />
            <Button variant="contained" onClick={login}>
              Login
            </Button>
            <Button onClick={() => setMode("register")}>
              Need an account? Register
            </Button>
          </>
        )}

        <Box sx={{ mt: 3 }}>
          <Typography variant="subtitle1">Email Verification</Typography>
          <TextField
            label="Verification Token"
            fullWidth
            value={form.verificationToken}
            onChange={handleChange("verificationToken")}
            sx={{ mt: 1 }}
          />
          <Button sx={{ mt: 1 }} onClick={verifyEmail}>
            Verify Email
          </Button>
        </Box>

        <Box sx={{ mt: 3 }}>
          <Typography variant="subtitle1">Password Reset</Typography>
          <TextField
            label="Email"
            fullWidth
            value={form.resetEmail}
            onChange={handleChange("resetEmail")}
            sx={{ mt: 1 }}
          />
          <Button sx={{ mt: 1 }} onClick={requestReset}>
            Request Reset Token
          </Button>
          <TextField
            label="Reset Token"
            fullWidth
            value={form.resetToken}
            onChange={handleChange("resetToken")}
            sx={{ mt: 1 }}
          />
          <TextField
            label="New Password"
            type="password"
            fullWidth
            value={form.newPassword}
            onChange={handleChange("newPassword")}
            sx={{ mt: 1 }}
          />
          <Button sx={{ mt: 1 }} onClick={resetPassword}>
            Reset Password
          </Button>
        </Box>
      </Stack>
    </Paper>
  );
}

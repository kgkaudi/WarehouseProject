import { useState } from "react";
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
import { useNavigate } from "react-router-dom";
import api from "../api";

export default function ResetPasswordPage() {
  const [form, setForm] = useState({
    email: "",
    token: "",
    newPassword: ""
  });

  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success"
  });

  const navigate = useNavigate();

  const handleChange = (field) => (e) =>
    setForm({ ...form, [field]: e.target.value });

  const showSnackbar = (message, severity = "success") => {
    setSnackbar({ open: true, message, severity });
  };

  const requestReset = async () => {
    try {
      const res = await api.post("/auth/request-password-reset", {
        email: form.email
      });

      showSnackbar("Reset token generated: " + res.data.resetToken, "success");
    } catch {
      showSnackbar("Failed to request password reset.", "error");
    }
  };

  const resetPassword = async () => {
    try {
      await api.post("/auth/reset-password", {
        token: form.token,
        newPassword: form.newPassword
      });

      showSnackbar("Password reset successful! Redirecting to login...", "success");

      // Redirect after 2 seconds
      setTimeout(() => navigate("/login"), 2000);

    } catch {
      showSnackbar("Invalid or expired reset token.", "error");
    }
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 500, mx: "auto", mt: 5 }}>
      <Stack spacing={2}>
        <Typography variant="h5">Password Reset</Typography>

        <TextField
          label="Email"
          fullWidth
          value={form.email}
          onChange={handleChange("email")}
        />

        <Button variant="contained" onClick={requestReset}>
          Request Reset Token
        </Button>

        <TextField
          label="Reset Token"
          fullWidth
          value={form.token}
          onChange={handleChange("token")}
        />

        <TextField
          label="New Password"
          type="password"
          fullWidth
          value={form.newPassword}
          onChange={handleChange("newPassword")}
        />

        <Button variant="contained" onClick={resetPassword}>
          Reset Password
        </Button>
      </Stack>

      {/* Snackbar Notification */}
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

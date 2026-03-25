import { useState } from "react";
import { Box, TextField, Button, Typography, Paper, Stack, Snackbar, Alert } from "@mui/material";
import { useNavigate } from "react-router-dom";
import api from "../api";

export default function VerifyEmailPage() {
  const [token, setToken] = useState("");

  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success"
  });

  const navigate = useNavigate();

  const showSnackbar = (message, severity = "success") => {
    setSnackbar({ open: true, message, severity });
  };

  const verifyEmail = async () => {
    try {
      await api.post(`/auth/verify-email?token=${token}`);

      showSnackbar("Email verified! Redirecting to login...", "success");

      setTimeout(() => navigate("/login"), 2000);

    } catch {
      showSnackbar("Invalid or expired token.", "error");
    }
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 500, mx: "auto", mt: 5 }}>
      <Stack spacing={2}>
        <Typography variant="h5">Verify Email</Typography>

        <TextField
          label="Verification Token"
          fullWidth
          value={token}
          onChange={(e) => setToken(e.target.value)}
        />

        <Button variant="contained" onClick={verifyEmail}>
          Verify Email
        </Button>
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

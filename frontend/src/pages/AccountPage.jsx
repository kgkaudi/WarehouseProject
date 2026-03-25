import { useState } from "react";
import {
  Paper,
  Typography,
  TextField,
  Button,
  Stack,
  Box,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from "@mui/material";
import api from "../api";

export default function AccountPage() {
  const [changeForm, setChangeForm] = useState({
    currentPassword: "",
    newPassword: ""
  });

  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const [deleteOpen, setDeleteOpen] = useState(false);

  const handleChange = (field) => (e) =>
    setChangeForm({ ...changeForm, [field]: e.target.value });

  const changePassword = async () => {
    try {
      await api.post("/auth/change-password", changeForm);
      setError("");
      setMessage("Password changed successfully.");
      setChangeForm({ currentPassword: "", newPassword: "" });
    } catch (err) {
      setMessage("");
      setError("Current password incorrect.");
    }
  };

  const deleteAccount = async () => {
    try {
      await api.delete("/auth/delete-account");
      setError("");
      setMessage("Account deleted. Please clear your token and log out.");
      setDeleteOpen(false);
    } catch (err) {
      setMessage("");
      setError("Failed to delete account.");
    }
  };

  return (
    <Stack spacing={3}>
      <Paper sx={{ p: 2 }}>
        <Typography variant="h6">Change Password</Typography>

        {message && (
          <Box
            sx={{
              p: 2,
              mt: 2,
              borderRadius: 1,
              bgcolor: "#e8f5e9",
              border: "1px solid #81c784",
              color: "#2e7d32"
            }}
          >
            {message}
          </Box>
        )}

        {error && (
          <Box
            sx={{
              p: 2,
              mt: 2,
              borderRadius: 1,
              bgcolor: "#ffebee",
              border: "1px solid #ef9a9a",
              color: "#c62828"
            }}
          >
            {error}
          </Box>
        )}

        <Stack spacing={2} sx={{ mt: 2 }}>
          <TextField
            label="Current Password"
            type="password"
            value={changeForm.currentPassword}
            onChange={handleChange("currentPassword")}
          />
          <TextField
            label="New Password"
            type="password"
            value={changeForm.newPassword}
            onChange={handleChange("newPassword")}
          />
          <Button variant="contained" onClick={changePassword}>
            Change Password
          </Button>
        </Stack>
      </Paper>

      <Paper sx={{ p: 2 }}>
        <Typography variant="h6" color="error">
          Danger Zone
        </Typography>

        <Button
          color="error"
          variant="contained"
          onClick={() => setDeleteOpen(true)}
        >
          Delete Account
        </Button>
      </Paper>

      {/* Confirmation Dialog */}
      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete Account</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete your account? This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={deleteAccount}>
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
}

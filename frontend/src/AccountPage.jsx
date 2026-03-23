import { useState } from "react";
import {
  Paper,
  Typography,
  TextField,
  Button,
  Stack
} from "@mui/material";
import api from "./api";

export default function AccountPage() {
  const [changeForm, setChangeForm] = useState({
    currentPassword: "",
    newPassword: ""
  });

  const handleChange = (field) => (e) =>
    setChangeForm({ ...changeForm, [field]: e.target.value });

  const changePassword = async () => {
    await api.post("/auth/change-password", changeForm);
    alert("Password changed");
    setChangeForm({ currentPassword: "", newPassword: "" });
  };

  const deleteAccount = async () => {
    if (!confirm("Are you sure you want to delete your account?")) return;
    await api.delete("/auth/delete-account");
    alert("Account deleted. Clear token manually in this dev UI.");
  };

  return (
    <Stack spacing={3}>
      <Paper sx={{ p: 2 }}>
        <Typography variant="h6">Change Password</Typography>
        <Stack spacing={2} sx={{ mt: 1 }}>
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
        <Button color="error" variant="contained" onClick={deleteAccount}>
          Delete Account
        </Button>
      </Paper>
    </Stack>
  );
}

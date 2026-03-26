import { useEffect, useState } from "react";
import {
  Paper,
  Typography,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Stack,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box
} from "@mui/material";

import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import ArrowUpwardIcon from "@mui/icons-material/ArrowUpward";
import ArrowDownwardIcon from "@mui/icons-material/ArrowDownward";

import api from "../api";
import { useSnackbar } from "../context/SnackbarContext";

export default function AdminPage() {
  const [users, setUsers] = useState([]);
  const [editOpen, setEditOpen] = useState(false);
  const [editForm, setEditForm] = useState({
    id: "",
    username: "",
    companyName: "",
    companyAddress: ""
  });

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState(null);

  const { showSnackbar } = useSnackbar();

  const load = async () => {
    const res = await api.get("/users");
    setUsers(res.data);
  };

  useEffect(() => {
    load();
  }, []);

  const openEdit = (u) => {
    setEditForm({
      id: u.id,
      username: u.username,
      companyName: u.companyName,
      companyAddress: u.companyAddress
    });
    setEditOpen(true);
  };

  const saveEdit = async () => {
    await api.put(`/users/${editForm.id}`, editForm);
    showSnackbar("User updated", "success");
    setEditOpen(false);
    load();
  };

  const remove = async () => {
    await api.delete(`/users/${deleteId}`);
    showSnackbar("User deleted", "success");
    setDeleteOpen(false);
    load();
  };

  const promote = async (id) => {
    await api.post(`/users/promote/${id}`);
    showSnackbar("User promoted to admin", "success");
    load();
  };

  const demote = async (id) => {
    await api.post(`/users/demote/${id}`);
    showSnackbar("User demoted to user", "success");
    load();
  };

  return (
    <Stack spacing={3}>
      <Paper sx={{ p: 2 }}>
        <Typography variant="h5">Admin — User Management</Typography>

        <List>
          {users.map((u) => (
            <ListItem
              key={u.id}
              secondaryAction={
                <Box sx={{ display: "flex", gap: 1 }}>
                  <IconButton onClick={() => openEdit(u)}>
                    <EditIcon />
                  </IconButton>

                  <IconButton
                    onClick={() => {
                      setDeleteId(u.id);
                      setDeleteOpen(true);
                    }}
                  >
                    <DeleteIcon color="error" />
                  </IconButton>

                  {u.role !== "admin" ? (
                    <IconButton onClick={() => promote(u.id)}>
                      <ArrowUpwardIcon color="success" />
                    </IconButton>
                  ) : (
                    <IconButton onClick={() => demote(u.id)}>
                      <ArrowDownwardIcon color="warning" />
                    </IconButton>
                  )}
                </Box>
              }
            >
              <ListItemText
                primary={`${u.username} (${u.role})`}
                secondary={`Company: ${u.companyName} — Products: ${u.products.length}`}
              />
            </ListItem>
          ))}
        </List>
      </Paper>

      {/* EDIT DIALOG */}
      <Dialog open={editOpen} onClose={() => setEditOpen(false)}>
        <DialogTitle>Edit User</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="Username"
              value={editForm.username}
              onChange={(e) => setEditForm({ ...editForm, username: e.target.value })}
            />
            <TextField
              label="Company Name"
              value={editForm.companyName}
              onChange={(e) => setEditForm({ ...editForm, companyName: e.target.value })}
            />
            <TextField
              label="Company Address"
              value={editForm.companyAddress}
              onChange={(e) => setEditForm({ ...editForm, companyAddress: e.target.value })}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={saveEdit}>
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* DELETE CONFIRMATION */}
      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete User</DialogTitle>
        <DialogContent>
          <Typography>This will delete the user and all their products.</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={remove}>
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
}

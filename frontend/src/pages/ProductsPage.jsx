import { useEffect, useState } from "react";
import {
  Paper,
  Typography,
  TextField,
  Button,
  Stack,
  List,
  Box,
  ListItem,
  ListItemText,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from "@mui/material";
import { useNavigate } from "react-router-dom";

import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";

import api from "../api";

export default function ProductsPage() {
  const [products, setProducts] = useState([]);

  const [editOpen, setEditOpen] = useState(false);
  const [editForm, setEditForm] = useState({
    id: null,
    name: "",
    description: "",
    dimensions: "",
    price: "",
    quantity: "",
    weight: ""
  });

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState(null);

  const navigate = useNavigate();

  const load = async () => {
    const res = await api.get("/products/mine");
    setProducts(res.data);
  };

  useEffect(() => {
    load();
  }, []);

  const handleEditChange = (field) => (e) =>
    setEditForm({ ...editForm, [field]: e.target.value });

  const openEdit = (product) => {
    setEditForm(product);
    setEditOpen(true);
  };

  const saveEdit = async () => {
    await api.put(`/products/${editForm.id}`, {
      name: editForm.name,
      description: editForm.description,
      dimensions: editForm.dimensions,
      price: parseFloat(editForm.price),
      quantity: parseInt(editForm.quantity) ?? "",
      weight: parseFloat(editForm.weight)
    });
    setEditOpen(false);
    load();
  };

  const remove = async () => {
    await api.delete(`/products/${deleteId}`);
    setDeleteOpen(false);
    load();
  };

  return (
    <Stack spacing={3}>
      <Button
        variant="contained"
        sx={{ mb: 2, alignSelf: "flex-start" }}
        onClick={() => navigate("/products/create")}
      >
        Create Product
      </Button>

      <Paper sx={{ p: 2 }}>
        <Typography variant="h6">My Products</Typography>
        <List>
          {products.map((p) => (
            <ListItem
              key={p.id}
              disableGutters
              secondaryAction={
                <Box sx={{ display: "flex", gap: 1, minWidth: 80 }}>
                  <IconButton onClick={() => openEdit(p)}>
                    <EditIcon />
                  </IconButton>

                  <IconButton
                    onClick={() => {
                      setDeleteId(p.id);
                      setDeleteOpen(true);
                    }}
                  >
                    <DeleteIcon color="error" />
                  </IconButton>
                </Box>
              }
            >
              <ListItemText
                primary={`${p.name} — ${p.price} SEK`}
                secondary={
                  <>
                    {p.description}
                    <br />
                    Dimensions: {p.dimensions}
                    <br />
                    Weight: {p.weight} kg
                    <br />
                    Quantity: {p.quantity}
                  </>
                }
                sx={{ pr: 10 }}   // <-- gives space so text never overlaps icons
              />
            </ListItem>
          ))}
        </List>
      </Paper>

      {/* EDIT DIALOG */}
      <Dialog open={editOpen} onClose={() => setEditOpen(false)}>
        <DialogTitle>Edit Product</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              label="Name"
              value={editForm.name}
              onChange={handleEditChange("name")}
            />
            <TextField
              label="Description"
              value={editForm.description}
              onChange={handleEditChange("description")}
            />
            <TextField
              label="Dimensions"
              value={editForm.dimensions}
              onChange={handleEditChange("dimensions")}
            />
            <TextField
              label="Price"
              type="number"
              value={editForm.price}
              onChange={handleEditChange("price")}
            />
            <TextField
              label="Quantity"
              type="number"
              value={editForm.quantity}
              onChange={handleEditChange("quantity")}
            />
            <TextField
              label="Weight"
              type="number"
              value={editForm.weight}
              onChange={handleEditChange("weight")}
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

      {/* DELETE CONFIRMATION DIALOG */}
      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete Product</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete this product? This action cannot be undone.
          </Typography>
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

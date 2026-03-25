import { useEffect, useState } from "react";
import {
  Paper,
  Typography,
  TextField,
  Button,
  Stack,
  List,
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

  const load = async () => {
    const res = await api.get("/products/mine");
    setProducts(res.data);
  };

  useEffect(() => {
    load();
  }, []);

  const navigate = useNavigate();

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

  const remove = async (id) => {
    if (!confirm("Delete this product?")) return;
    await api.delete(`/products/${id}`);
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
              secondaryAction={
                <>
                  <IconButton onClick={() => openEdit(p)}>
                    <EditIcon />
                  </IconButton>
                  <IconButton onClick={() => remove(p.id)}>
                    <DeleteIcon color="error" />
                  </IconButton>
                </>
              }
            >
              <ListItemText
                primary={`${p.name} — ${p.price} SEK`}
                secondary={p.description}
              />
            </ListItem>
          ))}
        </List>
      </Paper>

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
    </Stack>
  );
}

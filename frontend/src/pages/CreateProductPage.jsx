import { useState } from "react";
import {
  Paper,
  Typography,
  TextField,
  Button,
  Stack,
  Snackbar,
  Alert
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import api from "../api";

export default function CreateProductPage() {
  const [form, setForm] = useState({
    name: "",
    description: "",
    dimensions: "",
    price: "",
    quantity: "",
    weight: ""
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

  const create = async () => {
    try {
      await api.post("/products", {
        name: form.name,
        description: form.description,
        dimensions: form.dimensions,
        price: parseFloat(form.price),
        quantity: parseInt(form.quantity) ?? "",
        weight: parseFloat(form.weight)
      });

      showSnackbar("Product created successfully!", "success");

      setTimeout(() => navigate("/products"), 1500);

    } catch {
      showSnackbar("Failed to create product.", "error");
    }
  };

  return (
    <Paper sx={{ p: 3, maxWidth: 500, mx: "auto", mt: 4 }}>
      <Typography variant="h5">Create Product</Typography>

      <Stack spacing={2} sx={{ mt: 2 }}>
        <TextField label="Name" value={form.name} onChange={handleChange("name")} />
        <TextField label="Description" value={form.description} onChange={handleChange("description")} />
        <TextField label="Dimensions" value={form.dimensions} onChange={handleChange("dimensions")} />
        <TextField label="Price" type="number" value={form.price} onChange={handleChange("price")} />
        <TextField label="Quantity" type="number" value={form.quantity} onChange={handleChange("quantity")} />
        <TextField label="Weight" type="number" value={form.weight} onChange={handleChange("weight")} />

        <Button variant="contained" onClick={create}>
          Create
        </Button>
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

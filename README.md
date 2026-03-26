# 📦 Warehouse Management System

A full‑stack warehouse management application built with:

- **ASP.NET Core 10** (Backend API)
- **MongoDB** (Database)
- **React + Vite + Material UI** (Frontend)
- **xUnit + Moq** (Automated tests)

This project provides user authentication, product management, and an admin dashboard for managing users and their products.

---

## 🚀 Features

### 🔐 Authentication
- Register, login, email verification  
- JWT‑based authentication  
- Password hashing + salting  
- Reset password flow  

### 📦 Product Management
- Create, update, delete products  
- View your own products  
- Product validation  
- Clean Material UI interface  

### 🛠 Admin Dashboard
- View all users  
- Edit user details  
- Promote/demote users (admin/user)  
- Delete users + their products  

### 🎨 UI / UX
- Dark/light mode with persistence  
- Responsive navbar + drawer  
- Snackbar notifications  
- Dialog‑based confirmations  
- Clean, modern Material UI design  

---

## 🧰 Tech Stack

| Layer | Technology |
|-------|------------|
| **Backend** | ASP.NET Core 10, MongoDB Driver, JWT |
| **Frontend** | React, Vite, Material UI |
| **Database** | MongoDB |
| **Testing** | xUnit, Moq |
| **Build Tools** | .NET CLI, npm |

---

## 📂 Project Structure

WarehouseProject/
│
├── backend/               # ASP.NET Core API
│   ├── Controllers/
│   ├── DTOs/
│   ├── Models/
│   ├── Repositories/
│   ├── Services/
│   └── backend.csproj
│
├── backend.Tests/         # xUnit test project
│   ├── AuthControllerTests.cs
│   ├── UsersControllerTests.cs
│   └── ProductsControllerTests.cs
│
└── frontend/              # React + Vite frontend
├── src/
├── public/
└── package.json


---

## 🏁 Getting Started

### 1️⃣ Clone the repository

```bash
git clone https://github.com/kgkaudi/WarehouseProject.git
cd WarehouseProject

### 🖥 Backend Setup (ASP.NET Core)
Install dependencies

dotnet restore

## Run the backend
dotnet clean
dotnet build
dotnet run

The API will start on:
http://localhost:5000

🌐 Frontend Setup (React + Vite)
Install dependencies

cd frontend
npm install

Run the frontend

npm run dev

The app will start on:
http://localhost:5173

🧪 Running Tests
From the project root:

dotnet clean
dotnet build
dotnet test

🔧 Environment Variables
Your backend requires a MongoDB connection string.
Create a file:

backend/appsettings.Development.json

Example:

{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "your-app",
    "Audience": "your-app"
  }
}

📜 License
This project is for personal and educational use.

🙌 Author
Kostas — Full‑stack developer passionate about clean architecture, testing, and polished UI/UX.
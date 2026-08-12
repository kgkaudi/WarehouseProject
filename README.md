---

# 📦 Warehouse Management System

A full‑stack warehouse management application built with:

- **ASP.NET Core 10** (backend architecture)
- **MongoDB** (database design)
- **React + Vite + Material UI** (frontend structure)
- **xUnit + Moq** (automated testing)

This project provides user authentication, product management, and an admin dashboard for managing users and their products.

---

## 🚀 Features

### 🔐 Authentication
- Register, login, and email verification  
- JWT‑based authentication  
- Password hashing + salting  
- Reset password flow  
- Session expiration handling  

### 📦 Product Management
- Create, update, and delete products  
- View your own products  
- MongoDB‑backed repository layer  
- Validation and clean Material UI interface  

### 🛠 Admin Dashboard
- View all users  
- Edit user details  
- Promote/demote users (admin/user)  
- Delete users + cascade delete their products  

### 🎨 UI / UX
- Dark/light mode with persistence  
- Responsive navbar + mobile drawer  
- Snackbar notifications  
- Dialog‑based confirmations  
- Clean, modern Material UI design  

---

## 🧰 Tech Stack

| Layer | Technology |
|-------|-------------|
| **Backend** | ASP.NET Core 10, MongoDB Driver, JWT |
| **Frontend** | React, Vite, Material UI |
| **Database** | MongoDB |
| **Testing** | xUnit, Moq |
| **Build Tools** | .NET CLI, npm |

---

## 📂 Project Structure

```
WarehouseDotnetProject/
│
├── backend/                # ASP.NET Core API
│   ├── Controllers/
│   ├── DTOs/
│   ├── Models/
│   ├── Repositories/
│   ├── Seed/
│   ├── Service/
│   ├── Properties/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── backend.csproj
│
├── backend.Tests/          # xUnit test project
│   ├── Controllers/
│   ├── Repositories/
│   ├── Services/
│   ├── Shared/
│   └── backend.Tests.csproj
│
└── frontend/               # React + Vite frontend
    ├── src/
    │   ├── api/
    │   ├── components/
    │   ├── context/
    │   ├── pages/
    │   └── App.jsx
    ├── public/
    ├── .env
    ├── vite.config.js
    ├── eslint.config.js
    └── package.json
```

---

## 🏁 Getting Started

### 1️⃣ Clone the repository

```bash
git clone https://github.com/kgkaudi/WarehouseProject.git
cd WarehouseProject
```

---

## 🖥 Backend Setup (ASP.NET Core)

### Install dependencies

```bash
dotnet restore
```

### Run the backend

```bash
dotnet clean && dotnet build && dotnet run
```

The API will start on:

```
http://localhost:5266
```

---

## 🌐 Frontend Setup (React + Vite)

### Install dependencies

```bash
cd frontend
npm install
```

### Run the frontend

```bash
npm run dev
```

The app will start on:

```
http://localhost:5173
```

---

## 🧪 Running Tests

From the project root:

```bash
dotnet clean && dotnet build && dotnet test
```

---

## 🔧 Environment Variables

### Backend

Create:

```
backend/appsettings.Development.json
```

Example:

```json
{
  "JwtKey": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-QV30",
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017/WarehouseDb",
    "DatabaseName": "WarehouseDb"
  }
}
```

### Frontend

Create:

```
frontend/.env
```

Example:

```
VITE_API_URL=http://localhost:5266/api
```

---

## 🧱 Architecture Highlights

| Layer | Description |
|-------|--------------|
| **Controllers** | Handle HTTP requests and responses. |
| **Services** | Business logic and data coordination. |
| **Repositories** | MongoDB data access layer. |
| **DTOs / Models** | Define data structures and transfer objects. |
| **Tests** | Unit tests for controllers, services, and repositories. |

---

## 📜 License

This project is for personal and educational use.

---

## 🙌 Author

**Kostas** — Full‑stack developer passionate about clean architecture, testing, and polished UI/UX.

---
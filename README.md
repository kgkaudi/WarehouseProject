# 📦 Warehouse Management System

A full‑stack warehouse management application built with:

- **ASP.NET Core 10** (backend API)
- **MongoDB** (database)
- **React + Vite + Material UI** (frontend)
- **xUnit + Moq** (automated testing)
- **Docker Compose + Nginx** (containerised deployment)

This project provides user authentication, product management, and an admin dashboard for managing users and their products.

---

## 🚀 Features

### 🔐 Authentication
- Register, login, and email verification
- JWT‑based authentication
- Password hashing + salting
- Reset password flow (with show/hide password toggle)
- Session expiration handling

### 📦 Product Management
- Create, update, and delete products
- View your own products
- MongoDB‑backed repository layer
- Validation and clean Material UI interface

### 🛠 Admin Dashboard
- View all users
- Edit user details
- Promote/demote users (admin/user)
- Delete users + cascade delete their products

### 🎨 UI / UX
- Dark/light mode with persistence
- Responsive navbar + mobile drawer
- Snackbar notifications
- Dialog‑based confirmations
- Clean, modern Material UI design

### 🐳 DevOps
- One‑command full stack with Docker Compose
- Nginx serves the frontend and proxies `/api` to the backend (no CORS setup needed)
- Swagger UI for exploring the API

---

## 🧰 Tech Stack

| Layer | Technology |
|-------|-------------|
| **Backend** | ASP.NET Core 10, MongoDB Driver, JWT, Swagger |
| **Frontend** | React, Vite, Material UI |
| **Database** | MongoDB |
| **Testing** | xUnit, Moq |
| **Containers** | Docker, Docker Compose, Nginx |
| **Build Tools** | .NET CLI, npm |

---

## 📂 Project Structure

```
WarehouseDotnetProject/
│
├── backend/                # ASP.NET Core API
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
│   ├── Dockerfile
│   └── backend.csproj
│
├── backend.Tests/          # xUnit test project
│   ├── Controllers/
│   ├── Repositories/
│   ├── Services/
│   ├── Shared/
│   └── backend.Tests.csproj
│
├── frontend/               # React + Vite frontend
│   ├── src/
│   │   ├── api/
│   │   ├── components/
│   │   ├── context/
│   │   ├── pages/
│   │   └── App.jsx
│   ├── public/
│   ├── Dockerfile
│   ├── nginx.conf
│   ├── .dockerignore
│   ├── vite.config.js
│   ├── eslint.config.js
│   └── package.json
│
├── docker-compose.yml      # API + frontend (+ MongoDB)
├── .env.example            # Template for the root .env (JWT_KEY)
└── README.md
```

---

## 🏁 Getting Started

### 1️⃣ Clone the repository

```bash
git clone https://github.com/kgkaudi/WarehouseProject.git
cd WarehouseProject
```

Then choose **one** of the two ways to run it:

| Option | Best for | Frontend | API |
|--------|----------|----------|-----|
| **A. Docker** (recommended) | Running the whole app with one command | http://localhost:5173 | http://localhost:5266 |
| **B. Local development** | Editing code with hot reload | http://localhost:5173 | http://localhost:5266 |

In both cases the browser only talks to `http://localhost:5173`. The frontend calls the relative path `/api`, and a proxy forwards it to the API (Nginx in Docker, the Vite dev server locally).

---

## 🐳 Option A: Docker (full stack)

The Docker Compose setup runs the ASP.NET Core API, the React/Vite frontend (served by Nginx), and MongoDB. Nginx proxies `/api` requests to the API container, so browser requests stay same‑origin.

1. From the repository root, create a local environment file and set a unique JWT signing key:

   ```bash
   cp .env.example .env
   # Edit .env and replace JWT_KEY with a random secret (at least 32 characters).
   # You can generate one with: openssl rand -base64 48
   ```

2. Build and start the stack:

   ```bash
   docker compose up --build -d
   ```

3. Open the app at `http://localhost:5173`. The API is also exposed at `http://localhost:5266`, and Swagger is at `http://localhost:5266/swagger`. MongoDB is available to the other containers as `mongo:27017`.

Useful commands:

```bash
docker compose ps
docker compose logs -f backend
docker compose logs -f frontend
docker compose logs -f mongo
docker compose down
```

MongoDB data is stored in the `mongo_data` Docker volume and survives container recreation. To remove the database data as well, use `docker compose down -v` (this permanently deletes that volume). Do not commit your real `.env` file.

Quick health check once the containers are up (give the API a few seconds to start):

```bash
curl -i http://localhost:5266/api/users   # API directly
curl -i http://localhost:5173/api/users   # through the Nginx proxy (same response)
```

> **Note:** Vite bakes `VITE_*` variables into the bundle at **build time**. After changing the build arg `VITE_API_URL` in `docker-compose.yml`, rebuild with `docker compose build --no-cache frontend`.

---

## 🧑‍💻 Option B: Local development (hot reload)

Prerequisites: **.NET 10 SDK**, **Node.js 22+**, and a running **MongoDB** on `localhost:27017`.

### Backend

```bash
cd backend
dotnet restore
dotnet clean && dotnet build && dotnet run
```

The API starts on `http://localhost:5266`, with Swagger at `http://localhost:5266/swagger` (available in the `Development` environment).

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The app starts on `http://localhost:5173`. The Vite dev server proxies `/api` to `http://localhost:5266`, so **no CORS configuration and no `frontend/.env` file are needed**.

If your API runs somewhere else, override the proxy target:

```bash
API_PROXY_TARGET=http://localhost:5000 npm run dev
```

### Hybrid mode (API in Docker, frontend with hot reload)

```bash
docker compose up -d backend      # starts the API (and MongoDB, via depends_on)
docker compose stop frontend      # free port 5173, if the frontend container is running
cd frontend && npm run dev
```

> Don't run the frontend container and `npm run dev` at the same time. Both use port 5173.

---

## 🧪 Running Tests

From the project root:

```bash
dotnet clean && dotnet build && dotnet test
```

The test suite covers controllers, services, and repositories, including edge cases such as null/empty IDs, repository failures (HTTP 500 paths), and expired tokens.

---

## 🔧 Configuration

### Backend (local development)

Create:

```
backend/appsettings.Development.json
```

Example:

```json
{
  "JwtKey": "replace-with-a-random-secret-of-at-least-32-characters",
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017/WarehouseDb",
    "DatabaseName": "WarehouseDb"
  }
}
```

> **JWT key length:** HS256 requires a key of at least **256 bits (32 characters)**. A shorter key makes login fail with `IDX10720: ... key size must be greater than 256 bits`. Generate one with `openssl rand -base64 48`, and never commit a real key.

### Docker

Docker reads configuration from the root `.env` file (see `.env.example`) and passes it to the containers through `docker-compose.yml`:

| Variable | Purpose |
|----------|---------|
| `JWT_KEY` | JWT signing key (32+ characters). Compose refuses to start if it is missing. |

The API container receives its MongoDB settings as environment variables (`MongoDbSettings__ConnectionString` and `MongoDbSettings__DatabaseName`). Keep the database name consistent between the connection string and `DatabaseName`.

### Frontend

No `.env` file is required. `src/api/axios.js` uses the relative base URL `/api` by default:

```js
baseURL: import.meta.env.VITE_API_URL || "/api"
```

Only set `VITE_API_URL` if you deliberately want the browser to call the API directly (for example `http://localhost:5266/api`). In that case the API must allow `http://localhost:5173` via CORS. Note that a value in `frontend/.env` takes priority over the `/api` fallback in local development.

---

## 🧱 Architecture Highlights

| Layer | Description |
|-------|--------------|
| **Controllers** | Handle HTTP requests and responses. |
| **Services** | Business logic and data coordination. |
| **Repositories** | MongoDB data access layer. |
| **DTOs / Models** | Define data structures and transfer objects. |
| **Tests** | Unit tests for controllers, services, and repositories. |

Request flow:

```
Browser ──► localhost:5173 ──► Nginx (Docker) / Vite proxy (dev) ──► API :5266 ──► MongoDB
              static files          /api/* forwarded
```

---

## 🩺 Troubleshooting

| Symptom | Likely cause and fix |
|---------|----------------------|
| `502 Bad Gateway` on `/api/...` right after `docker compose up` | The API is still starting. Wait a few seconds, or check `docker compose logs backend`. |
| Network tab shows requests to `localhost:5173/api/...` | This is expected. The proxy forwards them to the API on port 5266. |
| `localhost:5266/swagger` returns 404 in Docker | Swagger is only enabled in the `Development` environment. Set `ASPNETCORE_ENVIRONMENT: Development` for the backend service, or enable it explicitly in `Program.cs`. |
| `500: Cannot deserialize a 'String' from BsonType 'ObjectId'` | The database contains users whose `_id` is an `ObjectId`, usually from another project sharing the same database. Use a dedicated database (e.g. `WarehouseDb`) or delete the stray documents. |
| Login returns 500 with `IDX10720` | `JWT_KEY` / `JwtKey` is shorter than 32 characters. |
| API can't reach MongoDB from Docker | Check that the connection string host is `mongo` (compose service) and the database name matches `DatabaseName`. |
| Frontend still calls the old URL after changing env/build args | Vite embeds values at build time. Run `docker compose build --no-cache frontend` and `docker compose up -d`. |
| Docker Desktop's `Docker.raw` keeps growing | Run `docker builder prune -f` and `docker image prune -f` periodically, then use Docker Desktop's *Troubleshoot → Clean / Purge data* if the file doesn't shrink. |

---

## 📜 License

This project is for personal and educational use.

---

## 🙌 Author

**Kostas** — Full‑stack developer passionate about clean architecture, testing, and polished UI/UX.
# SolidMReader Project

This project is a full-stack application for managing meter readings. It comprises two components:

- **SolidMReader.Api (ASP.NET Core Web API):**  The backend service that handles data storage, validation, and retrieval of meter readings.
- **solidmreader.ui (React Frontend):** The user interface that enables interaction with the API, allowing users to view the last reading and upload CSV files containing meter readings.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8 or later)
- [Node.js](https://nodejs.org/) and [npm](https://www.npmjs.com/) (or [yarn](https://yarnpkg.com/))

## Getting Started

### 1. Clone the Repository

```bash
git clone <your-repository-url>
cd SolidMReader
```

### 2. Set Up the Database
- Create a Database: Create a SQL Server database (or your preferred database) for storing meter reading data.

- Update Connection String: In the appsettings.json file of the SolidMReader.Api project, update the "DefaultConnection" string to point to your database.

### 3. Start the API and UI

**Start the API:**
```bash
dotnet run --project SolidMReader.Api
```
This will launch the API with Swagger UI at https://localhost:7036/swagger (or similar).

**Start the UI:**
```bash
cd solidmreader.ui
npm install      
npm start       
```
The React development server will start, usually on https://localhost:44471. (Make sure your API is already running).


### Frontend (React)
The React frontend uses a proxy configuration during development to simplify API requests (see proxy setting in solidmreader.ui/package.json).
The UI will be served on https://localhost:44471.

### Testing
Refer to the project's test documentation for instructions on running unit, integration, and end-to-end tests.

### Troubleshooting
**Build Errors**: Check the console output for error messages and consult the project's documentation.

**Proxy Issues**: If you get 404 errors when fetching data, verify that your API is running on https://localhost:7036 and accessible from the UI.
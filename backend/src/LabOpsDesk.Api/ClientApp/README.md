# LabOps Desk UI (Angular)

Lives inside the ASP.NET Core project as `ClientApp`.

## Preferred: one command

From `backend/`:

```bash
dotnet run --project src/LabOpsDesk.Api
```

SpaProxy starts Angular on port 4200 and serves the app through `http://localhost:5186`.

## Optional: UI-only

```bash
cd ClientApp
npm install
npm start
```

Uses `proxy.conf.json` to forward `/api` to `http://localhost:5186` (API must already be running).

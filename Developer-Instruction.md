# Developer Instruction — Full Stack Assessment

**Role:** Full Stack Developer  
**Stack:** Angular + ASP.NET Core (.NET 10)  
**Time box:** **4 hours**  
**Product:** LabOps Desk (fictional lab operations desk)

---

## 1. Purpose

Build a working vertical slice that manages **lab assets** and **spare parts**:

| Area | Persistence |
|------|-------------|
| Assets | JSON file (`data/assets.json` in the API project) |
| Parts | CSV file (`data/parts.csv` in the API project) |

You are evaluated on async C# / REST, Angular ↔ API integration, OOP design, and maintainable structure (SOLID in practice). Quality and judgment matter as much as feature count.

---

## 2. What you receive

| Item | Location |
|------|----------|
| Solution | `backend/LabOpsDesk.slnx` |
| Host project (API + UI) | `backend/src/LabOpsDesk.Api/` |
| Angular app | `backend/src/LabOpsDesk.Api/ClientApp/` |
| Unit tests | `backend/tests/LabOpsDesk.Api.Tests/` |
| Seed data | `src/LabOpsDesk.Api/data/assets.json`, `…/parts.csv` |
| Shell UI + typed API clients | `ClientApp/src/app/` |

API and Angular live in **one** ASP.NET Core project. SpaProxy starts the UI when you run the API. Contracts, endpoints, DI registration, and exception middleware are provided. Several domain and service classes currently throw `NotImplementedException`. Replace those implementations so the API, UI, and tests work end to end.

---

## 3. Domain rules

### 3.1 Assets

Statuses: `Available` | `InUse` | `Maintenance` | `Retired`

Allowed transitions:

| From | To |
|------|----|
| Available | InUse, Maintenance, Retired |
| InUse | Available, Maintenance |
| Maintenance | Available, Retired |
| Retired | *(terminal)* |

Checkout / check-in behavior:

- Checkout only from `Available` → `InUse`, with non-empty `assignee`; set `CheckedOutTo` and `CheckedOutAt`.
- Check-in only from `InUse` → `Available`; clear checkout fields.
- Illegal transitions must throw `InvalidAssetTransitionException` (mapped to HTTP **409** by middleware).
- Asset tag must be unique (duplicate create → **409** `ConflictException`).
- Missing entity → `EntityNotFoundException` (**404**).

### 3.2 Parts

- Persist all CRUD operations to the CSV file.
- SKU must be unique.
- `AdjustQuantity` must reject results below zero.
- `IsLowStock` is true when `QuantityOnHand <= ReorderThreshold`.

### 3.3 Persistence expectations

- `JsonAssetStore` reads/writes `assets.json` asynchronously; honor `CancellationToken`.
- `CsvPartStore` reads/writes `parts.csv` asynchronously; preserve the header row; honor `CancellationToken`.
- File IO should be safe for sequential requests in this assessment (a simple lock or serialized write is acceptable).
- Do **not** introduce a database.

---

## 4. Backend deliverables

Implement the incomplete pieces so that:

1. Domain methods on `Asset` and `Part` enforce the rules above.
2. `JsonAssetStore` and `CsvPartStore` fulfill their interfaces against the seed files.
3. `AssetService` and `PartService` orchestrate validation, mapping, and persistence.
4. Existing endpoints under `/api/assets` and `/api/parts` work with correct status codes.
5. Provided unit tests pass: `dotnet test` from `backend/`.
6. Prefer constructor injection and keep controllers/endpoints thin.

Suggested app URL: `http://localhost:5186` (API + Angular via SpaProxy).

---

## 5. Frontend deliverables

Complete the Angular app in `ClientApp` so an assessor can demo:

### Assets page (`/assets`)

- List assets from the API (table or equivalent).
- Create asset (form or modal).
- Edit name / platform / location / status / notes.
- Delete asset with confirmation.
- Checkout (assignee) and check-in actions when status allows.
- Loading, empty, and error states.
- Surface API failures clearly for **400**, **409**, and **500** (use `readApiError` or equivalent). Prefer typed models — avoid `any`.

### Parts page (`/parts`)

- List parts, highlighting low-stock rows.
- Create / edit / delete part.
- Adjust quantity (+/− or delta input).
- Loading, empty, and error states with the same error handling standard.

### UX bar

- Responsive enough to use on laptop width without broken layout.
- Keep navigation and feature folders coherent.
- You may restyle; do not remove the shell navigation.

With SpaProxy, the browser uses `http://localhost:5186` and `/api` is same-origin. Optional UI-only mode: `cd ClientApp && npm start` (uses `proxy.conf.json`; API must already be running).

---

## 6. Time guidance (advisory)

| Block | Focus |
|------:|-------|
| 0:00–0:20 | Read this doc, run the host project, inspect seed data |
| 0:20–1:40 | Domain + stores + services until `dotnet test` is green |
| 1:40–3:20 | Assets + Parts UI with error/loading states |
| 3:20–4:00 | Polish, manual smoke test, short `NOTES.md` |

If time is short, prioritize a correct backend + one complete CRUD screen over partial work on everything.

---

## 7. Submission

Provide:

1. Your updated source (the `LabOpsDesk.Api` project including `ClientApp`).
2. Confirmation that `dotnet test` passes.
3. A short `NOTES.md` at the assessment root covering:
   - How you ran the app
   - One design trade-off you made
   - How you used (or chose not to use) AI assistance, and how you reviewed the result
   - What you would add with more time (tests, CI stages, auth, etc.)

---

## 8. How to run

### One project (recommended)

```bash
cd backend
dotnet restore
dotnet run --project src/LabOpsDesk.Api
```

Open `http://localhost:5186`. SpaProxy launches Angular automatically (first run may `npm install` under `ClientApp`).

Health check: `GET http://localhost:5186/api/health`

### Tests

```bash
cd backend
dotnet test
```

---

## 9. Constraints

- Stay on **.NET 10** and the provided `ClientApp` Angular app.
- Keep JSON + CSV file storage (no SQL/NoSQL).
- Do not store genomic / PHI / run data.
- Do not add authentication unless you finish core CRUD early and note it as stretch work.
- Code must be something you can explain in a follow-up interview.

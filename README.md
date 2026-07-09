# Arcane Vault

A collection management prototype for hobby shops and collectors, built for IT2814 eBusiness Application Development.

## Solution structure

| Project | Purpose |
|---|---|
| `ArcaneVault.Shared` | DTOs (with validation attributes) shared between the Web and Api projects. |
| `ArcaneVault.Api` | ASP.NET Core Web API + EF Core + SQLite. Owns all data access. |
| `ArcaneVault.Web` | ASP.NET Core Razor Pages. Owns the UI and the auth cookie; never touches the database directly - every page consumes `ArcaneVault.Api` over HTTP via `ArcaneVaultApiClient`. |

## Running the solution

Both projects must be running at the same time (e.g. Visual Studio's "multiple startup projects", or two terminals):

```
dotnet run --project ArcaneVault.Api --urls http://localhost:5287
dotnet run --project ArcaneVault.Web --urls http://localhost:5068
```

The API creates and migrates `ArcaneVault.db` (SQLite) automatically on first run, and seeds:
- Two roles: `User` (RoleId 1) and `Staff` (RoleId 2)
- One Staff account: username **admin**, password **Admin@123**
- Five starter categories (Trading Cards, Coins & Currency, Action Figures, Comics & Manga, Stamps)

Browse to `http://localhost:5068`, log in as `admin` to reach Category Management and the Analytics Dashboard, or register a new account to use as a plain User.

## Design notes

**Why HTTP, not HTTPS, for local development.** Both projects run over plain HTTP rather than HTTPS. Trusting the ASP.NET Core local dev certificate (`dotnet dev-certs https --trust`) requires an interactive prompt on each machine it runs on; skipping it avoids a setup step that could otherwise make the submission look broken on a grading machine where the certificate isn't already trusted. This is a local-only prototype, so the trade-off is reasonable; a deployed version would enable HTTPS.

**How identity crosses the Web → Api boundary.** The Web project owns the authentication cookie and enforces `[Authorize]` / `[Authorize(Roles = "Staff")]` on its own pages. The Api is deliberately stateless - it has no authentication middleware of its own. Every call the Web project makes to the Api includes the current signed-in username (and the Api independently re-checks ownership, e.g. a `CollectionItem`'s `UserName`, before allowing an update/delete) as an explicit parameter. This keeps the Api as a pure data/business layer without needing CORS or a second cookie/token scheme, at the cost of trusting that Web is the only caller. A production system would instead have the Api issue and validate its own bearer token (JWT) so it can authenticate callers independently of Web.

**Soft delete vs hard delete.** `ArcaneVaultUsers` and `CollectionItems` both have an `IsDeleted` column, so deleting a user or a collection item sets that flag rather than removing the row - this preserves history and avoids breaking foreign keys from other tables. `Categories` has no such column, so deleting a category is a hard delete; the Api blocks deleting a category that is still assigned to any collection item (checked both in the UI and independently re-checked server-side) rather than silently cascading.

**StartingQuantity vs CurrentQuantity.** `StartingQuantity` is a fixed record of how many of an item a collector had when they first added it, set once at creation. `CurrentQuantity` is the live count and is the only quantity field editable afterwards.

**Propose-a-Feature: Staff Analytics Dashboard.** Aggregates platform-wide data (most popular items, category demand, most active collectors, plus summary KPI tiles) directly from the business goals described in the assignment background. Backed by four dedicated `AnalyticsController` endpoints and rendered with Chart.js. Clicking a bar in the "Most Popular Items" chart searches for that item in the signed-in Staff member's own collection, reusing the existing CollectionItems search feature.

# OloWordWrap

.NET 10 Web API (Clean Architecture). No database: nothing is stored; each request is answered from its own input.

## Run it
**F5** in VS Code (configuration "API"): builds, runs the API and opens `/health`. Without VS Code: `dotnet run --project src/OloWordWrap.Api`.
- API: http://localhost:5081
- Health: http://localhost:5081/health
- OpenAPI document (Development only): http://localhost:5081/openapi/v1.json (import it into Bruno)

## How a feature is laid out
The placeholder **SampleItem** feature goes through every layer once, so each new feature has a pattern to copy:

| Layer | Placeholder | What lives there |
|---|---|---|
| Domain | `src/OloWordWrap.Domain/SampleItems/SampleItem.cs` | the entity and its rules |
| Application | `src/OloWordWrap.Application/SampleItems/` | `GetSampleItemsHandler` (use case), `SampleItemDto` (what the API returns), `ISampleItemRepository` (what it needs from storage) |
| Infrastructure | `src/OloWordWrap.Infrastructure/InMemory/SampleItems/` | the repository: three fixed items in memory (there's no database) |
| Api | `src/OloWordWrap.Api/Controllers/SampleItemsController.cs` | `GET /api/sample-items` |
| Tests | `tests/…/SampleItems*` | one per test level (see Tests) |

Register new handlers in `AddApplication()` and new repositories in `AddInfrastructure()`. API routes start with `/api/`.

**A feature that only computes** (request in, answer out, nothing stored): put the logic in Domain as a pure function or class, a handler in Application that maps the request to it (registered in `AddApplication()`), and a controller in Api that calls the handler. It needs nothing from Infrastructure.

### Placeholder feature: replace or remove it
Either rename it into the first real feature, or delete it when the first real feature lands:
1. Delete every file and folder named `SampleItem*` / `SampleItems*` / `sample-items` (the table above lists them; `InMemorySampleItemRepository` too), and the registrations in the two `DependencyInjection.cs` files.

## Tests
| Level | Where | Run |
|---|---|---|
| Unit | `tests/OloWordWrap.UnitTests` (xUnit v3, NSubstitute) | `dotnet test` |
| Integration | `tests/OloWordWrap.IntegrationTests`: the real API in memory (`WebApplicationFactory`; no Docker needed) | `dotnet test` |

`dotnet test` also runs as the VS Code "test" task.

## CI
`.github/workflows/ci.yml` runs on every push to `main` and every pull request:
- **api:** build (warnings are errors, NuGet audit on), `dotnet format --verify-no-changes`, unit and integration tests.

## Security defaults
Why each of these is (or isn't) here: `docs/decisions.md`.
- **Rate limiting:** a global limit per client IP (`RateLimiting` in `appsettings.json`: 100 requests per 60 seconds); over it returns 429. Behind a reverse proxy, configure forwarded headers first, or every client shares the proxy's address.
- **Sign-in:** none wired. See `docs/decisions.md` for whether that's deliberate or a ticket still to come.

## Layout
```
src/
  OloWordWrap.Domain/          entities and rules; references nothing
  OloWordWrap.Application/     use cases; defines interfaces for what it needs from outside
  OloWordWrap.Infrastructure/  external services (none yet) and the placeholder's in-memory repository
  OloWordWrap.Api/             controllers, composition root (Program.cs)
tests/
  OloWordWrap.UnitTests/
  OloWordWrap.IntegrationTests/  WebApplicationFactory
.github/workflows/ci.yml    CI
```

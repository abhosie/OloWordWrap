# OloWordWrap

.NET 10 Web API (Clean Architecture). No database: nothing is stored; each request is answered from its own input.

## Run it
**F5** in VS Code (configuration "API"): builds, runs the API and opens `/health`. Without VS Code: `dotnet run --project src/OloWordWrap.Api`.
- API: http://localhost:5081
- Health: http://localhost:5081/health
- OpenAPI document (Development only): http://localhost:5081/openapi/v1.json

## Demo: word wrap
`POST /api/word-wrap?limit=N` with a JSON body `{ "text": "..." }` returns `{ "lines": [...], "wrapped": "..." }`.
1. Press **F5** with the "API" configuration (see Run it).
2. In [Bruno](https://www.usebruno.com/), choose **Open Collection** and pick the `bruno/` folder.
3. Select the **Local** environment (top right). It points at http://localhost:5081.
4. Send a request, or right-click the collection and choose **Run** to send them all. Each request has tests that check the response.

The requests, in order:
- **01–07 (200):** the brief sentence, an exact fit, a long word split with hyphens, extra spaces collapsed, line breaks kept, empty text (`lines: []`) and exactly 5,000 characters.
- **08–13 (400):** `limit` missing, not a whole number, or 0; `text` null, missing, or over 5,000 characters.
- **14 (413):** a body over 64 KB is turned away before it is read.
- **15–16 (200):** hyphens already in a word: no second hyphen is added at the break.

Rules:
- A word that exactly fits the limit stays on the line.
- Spaces and tabs collapse; no line starts or ends with a space.
- A word longer than the limit is split over lines with a hyphen at each break (each broken line is exactly the limit long; a hyphen already in the word at the break is used instead of adding one; at limit 1 there's no room for a hyphen).
- Line breaks you type are kept (a blank line stays blank); output lines are joined with `\n`.
- Text is at most 5,000 characters; `limit` is a whole number of 1 or more.

## How a feature is laid out
The **word wrap** feature (`WordWrapping`) is a feature that only computes: request in, answer out, nothing stored.

| Layer | Where | What lives there |
|---|---|---|
| Domain | `src/OloWordWrap.Domain/WordWrapping/WordWrapper.cs` | the pure wrapping function |
| Application | `src/OloWordWrap.Application/WordWrapping/` | `WrapTextHandler` (use case), `WrapTextRequest` and `WrappedTextDto` (what goes in and out) |
| Infrastructure | `src/OloWordWrap.Infrastructure/` | empty: this feature needs nothing from outside |
| Api | `src/OloWordWrap.Api/Controllers/WordWrapController.cs` | `POST /api/word-wrap` |
| Tests | `tests/…/WordWrapping/`, `tests/…/WordWrapTests.cs` | one per test level (see Tests) |

Register new handlers in `AddApplication()` and new external services in `AddInfrastructure()`. API routes start with `/api/`.

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
- **Sign-in:** none, deliberately: it runs on this machine only (see `docs/decisions.md`). Add sign-in before running it anywhere shared.

## If I had more time

- **Characters wider than one column:** lengths are counted per `char`, so emoji, some accented letters and non-Latin characters may not be measured the way they print. Worth looking into.
- **Error logging:** rejected requests aren't logged at the moment. I'd consider adding that, while being careful about logging customer text.
- **Validating the instruction text:** the text comes straight from customers, so I'd want to look at whether anything in it could be injected and cause unexpected behaviour further down the line.

## Layout
```
src/
  OloWordWrap.Domain/          entities and rules; references nothing
  OloWordWrap.Application/     use cases; defines interfaces for what it needs from outside
  OloWordWrap.Infrastructure/  external services (none yet)
  OloWordWrap.Api/             controllers, composition root (Program.cs)
tests/
  OloWordWrap.UnitTests/
  OloWordWrap.IntegrationTests/  WebApplicationFactory
bruno/                      Bruno collection for the demo
.github/workflows/ci.yml    CI
```

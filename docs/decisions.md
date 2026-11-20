# OloWordWrap: project decisions

> Written by `/new-project` on 2026-11-20, for one-shot OLO1. These are the skeleton choices: designers and planners take them as given. Changing one is its own ticket; update this file when you do.

## Facts these are based on
| Question | Answer |
|---|---|
| Browser front end | none (demo via Swagger / Bruno / `.http`) |
| Who calls the API | nobody signs in (demo / interview) |
| Where it runs | this machine only |
| Front end served from (production) | n/a |
| Stores data between requests | no: answers are computed from the request |

## Decisions

### Database
- **Choice:** none
- **Why:** word wrapping is computed from the request; nothing is stored, so a database would only add setup time (Docker, migrations) and a removal step.
- **Alternatives:** PostgreSQL with EF Core, Docker Compose and Testcontainers.
- **Revisit when:** a feature needs to keep data.
- **In the code:** the `SampleItem` placeholder is served from memory (`Infrastructure/InMemory`); tests need no Docker.

### Front end
- **Choice:** none
- **Why:** the brief asks for an endpoint to demo the solution; Swagger / Bruno / a `.http` file is enough, and a React app costs time.
- **Alternatives:** a React app in `src/Web`.
- **Revisit when:** someone needs a page to type instructions into.
- **In the code:** nothing.

### CORS
- **Choice:** off
- **Why:** no browser page on another origin calls the API.
- **Alternatives:** a `Frontend` CORS policy for a separately hosted front end.
- **Revisit when:** a front end is hosted on another origin.
- **In the code:** nothing.

### Sign-in
- **Choice:** none. The deny-by-default fallback policy (`rules/dotnet/security.md`) is deliberately not applied.
- **Why:** a demo / interview exercise run on this machine only; nobody needs to sign in.
- **Alternatives:** JWT bearer (other services), cookie sign-in (people in a browser).
- **Revisit when:** it runs anywhere other than this machine. Don't expose it to the public internet without sign-in.
- **In the code:** nothing.

### Rate limiting
- **Choice:** on (global, per client IP)
- **Why:** always on in the skeleton; costs nothing and caps abuse of the endpoint.
- **Alternatives:** off.
- **Revisit when:** it runs behind a proxy (forwarded headers) or needs per-endpoint policies.
- **In the code:** `RateLimitingOptions`, `DependencyInjection.cs`, `appsettings.json` (`RateLimiting`).

### Ports
- **Choice:** API on `http://localhost:5081` (5080 was taken by another project under `repos/`).
- **Why:** chosen automatically to avoid clashes.
- **Alternatives:** any free port.
- **Revisit when:** it clashes with something new.
- **In the code:** `src/OloWordWrap.Api/Properties/launchSettings.json`.

## Starting point
What the scaffold generated, so work builds on a known structure.
- **Placeholder feature `SampleItem`:** served from memory, `GET /api/sample-items` and a test at each level. It's the pattern to copy; the first feature renames or removes it (README: "Placeholder feature").
- **CI:** `.github/workflows/ci.yml` on push to `main` and on pull requests: `api` (build, format, unit + integration tests).

## Not included
What the skeleton deliberately leaves out, and what would bring it in.
- Database: added if a feature needs to store data.
- Sign-in: added before it runs anywhere other than this machine.
- Front end: added if a page is wanted for the demo.

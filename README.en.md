# obilet Case

[Türkçe](README.md) | **English**

Programming assignment for the obilet.com Sr. Full Stack Developer position: an ASP.NET Core MVC application that searches bus journeys through obilet's partner API.

- **Search page:** origin and destination (searchable by typing), date, Today/Tomorrow shortcuts and a swap button.
- **Journey page:** the journeys for the selected route and day, ordered by departure time.

## Requirements

- .NET 8 SDK
- An obilet API client token

## Setup and running

The API token is not kept in the repository. Locally it is provided with `dotnet user-secrets`:

```bash
dotnet user-secrets set "ObiletApi:ClientToken" "<token>" --project src/ObiletCase.Web
dotnet run --project src/ObiletCase.Web
```

The application opens at `http://localhost:5023`. If the token is missing, the application stops at start-up with a clear error message.

Tests:

```bash
dotnet test
```

The tests do not call obilet; API answers are imitated with samples trimmed from real responses.

## Project structure

```
src/ObiletCase.Web
├── Configuration/      obilet API settings (address, token, language)
├── Controllers/        HomeController (search), JourneyController (journeys)
├── Filters/            exception filter that handles obilet failures in one place
├── Models/             view models of the pages and their validation rules
├── Services/           TurkeyTime (time in Turkey); under Obilet/ the API client, the session service and the API models
├── Views/              Razor pages
└── wwwroot/            CSS and two small JavaScript files
tests/ObiletCase.Tests  xUnit tests
```

## How a request flows

1. The browser only talks to this application; every request to obilet is sent from the server.
2. The controller asks `IObiletSessionService` for the visitor's obilet session.
   - On the first request `GetSession` is sent to obilet and the returned session is kept in the ASP.NET Core Session.
   - Later requests use the stored session; every visitor has their own.
3. The controller calls `GetBusLocations` or `GetJourneys` through `IObiletApiClient` with that session.
4. The result is given to the Razor view; for the search box it is returned as JSON.

## Design decisions

- **Validation is on the server.** The rules live in `JourneySearchViewModel`: origin and destination cannot be the same, and the date cannot be before today. The journey page uses the same model, so the same rules run when the address is changed by hand. `required` and `min` in the browser are only a convenience.
- **Single-box location search.** The box is a text input attached to Bootstrap's dropdown. The typed text goes to the server after a short pause, and from there to obilet. The id and the name of the selected location are kept in hidden inputs, which the form posts.
- **The last search is stored in the browser.** It is written to `localStorage` when the journey page opens, so only searches that passed validation are stored. If the stored date has passed, the form opens with the default date (tomorrow).
- **Journeys are sorted on the server.** obilet does not send them in departure order.
- **Journeys after midnight are marked.** For a given day obilet also returns journeys that leave in the first hours of the next day; their departure time is shown in a different colour with a crescent icon.
- **The currency comes from obilet.** Turkish lira is written as "TL", other currencies with their code (international journeys can come in another currency).
- **Errors are handled in one place.** When obilet answers with an error, cannot be reached (for example 429 after too many requests) or does not answer within 20 seconds, `ObiletExceptionFilter` logs the error and shows the visitor the error page. When obilet rejects the session (400), the stored session is forgotten and the visitor's next request gets a new one.
- **Bootstrap is the only UI library.** The jQuery that came with the template was removed; the browser side is written in plain JavaScript.
- **Dates and prices are written in Turkish format.** The culture is defined in one place, in `Program.cs`.
- **"Today" is calculated in Turkey's time.** Almost all journeys leave from Turkey, so the Today/Tomorrow dates and the "date cannot be before today" rule use the day in Turkey, independent of the time zone of the server or the visitor (`TurkeyTime`).

## Known limits

- **The session is kept in memory.** That is enough for a single server; to run on several servers the in-memory cache in `Program.cs` has to be replaced with a shared cache such as Redis.
- **"Today" is the day in Turkey.** When the origin is abroad, its day differs from Turkey's for one to two hours around midnight; during that time "Today" and the earliest date do not match the origin's day. obilet sends the time zone of each location, so the rule could be calculated for the origin if needed.
- **The visitor's IP address is read from the connection.** If the application runs behind a proxy or a load balancer, forwarded headers have to be configured.
- **obilet limits requests.** When many new sessions are requested in a short time, obilet answers with 429 for a while; the application then shows the error page and does not retry on its own.

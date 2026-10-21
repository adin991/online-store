# M-HUB – Online prodavnica (Buy and Sell)

M-HUB je web aplikacija za online kupovinu i prodaju. Registrovani korisnici objavljuju oglase za proizvode, a drugi korisnici ih pretražuju, dodaju u korpu i plaćaju karticom preko **Stripe** integracije. Aplikacija ima i **AI asistenta** (Google Gemini) koji odgovara na pitanja kupaca, podršku za dva jezika (**bosanski i engleski**) i administratorski dio za upravljanje korisnicima.

## Namjena aplikacije

- **Kupci** pregledaju oglase po kategorijama (Electronics, Cars & Vehicles, Fashion, Home & Garden, Gaming, Books, Sports, Beauty), pretražuju proizvode, ostavljaju komentare, dodaju proizvode u korpu ili ih spremaju za kasnije i plaćaju online.
- **Prodavci** objavljuju oglase sa slikom, cijenom, kategorijom, lokacijom i opcijama (boje, veličine), te prate svoje oglase kroz dashboard (aktivni, zatvoreni/prodani).
- **Administratori** upravljaju korisnicima i njihovim ulogama.

---

## Tehnologije

| Sloj | Tehnologije |
|------|-------------|
| Frontend | Angular 19, TypeScript, RxJS, ngx-translate (i18n: `bs` / `en`), Stripe.js |
| Backend | ASP.NET Core 8 Web API, MediatR (CQRS), FluentValidation, Entity Framework Core 8 |
| Baza | Microsoft SQL Server |
| Autentifikacija | JWT (access + refresh token), BCrypt hashiranje lozinki |
| Plaćanje | Stripe (Stripe.net + Stripe.js) |
| AI asistent | Google Gemini API |
| Logovanje | Serilog (konzola + fajl), middleware za logovanje requestova |
| Testovi | xUnit, `WebApplicationFactory`, EF Core InMemory |
| Dokumentacija API-ja | Swagger (Swashbuckle) |

---

## Arhitektura

Backend je organizovan po principima **Clean Architecture**: zavisnosti idu prema unutra, a domenski sloj ne zavisi ni od jednog drugog sloja. Poslovna logika je u Application sloju i podijeljena je po **CQRS** obrascu (Commands / Queries) uz **MediatR**.

```
┌──────────────────────────────────────────────────────────────┐
│                 Angular 19 SPA  (WebShop.Frontend)           │
│  modules (public / client / auth) · api-services · guards    │
│  interceptors (JWT) · chat-widget · stripe-payment-modal     │
└───────────────────────────┬──────────────────────────────────┘
                            │ HTTP / JSON  (JWT Bearer)
┌───────────────────────────▼──────────────────────────────────┐
│  WebShop.API          Controllers, Middleware, Swagger, DI   │
├──────────────────────────────────────────────────────────────┤
│  WebShop.Application  Commands / Queries (MediatR),          │
│                       Validators (FluentValidation),         │
│                       Pipeline Behaviors, Abstractions       │
├──────────────────────────────────────────────────────────────┤
│  WebShop.Infrastructure  EF Core DbContext, Migrations,      │
│                          Seeders, Gemini & Stripe servisi    │
├──────────────────────────────────────────────────────────────┤
│  WebShop.Domain       Entiteti (Catalog, Identity, WebShop)  │
│  WebShop.Shared       DTO-ovi, konstante, Options klase      │
└───────────────────────────┬──────────────────────────────────┘
                            │
        ┌───────────────────┼────────────────────┐
        ▼                   ▼                    ▼
   SQL Server          Stripe API           Gemini API
```

### Backend projekti (`WebShop.Backend/`)

| Projekat | Odgovornost |
|----------|-------------|
| `WebShop.API` | Ulazna tačka. REST kontroleri (Ads, Comment, Payments, User, Chat, Files, Notifications, Messages…), middleware za logovanje, konfiguracija JWT-a i Swaggera. |
| `WebShop.Application` | Poslovna logika kao MediatR Commands/Queries (npr. `Ad/Commands/Create`, `Ad/Queries/List`), FluentValidation validatori, pipeline behaviori, interfejsi za vanjske servise (`IGeminiChatService`…). |
| `WebShop.Domain` | Domenski entiteti grupisani u `Catalog`, `Identity` i `WebShopEntities`. |
| `WebShop.Infrastructure` | `DatabaseContext` (EF Core), konfiguracije entiteta, migracije, seederi i implementacije vanjskih servisa (Gemini, Stripe). |
| `WebShop.Shared` | DTO-ovi, konstante i Options klase koje dijele ostali slojevi. |
| `WebShop.Tests` | Integracijski i unit testovi (Ads, Files, Payment, ProductCategory, User). |

### Tok jednog zahtjeva

1. Angular komponenta poziva odgovarajući servis iz `api-services/`.
2. HTTP interceptor dodaje JWT token u header.
3. Kontroler u `WebShop.API` prima zahtjev i šalje Command ili Query kroz `IMediator`.
4. MediatR pipeline pokreće validaciju (FluentValidation), a zatim handler.
5. Handler preko `DatabaseContext`-a (EF Core) čita ili piše podatke u SQL Server, ili poziva vanjski servis (Stripe, Gemini).
6. Rezultat se vraća kao DTO, a frontend ga prikazuje.

### Frontend (`WebShop.Frontend/src/app/`)

| Folder | Sadržaj |
|--------|---------|
| `modules/public` | Javni dio: početna stranica, detalji proizvoda, layout |
| `modules/client` | Dio za prijavljene korisnike: dashboard, dodavanje i uređivanje oglasa, profil, lista korisnika (admin) |
| `modules/auth` | Prijava i registracija |
| `api-services` | Servisi za komunikaciju s backendom (ads, auth, chat, comments, payments…) |
| `core` | Zajedničke komponente (chat widget, Stripe modal, paginator, toast, image crop), guardovi, interceptori, validatori, servisi (auth, cart) |
| `public/i18n` | Prevodi `bs.json` i `en.json` |

### Baza podataka

![Dijagram baze](documents/DijagramBaze-najnovija-verzija.png)

---

## Screenshotovi

### Početna stranica
Na vrhu su pretraga, izbor jezika, dugme za dodavanje proizvoda, korpa i korisnički profil. Ispod su kategorije i istaknuti proizvodi s cijenom i lokacijom, a zatim paginacija, prijava na newsletter i footer.

![Početna stranica](documents/img/localhost_4200_.png)

### AI asistent (WebShop assistant)
Plutajući chat widget u donjem desnom uglu povezan je s Google Gemini API-jem i odgovara na pitanja korisnika o prodavnici.

![AI asistent](documents/img/1.png)

### Detalji oglasa (vlasnik)
Kad korisnik otvori svoj oglas, vidi sliku, prodavca, kategoriju, cijenu, datum objave i opis, te dugmad **Edit ad** i **Delete ad**. Ispod je sekcija za komentare.

![Detalji vlastitog oglasa](documents/img/localhost_4200_ad_23.png)

### Detalji oglasa (kupac) i korpa
Kupac može dodati proizvod u korpu. Korpa se otvara kao bočni panel u kojem se vide stavke i ukupna cijena. Stavku je moguće obrisati ili spremiti za kasnije (**Save for later**), a dugme **Checkout** pokreće plaćanje.

![Korpa](documents/img/localhost_4200_ad_24.png)

![Korpa - detalj](documents/img/25.png)

### Plaćanje (Stripe)
Plaćanje se obavlja kroz Stripe modal s više načina plaćanja (kartica, Bancontact, MB WAY, EPS, Satispay). Cijena se prikazuje u KM, uz preračun u EUR. U test modu se koristi kartica `4242 4242 4242 4242`.

![Stripe plaćanje](documents/img/24.png)

### Dodavanje novog proizvoda
Forma za objavu oglasa: slika (PNG/JPG do 10 MB, uz mogućnost isijecanja), naziv, cijena u KM, kategorija, lokacija, opcionalne boje i veličine (kupac bira jednu prije kupovine) i opis. Sva polja se validiraju na frontendu i na backendu.

![Dodavanje proizvoda](documents/img/localhost_4200_client_product-add.png)

### Korisnički dashboard
Pregled vlastitih oglasa: broj oglasa, broj aktivnih (Live) i zatvorenih/prodanih, stanje korpe, te lista oglasa s pretragom, filterom po statusu i sortiranjem.

![Dashboard](documents/img/localhost_4200_client_dashboard.png)

### Upravljanje korisnicima (admin)
Administrator vidi listu svih korisnika s pretragom po imenu, prezimenu ili emailu, filterom po ulozi, sortiranjem i paginacijom, te može brisati korisnike.

![Lista korisnika](documents/img/localhost_4200_client_users.png)

---

## Pokretanje projekta

### Preduslovi
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS) i Angular CLI (`npm i -g @angular/cli`)
- SQL Server (lokalni ili Express)

### Backend

```bash
cd WebShop.Backend/WebShop.API

# Tajni ključevi se ne čuvaju u repozitoriju – postavi ih preko User Secrets:
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "Gemini:ApiKey" "..."

dotnet run
```

Connection string je u `appsettings.json` (`ConnectionStrings:Main`, po defaultu `Server=localhost;Database=WebShopDB`). Pri pokretanju se baza kreira i popunjava početnim podacima. API je dostupan na `http://localhost:5177`, a Swagger na `http://localhost:5177/swagger`.

### Frontend

```bash
cd WebShop.Frontend
npm install
ng serve
```

Aplikacija je dostupna na `http://localhost:4200`.

### Testovi

```bash
cd WebShop.Backend
dotnet test
```

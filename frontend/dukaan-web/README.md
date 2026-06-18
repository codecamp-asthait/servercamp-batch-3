# Dukaan - Web Frontend

Multi-vendor e-commerce storefront and merchant dashboard built with Next.js 16.

## Tech Stack

- **Framework:** Next.js 16 (App Router), React 19
- **Language:** TypeScript
- **Styling:** Tailwind CSS 4, tw-animate-css
- **UI Components:** shadcn/ui, Base UI, Lucide icons
- **Data Fetching:** TanStack Query (React Query)
- **Rich Text:** TipTap editor
- **Testing:** Jest + React Testing Library
- **Linting:** ESLint

## Getting Started

```bash
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000).

## Scripts

| Command        | Description              |
| -------------- | ------------------------ |
| `npm run dev`  | Start dev server         |
| `npm run build`| Production build         |
| `npm start`    | Start production server  |
| `npm run lint` | Run ESLint               |
| `npm test`     | Run Jest tests           |

## Project Structure

```
frontend/dukaan-web/
├── public/                   # Static assets
├── src/
│   ├── app/                  # Next.js App Router pages
│   │   ├── (merchant)/       # Merchant portal routes
│   │   │   └── merchant/
│   │   │       ├── (auth)/login
│   │   │       └── (protected)/
│   │   │           ├── dashboard
│   │   │           ├── products
│   │   │           └── categories
│   │   └── (store)/          # Storefront routes
│   │       └── store/[slug]/
│   │           ├── (main)    # Products listing & detail
│   │           ├── login
│   │           └── register
│   ├── components/           # Shared UI components
│   │   └── ui/               # shadcn/ui primitives
│   ├── hooks/                # Shared hooks
│   ├── lib/                  # Utilities, HTTP client, storage
│   └── modules/              # Feature modules
│       ├── merchant/
│       │   ├── auth
│       │   ├── dashboard
│       │   ├── products
│       │   └── categories
│       └── store/
│           ├── auth
│           ├── products
│           └── cart
```

Each module follows a consistent structure: `api.ts`, `hooks.ts`, `types.ts`, and `components/`.

## Modules

### Merchant Portal (`/merchant`)

- **Auth** — Login for merchants
- **Dashboard** — Overview with sidebar navigation
- **Products** — CRUD table with product form (rich text editor)
- **Categories** — Category management table

### Storefront (`/store/[slug]`)

- **Auth** — Customer login & registration
- **Products** — Product grid, cards, detail view, category filter
- **Cart** — Drawer with item rows, quantity controls, summary

## Environment Variables

| Variable                      | Description              |
| ----------------------------- | ------------------------ |
| `NEXT_PUBLIC_API_URL`         | Backend API base URL     |
| `NEXT_PUBLIC_MEDIA_URL`       | Media service base URL   |

Read this file before every task.

# Project: StoronnimV.Client

## Git
- **GitHub Owner:** ilyaghrischenko.
- **GitHub Repo:** StoronnimV.
- **Main branch name:** main.

## Architecture & Patterns
- React single-page application with public pages and administrator screens routed through React Router.
- Feature-oriented source tree: page components live in `src/components/pages`, reusable and feature UI in `src/components/elements`, feature state/API providers in `src/components/contexts`, API models in `src/models`, and styles in `src/styles`.
- React Context API provides global loading/modal/auth/request state and separate feature contexts for home, news, schedule, group, music, and video data.
- Functional components and hooks are used throughout; shared UI includes generic list, modal, loading, pagination, frame, header, and footer components.
- Vite validates and injects the environment-specific API base URL; Azure Static Web Apps route rewrites support client-side routing.

## Stack
- **Runtime:** React 18, React DOM 18, TypeScript 5.6.
- **Build tooling:** Vite 6, `@vitejs/plugin-react`, `vite-plugin-svgr`.
- **Routing and document metadata:** React Router DOM 7, React Helmet Async.
- **Data access:** Axios with credentialed requests.
- **UI and styling:** React Bootstrap, Bootstrap 5, MUI 7, Emotion, SCSS/CSS, React Icons.
- **Animation and media:** Framer Motion, Swiper, React Player.
- **Deployment and SEO tooling:** Azure Static Web Apps routing config, sitemap-generator-cli.
- **Package management:** npm with `package-lock.json` lockfile v3.

## Static Code Analyzer
- ESLint 9 flat config at `storonnimv.client/eslint.config.js` with `@eslint/js`, `typescript-eslint`, `eslint-plugin-react-hooks`, and `eslint-plugin-react-refresh` recommended rules.
- TypeScript static checks at `storonnimv.client/tsconfig.app.json` and `storonnimv.client/tsconfig.node.json` enable strict typing, unused symbol checks, switch fallthrough checks, and unchecked side-effect import checks.

## Critical Coding Rules (MUST FOLLOW)
- Keep TypeScript strict: do not introduce implicit `any`, unused locals or parameters, switch fallthrough, or unchecked side-effect imports.
- Follow the configured ESLint recommended rules, React Hooks rules, and React Refresh export rule; do not suppress diagnostics without a project-specific reason.
- Use functional React components and hooks; type component props and context values with TypeScript interfaces or type-safe generics.
- Keep routing in `src/components/pages/shared/Page.tsx`; place page composition in `pages`, reusable/feature UI in `elements`, API state and fetch operations in `contexts`, and response shapes in `models`.
- Create feature contexts with an explicit typed value and provider; obtain shared request, auth, modal, and loading behavior from `GlobalContext` instead of duplicating it.
- Build API endpoints from `serverRoute`, which comes from the validated `import.meta.env.VITE_API_URL`; do not hardcode API origins in components or contexts.
- Send HTTP requests through `GlobalContext.sendRequest` so credential handling and rate-limit behavior stay centralized.
- For async UI operations, set the matching page or modal loading state before the request and clear it in `finally`.
- Use controlled form state for admin forms and `FormData` when the request includes files.
- Keep API model property names camelCase to match the JSON response shapes used by existing contexts and components.
- Add source styles under `src/styles`; `index.html` loads the compiled `src/styles/style.css` entrypoint.

## Workspace Commands

Use `frontend/storonnimv.client` as the working directory.

- Install: `npm install`
- Run development server: `npm run dev`
- Build: `npm run build`
- Lint: `npm run lint`
- Preview production build: `npm run preview`
- Test: (none detected)

## Project Learnings

**Accumulated corrections. This section is for the agent to maintain, not just the human.**

When the user corrects your approach, append a one-line rule here before ending the session. Write it concretely ("Always use X for Y"), never abstractly ("be careful with Y"). If an existing line already covers the correction, tighten it instead of adding a new one. Remove lines when the underlying issue goes away (model upgrades, refactors, process changes).

- (empty)

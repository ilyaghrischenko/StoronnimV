---
title: Authentication and Media Contracts
type: contract
status: current
tags:
  - contract
  - authentication
  - csrf
  - media
created: 2026-07-20
updated: 2026-07-20
verified_at: 2026-07-20T12:00:00Z
verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
code_refs:
  - path: backend/StoronnimV.Server/StoronnimV.Api/Program.cs
    symbol: authentication middleware order
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Extensions/WebApplicationBuilderExtensions.cs
    symbol: JWT, CORS, antiforgery, and upload configuration
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Api/Middlewares/AntiforgeryMiddleware.cs
    symbol: unsafe cookie request validation
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Application/Services/Utils/MediaFileValidator.cs
    symbol: media policy
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: backend/StoronnimV.Server/StoronnimV.Application/Services/Utils/MediaStorageService.cs
    symbol: media compensation orchestration
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
  - path: frontend/storonnimv.client/src/components/contexts/shared/GlobalContext.tsx
    symbol: credentialed client and CSRF header
    kind: file
    verified_commit: 09f3b80e147706f436c4bc59065f1546064781ac
source_refs: []
summary: Cookie JWT, CORS and antiforgery topology together with upload validation and DB/Blob compensation rules.
---
# Authentication and Media Contracts

Authentication uses a JWT bearer handler that accepts either an Authorization header token or the HttpOnly Token cookie. The HTTP pipeline invokes authentication before antiforgery validation and authorization. The frontend never treats its local role string as the authority for protected access; it asks the API for administrator state and the current role.

CLIENT_URL is validated as one exact HTTP(S) origin and CORS permits credentials for that origin. For unsafe cookie-authenticated requests, the client first obtains a fresh antiforgery token and sends X-CSRF-TOKEN. Bearer-only mutations are not subjected to the cookie-CSRF rule. Cookie Secure and SameSite settings are environment-dependent and must match the final deployment topology.

Photo uploads are limited to supported JPEG, PNG, or WebP content and video uploads to MP4, within configured hard caps. Validation checks size, extension, MIME type, and file signature before upload. Create and replace flows remove a newly uploaded blob when the database update fails. Old blobs are removed only after the database points to the replacement; cleanup continues independently of request cancellation after commit. A failed post-commit deletion yields an identifiable safe orphan for operational cleanup.

See [[wiki/contracts/http-api|HTTP API]], [[wiki/database/postgresql-and-storage|PostgreSQL and Storage]], and [[wiki/runbooks/local-development|Local Development]]. Return to [[wiki/contracts/_index|Contracts]].

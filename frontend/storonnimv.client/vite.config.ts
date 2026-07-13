import {defineConfig, loadEnv} from 'vite'
import react from '@vitejs/plugin-react'
import svgr from "vite-plugin-svgr";

// https://vite.dev/config/
export default defineConfig(({mode}) => {
  const rawApiUrl = loadEnv(mode, ".").VITE_API_URL?.trim()

  if (!rawApiUrl) {
    throw new Error("VITE_API_URL is required")
  }

  let apiUrl: URL

  try {
    apiUrl = new URL(rawApiUrl)
  } catch {
    throw new Error("VITE_API_URL must be an absolute HTTP(S) URL")
  }

  if (
    !["http:", "https:"].includes(apiUrl.protocol) ||
    apiUrl.username ||
    apiUrl.password ||
    apiUrl.search ||
    apiUrl.hash
  ) {
    throw new Error("VITE_API_URL must be an absolute HTTP(S) URL without credentials, query, or fragment")
  }

  const normalizedApiUrl = apiUrl.href.replace(/\/+$/, "")

  return {
    plugins: [react(), svgr()],
    define: {
      "import.meta.env.VITE_API_URL": JSON.stringify(normalizedApiUrl)
    }
  }
})

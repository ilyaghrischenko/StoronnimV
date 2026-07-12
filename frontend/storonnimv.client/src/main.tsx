import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import App from './App.tsx'
import {GlobalContextProvider} from "./components/contexts/shared/GlobalContext.tsx";
import {HelmetProvider} from "react-helmet-async";

createRoot(document.getElementById('root')!).render(
    <GlobalContextProvider>
        <HelmetProvider>
            <StrictMode>
                <App/>
            </StrictMode>
        </HelmetProvider>
    </GlobalContextProvider>
)

import {JSX, useContext} from 'react';
import {BrowserRouter as Router} from "react-router-dom";
import {Page} from "./components/pages/shared/Page";
import {ModalWindow} from "./components/elements/shared/ModalWindow";
import {GlobalContext} from "./components/contexts/shared/GlobalContext.tsx";
import {FrameLayout} from "./components/elements/shared/FrameLayout.tsx";
import {Header} from "./components/elements/shared/Header.tsx";
import {Footer} from "./components/elements/shared/Footer.tsx";

function App(): JSX.Element {
    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {isAdminRoute} = globalContext;

    return (
        // <ResolutionWrapper>
            <div className="app-container">
                <Router>
                    <FrameLayout
                        header={!isAdminRoute() ? <Header/> : null}
                        footer={!isAdminRoute() ? <Footer/> : null}
                    >
                        <ModalWindow/>
                        <div className="inFrame__content">
                            <Page/>
                        </div>
                    </FrameLayout>
                </Router>
            </div>
        // </ResolutionWrapper>
    );

}

export default App;

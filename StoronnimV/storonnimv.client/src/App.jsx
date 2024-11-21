import {Container} from 'react-bootstrap';
import {BrowserRouter as Router} from "react-router-dom";

// Components
import {GlobalContextProvider} from "./components/contexts/GlobalContext";
import {Page} from "./components/pages/Page";
import {Footer} from "./components/elements/Footer";
import {Header} from "./components/elements/Header";

function App() {
    return (
        <GlobalContextProvider>
            <Container>
                <Router>
                    <Header />
                    <Page />
                    <Footer />
                </Router>
            </Container>
        </GlobalContextProvider>
    );
}

export default App;

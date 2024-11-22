import {Container} from 'react-bootstrap';
import {BrowserRouter as Router} from "react-router-dom";

import './styles/App.css';

// Components
import {GlobalContextProvider} from "./components/contexts/GlobalContext";
import {Page} from "./components/pages/Page";
import {Footer} from "./components/elements/Footer";
import {Header} from "./components/elements/Header";

function App() {
    return (
        <GlobalContextProvider>
            <Container className='app-container' fluid>
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

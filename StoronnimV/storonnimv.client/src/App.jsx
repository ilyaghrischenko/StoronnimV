import {Container} from 'react-bootstrap';
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";

// #region Components
import {Header} from "./components/Header";
import {News} from "./components/News";
import {Group} from "./components/Group";
import {Contacts} from "./components/Contacts";
// #endrgion

function App() {
    return (
        <Router>
            <Header />
            <Container className="mt-4">
                <Routes>
                    <Route path="/news" element={<News />} />
                    <Route path="/group" element={<Group />} />
                    <Route path="/contacts" element={<Contacts />} />
                </Routes>
            </Container>
        </Router>
    );
}

export default App;

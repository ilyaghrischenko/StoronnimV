import {Container} from 'react-bootstrap';
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";

// #region Components
import {News} from "./components/News";
import {Group} from "./components/Group";
import {Contacts} from "./components/Contacts";
import {ContextProvider} from "./components/Context";
// #endrgion

function App() {
    return (
        <ContextProvider>
            <Container>
                <Router>
                    <Routes>
                        <Route path="/news" element={<News/>}/>
                        <Route path="/group" element={<Group/>}/>
                        <Route path="/contacts" element={<Contacts/>}/>
                    </Routes>
                </Router>
            </Container>
        </ContextProvider>
    );
}

export default App;

import {Container} from 'react-bootstrap';
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";

// Components
import {GlobalContextProvider} from "./components/contexts/GlobalContext";
import {News} from "./components/pages/News";
import {Group} from "./components/pages/Group";
import {Contacts} from "./components/pages/Contacts";
import {Schedule} from "./components/pages/Schedule";
import {Music} from "./components/pages/Music";

function App() {
    return (
        <GlobalContextProvider>
            <Container>
                <Router>
                    <Routes>
                        <Route path="/" element={<Schedule />} />
                        <Route path="/schedule" element={<Schedule />}/>
                        <Route path="/news" element={<News />}/>
                        <Route path="/music" element={<Music />}/>
                        <Route path="/group" element={<Group />}/>
                        <Route path="/contacts" element={<Contacts />}/>
                    </Routes>
                </Router>
            </Container>
        </GlobalContextProvider>
    );
}

export default App;

import {Container} from "react-bootstrap";

import {Header} from "../elements/Header";
import {MusicContextProvider} from "../contexts/MusicContext";
import {Footer} from "../elements/Footer";

const Music = () => {
    return (
        <MusicContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <h1>MUSIC</h1>
                <Footer />
            </Container>
        </MusicContextProvider>
    );
};

export { Music };
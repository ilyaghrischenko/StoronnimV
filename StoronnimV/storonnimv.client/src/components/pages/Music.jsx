import {useContext} from "react";
import {Container} from "react-bootstrap";

import {GlobalContext} from "../contexts/GlobalContext";
import {MusicContextProvider} from "../contexts/MusicContext";

const Music = () => {
    const {setBgImage} = useContext(GlobalContext);
    setBgImage('photo.jpg');
    
    return (
        <MusicContextProvider>
            <Container>
                <h1>MUSIC</h1>
            </Container>
        </MusicContextProvider>
    );
};

export { Music };
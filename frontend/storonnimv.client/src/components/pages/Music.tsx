import {FC, useContext, useEffect} from "react";
import {MusicContextProvider} from "../contexts/MusicContext";
import {Button, Container} from "react-bootstrap";
import {SpotifyContainer} from "../elements/music/SpotifyContainer.tsx";
import {MusicPlatforms} from "../elements/music/MusicPlatforms";
import {GlobalContext} from "../contexts/shared/GlobalContext.tsx";
import {AddMusicPlatformModal} from "../elements/music/forms/AddMusicPlatformModal.tsx";
import {FaPlus} from "react-icons/fa";
import {Helmet} from "react-helmet-async";

const Music: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'music');

    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {fetchIsAdmin, isAdmin, OnShowModal} = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);

    return (
        <MusicContextProvider>
            <Helmet>
                <title>Музика - Стороннім В</title>
                <meta name="description" content="Слухайте музику гурту Стороннім В онлайн." />
            </Helmet>

            <Container className="page">
                <div className='music-page'>
                    {isAdmin &&
                        <Button
                            className='admin-button__add'
                            onClick={() => OnShowModal(<AddMusicPlatformModal/>)}
                        >
                            <FaPlus/>
                        </Button>}
                    <MusicPlatforms/>
                    <SpotifyContainer/>
                </div>
            </Container>
        </MusicContextProvider>
    );
};

export {Music};

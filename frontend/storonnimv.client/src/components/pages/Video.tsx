import {FC, ReactNode, useContext, useEffect} from "react";
import {Container} from "react-bootstrap";
import {VideoContextProvider} from "../contexts/VideoContext";
import {GlobalContext} from "../contexts/shared/GlobalContext.tsx";
import {Helmet} from "react-helmet-async";

interface VideoProps {
    children: ReactNode;
}

const Video: FC<VideoProps> = ({children}) => {
    sessionStorage.setItem('pressedButtonName', 'video/sections');

    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {fetchIsAdmin} = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);


    return (
        <VideoContextProvider>
            <Helmet>
                <title>Відео - Стороннім В</title>
                <meta name="description" content="Переглядайте відео гурту Стороннім В." />
            </Helmet>

            <Container className='page'>
                {children}
            </Container>
        </VideoContextProvider>
    );
};

export {Video};

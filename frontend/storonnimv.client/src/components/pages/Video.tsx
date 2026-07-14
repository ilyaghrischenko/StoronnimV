import {FC, ReactNode} from "react";
import {Container} from "react-bootstrap";
import {VideoContextProvider} from "../contexts/VideoContext";
import {Helmet} from "react-helmet-async";

interface VideoProps {
    children: ReactNode;
}

const Video: FC<VideoProps> = ({children}) => {
    sessionStorage.setItem('pressedButtonName', 'video/sections');

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

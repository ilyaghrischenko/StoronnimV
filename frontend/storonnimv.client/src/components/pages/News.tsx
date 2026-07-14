import {FC} from "react";
import {NewsContextProvider} from "../contexts/NewsContext";
import {Container} from "react-bootstrap";
import {NewsList} from "../elements/news/NewsList";
import {Helmet} from "react-helmet-async";


const News: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'news');

    return (
        <NewsContextProvider>
            <Helmet>
                <title>Новини - Стороннім В</title>
                <meta name="description" content="Стежте за активністю гурту Стороннім В." />
            </Helmet>

            <Container className="page">
                <NewsList/>
            </Container>
        </NewsContextProvider>
    );
};

export {News};

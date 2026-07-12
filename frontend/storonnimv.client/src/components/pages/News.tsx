import {FC, useContext, useEffect} from "react";
import {NewsContextProvider} from "../contexts/NewsContext";
import {Container} from "react-bootstrap";
import {NewsList} from "../elements/news/NewsList";
import {GlobalContext} from "../contexts/shared/GlobalContext.tsx";
import {Helmet} from "react-helmet-async";


const News: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'news');

    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {fetchIsAdmin} = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);

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

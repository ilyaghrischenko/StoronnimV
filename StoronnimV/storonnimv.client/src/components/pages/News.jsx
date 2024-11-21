import {Container} from "react-bootstrap";

import {Header} from "../elements/Header";
import {NewsList} from "../elements/NewsList";
import {NewsContextProvider} from "../contexts/NewsContext";
import {Footer} from "../elements/Footer";

const News = () => {
    return (
        <NewsContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <NewsList />
                <Footer />
            </Container>
        </NewsContextProvider>
    );
};

export {News};
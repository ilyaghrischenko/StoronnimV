import {Container} from "react-bootstrap";

import {Header} from "../elements/Header";
import {NewsList} from "../elements/NewsList";
import {NewsContextProvider} from "../contexts/NewsContext";

const News = () => {
    return (
        <NewsContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <NewsList/>
            </Container>
        </NewsContextProvider>
    );
};

export {News};
import {Container} from "react-bootstrap";

import {Header} from "./Header";
import {NewsList} from "./NewsList";
import {NewsContextProvider} from "./contexts/NewsContext";

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
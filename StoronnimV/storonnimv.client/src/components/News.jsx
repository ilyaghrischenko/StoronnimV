import {Container} from "react-bootstrap";

import {Header} from "./Header";
import {NewsList} from "./NewsList";

const News = () => {
    return (
        <Container>
            <Header bgImage={'photo.jpg'} />
            <NewsList />
        </Container>
    );  
};

export { News };
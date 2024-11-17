import {Container} from "react-bootstrap";

import {PhotoContainer} from "./PhotoContainer";

const News = () => {
    return (
        <Container>
            <PhotoContainer
                photoUrl={'./photo.jpg'}
                text={'hello world!'}/>
            <h1>NEWS</h1>
        </Container>
    );  
};

export { News };
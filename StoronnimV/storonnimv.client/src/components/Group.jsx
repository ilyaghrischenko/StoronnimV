import {Container} from "react-bootstrap";

import {PhotoContainer} from "./PhotoContainer";

const Group = () => {
    return (
        <Container>
            <PhotoContainer
                photoUrl={'./photo.jpg'}
                text={'hello world!'}/>
            
            <h1>GROUP</h1>
        </Container>
    );
};

export { Group };
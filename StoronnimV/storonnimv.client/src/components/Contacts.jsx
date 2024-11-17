import {Container} from "react-bootstrap";

import {PhotoContainer} from "./PhotoContainer";

const Contacts = () => {
    return (
        <Container>
            <PhotoContainer
                photoUrl={'./photo.jpg'}
                text={'hello world!'}/>
            
            <h1>CONTACTS</h1>
        </Container>
    );
};

export { Contacts };
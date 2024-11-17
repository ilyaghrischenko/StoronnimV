import '../styles/PhotoContainer.css';

import {Container} from "react-bootstrap";

const PhotoContainer = ({photoUrl, text}) => {
    return (
        <Container
            className='photo-container'
            style={{backgroundImage: `url(${photoUrl})`}}>
            
            <h1>{text}</h1>
        </Container>
    );
};

export { PhotoContainer };
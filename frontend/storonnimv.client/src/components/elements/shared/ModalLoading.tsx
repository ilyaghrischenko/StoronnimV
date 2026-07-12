import {Container} from 'react-bootstrap';
import {FC} from "react";


const ModalLoading: FC = () => {
    return (
        <Container className='loading-container'>
            <Container className="loading-container__spinner"/>
        </Container>
    );
};

export { ModalLoading };

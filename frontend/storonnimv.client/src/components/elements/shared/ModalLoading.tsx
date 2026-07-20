import {Container} from 'react-bootstrap';
import {FC} from "react";


const ModalLoading: FC = () => {
    return (
        <Container className='loading-container' role="status" aria-label="Завантаження">
            <Container className="loading-container__spinner" aria-hidden="true"/>
        </Container>
    );
};

export { ModalLoading };

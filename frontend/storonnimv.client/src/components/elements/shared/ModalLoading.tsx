import {Container} from 'react-bootstrap';
import {FC} from "react";


const ModalLoading: FC = () => {
    return (
        <Container className='loading-container' role="status" aria-live="polite">
            <span className="visually-hidden-heading">Завантаження…</span>
            <Container className="loading-container__spinner" aria-hidden="true"/>
        </Container>
    );
};

export { ModalLoading };

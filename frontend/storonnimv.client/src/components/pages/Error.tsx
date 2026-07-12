import {FC} from "react";
import {Container} from "react-bootstrap";
import {useSearchParams} from "react-router-dom";
import {Helmet} from "react-helmet-async";

const Error: FC = () => {
    const [searchParams] = useSearchParams();
    const statusCode = searchParams.get("statusCode") || "500";
    const message = searchParams.get("message") || "Unexpected error";

    return (
        <Container className='page'>
            <Helmet>
                <title>Помилка - Стороннім В</title>
                <meta name="description" content="Щось пішло не так..." />
            </Helmet>

            <Container className='error-info-container'>
                <p className='error-info-container__status-code'>{statusCode}</p>
                <p className='error-info-container__message'>{message}</p>
            </Container>
        </Container>
    );
};

export {Error};
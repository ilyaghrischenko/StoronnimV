import {FC} from "react";
import {ScheduleContextProvider} from "../contexts/ScheduleContext";
import {Container} from "react-bootstrap";
import {SchedulesList} from "../elements/schedule/SchedulesList";
import {Helmet} from "react-helmet-async";

const Schedule: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'schedule');

    return (
        <ScheduleContextProvider>
            <Helmet>
                <title>Афіша - Стороннім В</title>
                <meta name="description" content="Будьте в курсі усіх виступів гурту Стороннім В." />
            </Helmet>

            <Container className="page schedule-page">
                <h1 className="visually-hidden-heading">Афіша</h1>
                <SchedulesList/>
            </Container>
        </ScheduleContextProvider>
    );
};

export {Schedule};

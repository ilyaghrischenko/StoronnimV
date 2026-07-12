import {FC, useContext, useEffect} from "react";
import {ScheduleContextProvider} from "../contexts/ScheduleContext";
import {Container} from "react-bootstrap";
import {SchedulesList} from "../elements/schedule/SchedulesList";
import {GlobalContext} from "../contexts/shared/GlobalContext.tsx";
import {Helmet} from "react-helmet-async";

const Schedule: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'schedule');

    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {fetchIsAdmin} = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);

    return (
        <ScheduleContextProvider>
            <Helmet>
                <title>Афіша - Стороннім В</title>
                <meta name="description" content="Будьте в курсі усіх виступів гурту Стороннім В." />
            </Helmet>

            <Container className="page">
                <SchedulesList/>
            </Container>
        </ScheduleContextProvider>
    );
};

export {Schedule};

import {Container} from "react-bootstrap";

import {Header} from "../elements/Header";
import {ScheduleContextProvider} from "../contexts/ScheduleContext";
import {Footer} from "../elements/Footer";

const Schedule = () => {
    return (
        <ScheduleContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <h1>Schedule</h1>
                <Footer />
            </Container>
        </ScheduleContextProvider>
    );
}

export { Schedule };
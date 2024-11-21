import React, {useContext} from "react";
import {Container} from "react-bootstrap";

import {GlobalContext} from "../contexts/GlobalContext";
import {ScheduleContextProvider} from "../contexts/ScheduleContext";

const Schedule = () => {
    const {setBgImage} = useContext(GlobalContext);
    setBgImage('photo.jpg');
    
    return (
        <ScheduleContextProvider>
            <Container>
                <h1>Schedule</h1>
            </Container>
        </ScheduleContextProvider>
    );
}

export { Schedule };
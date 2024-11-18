import {Container} from "react-bootstrap";

import {Header} from "../elements/Header";
import {GroupContextProvider} from "../contexts/GroupContext";

const Group = () => {
    return (
        <GroupContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <h1>GROUP</h1>
            </Container>
        </GroupContextProvider>
    );
};

export {Group};
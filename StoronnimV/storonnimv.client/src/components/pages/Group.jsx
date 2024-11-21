import {useContext} from "react";
import {Container} from "react-bootstrap";

import {GlobalContext} from "../contexts/GlobalContext";
import {GroupContextProvider} from "../contexts/GroupContext";

const Group = () => {
    const {setBgImage} = useContext(GlobalContext);
    setBgImage('photo.jpg');
    
    return (
        <GroupContextProvider>
            <Container>
                <h1>GROUP</h1>
            </Container>
        </GroupContextProvider>
    );
};

export { Group };
import {useContext} from "react";
import {Container} from "react-bootstrap";

import {GlobalContext} from "../contexts/GlobalContext";
import {ContactsContextProvider} from "../contexts/ContactsContext";

const Contacts = () => {
    const {setBgImage} = useContext(GlobalContext);
    setBgImage('photo.jpg');
    
    return (
        <ContactsContextProvider>
            <Container>
                <h1>CONTACTS</h1>
            </Container>
        </ContactsContextProvider>
    );
};

export {Contacts};
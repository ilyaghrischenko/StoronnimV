import {Container} from "react-bootstrap";

import {Header} from "./Header";
import {ContactsContextProvider} from "./contexts/ContactsContext";

const Contacts = () => {
    return (
        <ContactsContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <h1>CONTACTS</h1>
            </Container>
        </ContactsContextProvider>
    );
};

export {Contacts};
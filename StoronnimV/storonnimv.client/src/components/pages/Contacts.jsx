import {Container} from "react-bootstrap";

import {Header} from "../elements/Header";
import {ContactsContextProvider} from "../contexts/ContactsContext";
import {Footer} from "../elements/Footer";

const Contacts = () => {
    return (
        <ContactsContextProvider>
            <Container>
                <Header bgImage={'photo.jpg'}/>
                <h1>CONTACTS</h1>
                <Footer />
            </Container>
        </ContactsContextProvider>
    );
};

export {Contacts};
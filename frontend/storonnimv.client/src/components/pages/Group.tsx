import {FC} from "react";
import {GroupContextProvider} from "../contexts/GroupContext";
import {Container} from "react-bootstrap";
import {GroupDescription} from "../elements/group/GroupDescription";
import {Helmet} from "react-helmet-async";


const Group: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'group');

    return (
        <GroupContextProvider>
            <Helmet>
                <title>Група - Стороннім В</title>
                <meta name="description" content="Дізнайтеся більше про учасників та сам гурт Стороннім В." />
            </Helmet>

            <Container className='page group-page'>
                <h1 className="visually-hidden-heading">Група Стороннім В</h1>
                <GroupDescription/>
            </Container>
        </GroupContextProvider>
    );
};

export {Group};

import {FC, useContext, useEffect} from "react";
import {GroupContextProvider} from "../contexts/GroupContext";
import {Container} from "react-bootstrap";
import {GroupDescription} from "../elements/group/GroupDescription";
import {GlobalContext} from "../contexts/shared/GlobalContext.tsx";
import {Helmet} from "react-helmet-async";


const Group: FC = () => {
    sessionStorage.setItem('pressedButtonName', 'group');

    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {fetchIsAdmin} = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);

    return (
        <GroupContextProvider>
            <Helmet>
                <title>Група - Стороннім В</title>
                <meta name="description" content="Дізнайтеся більше про учасників та сам гурт Стороннім В." />
            </Helmet>

            <Container className='page'>
                <GroupDescription/>
            </Container>
        </GroupContextProvider>
    );
};

export {Group};
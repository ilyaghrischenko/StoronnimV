import React, {useContext} from "react";
import {Container} from "react-bootstrap";

import {HomeContextProvider} from "../contexts/HomeContext";
import {GlobalContext} from "../contexts/GlobalContext";

const Home = () => {
    const {setBgImage} = useContext(GlobalContext);
    setBgImage('photo.jpg');
    
    return (
        <HomeContextProvider>
            <Container>
                <h1>HOME</h1>
            </Container>
        </HomeContextProvider>
    );
};

export {Home};
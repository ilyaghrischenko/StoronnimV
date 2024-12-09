import '../../styles/elements/Header.css';

import React, {useContext} from 'react';
import {Container, Navbar, Nav} from 'react-bootstrap';
import {NavLink} from 'react-router-dom';
import {GlobalContext} from "../contexts/GlobalContext";

const Header = () => {
    const {bgImage} = useContext(GlobalContext);
    
    return (
        <Container
            style={{backgroundImage: `url(${bgImage})`}}
            className='header-container'>
            <Navbar bg="dark" variant="dark" expand="lg" sticky="top">
                <Container className='navbar-container'>
                    <Navbar.Brand
                        as={NavLink}
                        to="/">
                        
                        Стороннім В
                    </Navbar.Brand>
                    
                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="me-auto header">
                            <Nav.Link
                                as={NavLink}
                                to="/schedule"
                                className="link-item">

                                Афіша
                            </Nav.Link>
                            <Nav.Link
                                as={NavLink}
                                to="/news"
                                className="link-item">

                                Новини
                            </Nav.Link>
                            <Nav.Link
                                as={NavLink}
                                to="/music"
                                className="link-item">

                                Музика
                            </Nav.Link>
                            <Nav.Link
                                as={NavLink}
                                to="/group"
                                className="link-item">

                                Група
                            </Nav.Link>
                            <Nav.Link
                                as={NavLink}
                                to="/contacts"
                                className="link-item">

                                Контакти
                            </Nav.Link>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>
        </Container>
    );
};

export {Header};
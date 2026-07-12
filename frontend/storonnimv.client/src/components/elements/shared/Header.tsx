import {FC, useContext, useEffect, useState} from "react";
import {Button, Container, Nav, Navbar} from "react-bootstrap";
import {NavLink} from "react-router-dom";

// @ts-ignore
import Logo from '../../../assets/logo.svg?react';
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

const Header: FC = () => {
    const { sendRequest, isAdmin, setIsAdmin, serverRoute } = useContext(GlobalContext)!;
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState<boolean>(false);

    const logout = async () => {
        try {
            const response = await sendRequest(
                `${serverRoute}/admin/logout`,
                'POST'
            );

            if (response.status === 200) {
                setIsAdmin(false);
                sessionStorage.removeItem('role');
            }
        } catch (error) {
            console.error("Error while logging out: ", error);
        }
    };

    const savedPressedButtonName = sessionStorage.getItem('pressedButtonName') ?? '';
    const [pressedButtonName, setPressedButtonName] = useState<string>(savedPressedButtonName);

    useEffect(() => {
        if (!isMobileMenuOpen) {
            document.body.style.overflow = "";
            return;
        }

        const previousOverflow = document.body.style.overflow;
        document.body.style.overflow = "hidden";

        return () => {
            document.body.style.overflow = previousOverflow;
        };
    }, [isMobileMenuOpen]);

    const navLinkOnClick = (name: string) => {
        setPressedButtonName(name);
        sessionStorage.setItem('pressedButtonName', name);
        setIsMobileMenuOpen(false);
    };

    const navigationItems = [
        { to: "/schedule", name: "schedule", label: "Афіша" },
        { to: "/news", name: "news", label: "Новини" },
        { to: "/music", name: "music", label: "Музика" },
        { to: "/group", name: "group", label: "Група" },
        { to: "/video/sections", name: "video/sections", label: "Відео" }
    ];

    const renderNavLinks = (className?: string) => (
        <>
            {navigationItems.map((item) => (
                <Nav.Link
                    key={item.name}
                    as={NavLink}
                    to={item.to}
                    className={`navbar-container__link-item ${pressedButtonName !== item.name ? 'basic-button' : 'basic-button-pressed'} ${className ?? ''}`.trim()}
                    onClick={() => navLinkOnClick(item.name)}
                >
                    {item.label}
                </Nav.Link>
            ))}
        </>
    );

    return (
        <Container
            className="header-container"
        >
            <Navbar bg="dark" variant="dark" expand="lg" className="header-navbar">
                <Container className="navbar-container">
                    <div className="navbar-container__brand-group">
                        <Navbar.Brand
                            as={NavLink}
                            to="/"
                            className="navbar-container__brand"
                            onClick={() => navLinkOnClick('')}
                        >
                            <Logo className='navbar-container__logo' />
                        </Navbar.Brand>
                    </div>

                    <div className="navbar-container__desktop-links">
                        {renderNavLinks()}
                    </div>

                    <div className="navbar-container__utility-group">
                        {isAdmin && (
                            <Button onClick={logout} className="navbar-container__utility-button main-text">
                                Вийти
                            </Button>
                        )}

                        <div className="navbar-container__mobile-actions">
                            {!isMobileMenuOpen && (
                                <button
                                    type="button"
                                    className="navbar-container__burger"
                                    aria-label="Open navigation menu"
                                    onClick={() => setIsMobileMenuOpen(true)}
                                >
                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path
                                            d="M4 7h16M4 12h16M4 17h16"
                                            fill="none"
                                            stroke="currentColor"
                                            strokeWidth="2.75"
                                            strokeLinecap="round"
                                        />
                                    </svg>
                                </button>
                            )}
                        </div>
                    </div>
                </Container>
            </Navbar>

            <div
                className={`mobile-menu-overlay ${isMobileMenuOpen ? 'mobile-menu-overlay--open' : ''}`}
                onClick={() => setIsMobileMenuOpen(false)}
            />

            <aside className={`mobile-menu-drawer ${isMobileMenuOpen ? 'mobile-menu-drawer--open' : ''}`}>
                {isMobileMenuOpen && (
                    <button
                        type="button"
                        className="mobile-menu-drawer__close"
                        aria-label="Close navigation menu"
                        onClick={() => setIsMobileMenuOpen(false)}
                    >
                        <svg viewBox="0 0 24 24" aria-hidden="true">
                            <path
                                d="M6 6l12 12M18 6L6 18"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="2.75"
                                strokeLinecap="round"
                            />
                        </svg>
                    </button>
                )}

                <div className="mobile-menu-drawer__links">
                    {renderNavLinks("mobile-menu-drawer__link")}

                    {isAdmin && (
                        <Button
                            onClick={async () => {
                                await logout();
                                setIsMobileMenuOpen(false);
                            }}
                            className="navbar-container__utility-button main-text mobile-menu-drawer__link"
                        >
                            Вийти
                        </Button>
                    )}
                </div>
            </aside>
        </Container>
    );
};

export {Header};

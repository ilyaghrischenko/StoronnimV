import {FC, useCallback, useContext, useEffect, useRef, useState} from "react";
import {Button, Container, Nav, Navbar} from "react-bootstrap";
import {NavLink} from "react-router-dom";

// @ts-expect-error vite-plugin-svgr resolves the React component query during the Vite build.
import Logo from '../../../assets/logo.svg?react';
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

const Header: FC = () => {
    const { sendRequest, isAdmin, setIsAdmin, serverRoute } = useContext(GlobalContext)!;
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState<boolean>(false);
    const burgerRef = useRef<HTMLButtonElement>(null);
    const closeButtonRef = useRef<HTMLButtonElement>(null);
    const drawerRef = useRef<HTMLElement>(null);
    const desktopLinksRef = useRef<HTMLDivElement>(null);
    const restoreFocusRef = useRef<HTMLElement | null>(null);

    const closeMobileMenu = useCallback((restoreFocus = true) => {
        setIsMobileMenuOpen(false);

        if (restoreFocus) {
            window.requestAnimationFrame(() => restoreFocusRef.current?.focus());
        }
    }, []);

    const openMobileMenu = () => {
        restoreFocusRef.current = document.activeElement instanceof HTMLElement
            ? document.activeElement
            : burgerRef.current;
        setIsMobileMenuOpen(true);
    };

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
        if (!isMobileMenuOpen) return;
        const previousOverflow = document.body.style.overflow;
        document.body.style.overflow = "hidden";
        window.requestAnimationFrame(() => closeButtonRef.current?.focus());

        const handleKeyDown = (event: KeyboardEvent) => {
            if (event.key === "Escape") {
                event.preventDefault();
                closeMobileMenu();
                return;
            }

            if (event.key !== "Tab") return;

            const focusableElements = drawerRef.current?.querySelectorAll<HTMLElement>(
                'a[href], button:not([disabled]), [tabindex]:not([tabindex="-1"])'
            );
            if (!focusableElements?.length) return;

            const first = focusableElements[0];
            const last = focusableElements[focusableElements.length - 1];
            if (event.shiftKey && document.activeElement === first) {
                event.preventDefault();
                last.focus();
            } else if (!event.shiftKey && document.activeElement === last) {
                event.preventDefault();
                first.focus();
            }
        };

        const compactViewport = window.matchMedia("(max-width: 1024px)");
        const handleViewportChange = (event: MediaQueryListEvent) => {
            if (event.matches) return;

            closeMobileMenu(false);
            window.requestAnimationFrame(() => {
                desktopLinksRef.current?.querySelector<HTMLElement>('a[href]')?.focus();
            });
        };

        document.addEventListener("keydown", handleKeyDown);
        compactViewport.addEventListener("change", handleViewportChange);

        return () => {
            document.removeEventListener("keydown", handleKeyDown);
            compactViewport.removeEventListener("change", handleViewportChange);
            document.body.style.overflow = previousOverflow;
        };
    }, [closeMobileMenu, isMobileMenuOpen]);

    const navLinkOnClick = (name: string) => {
        setPressedButtonName(name);
        sessionStorage.setItem('pressedButtonName', name);
        closeMobileMenu();
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
                            aria-label="Головна — Стороннім В"
                            onClick={() => navLinkOnClick('')}
                        >
                            <Logo aria-hidden="true" focusable="false" className='navbar-container__logo' />
                        </Navbar.Brand>
                    </div>

                    <div ref={desktopLinksRef} className="navbar-container__desktop-links">
                        {renderNavLinks()}
                    </div>

                    <div className="navbar-container__utility-group">
                        {isAdmin && (
                            <Button onClick={logout} className="navbar-container__utility-button main-text">
                                Вийти
                            </Button>
                        )}

                        <div className="navbar-container__mobile-actions">
                            <button
                                ref={burgerRef}
                                type="button"
                                className="navbar-container__burger"
                                aria-label={isMobileMenuOpen ? "Закрити основну навігацію" : "Відкрити основну навігацію"}
                                aria-expanded={isMobileMenuOpen}
                                aria-controls="mobile-navigation-drawer"
                                onClick={isMobileMenuOpen ? () => closeMobileMenu() : openMobileMenu}
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
                        </div>
                    </div>
                </Container>
            </Navbar>

            <div
                className={`mobile-menu-overlay ${isMobileMenuOpen ? 'mobile-menu-overlay--open' : ''}`}
                aria-hidden="true"
                onClick={() => closeMobileMenu()}
            />

            <aside
                ref={drawerRef}
                id="mobile-navigation-drawer"
                aria-label="Основна навігація"
                aria-hidden={!isMobileMenuOpen}
                className={`mobile-menu-drawer ${isMobileMenuOpen ? 'mobile-menu-drawer--open' : ''}`}
            >
                {isMobileMenuOpen && (
                    <>
                    <button
                        ref={closeButtonRef}
                        type="button"
                        className="mobile-menu-drawer__close"
                        aria-label="Закрити основну навігацію"
                        onClick={() => closeMobileMenu()}
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

                    <nav className="mobile-menu-drawer__links" aria-label="Посилання основної навігації">
                        {renderNavLinks("mobile-menu-drawer__link")}

                        {isAdmin && (
                            <Button
                                onClick={async () => {
                                    await logout();
                                    closeMobileMenu();
                                }}
                                className="navbar-container__utility-button main-text mobile-menu-drawer__link"
                            >
                                Вийти
                            </Button>
                        )}
                    </nav>
                    </>
                )}
            </aside>
        </Container>
    );
};

export {Header};

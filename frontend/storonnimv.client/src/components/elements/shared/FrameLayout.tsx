import React, { FC } from 'react';
// @ts-expect-error vite-plugin-svgr resolves the React component query during the Vite build.
import FrameSVG from '../../../assets/frame.svg?react';

interface FrameLayoutProps {
    children: React.ReactNode;
    header: React.ReactNode;
    footer: React.ReactNode;
}

const FrameLayout: FC<FrameLayoutProps> = ({ children, header, footer }) => {
    return (
        <div className="frame">
            <a
                className="skip-link"
                href="#main-content"
                onClick={() => document.getElementById('main-content')?.focus()}
            >
                Перейти до основного вмісту
            </a>
            <FrameSVG
                aria-hidden="true"
                focusable="false"
                className="frame__svg"
                preserveAspectRatio="none"
            />

            <div className="frame__content">
                {header && <header className="frame__content-header">{header}</header>}
                <main id="main-content" tabIndex={-1} className="frame__content-main">
                    {children}
                </main>
                {footer && <footer className="frame__content-footer">{footer}</footer>}
            </div>
        </div>
    );
};

export { FrameLayout };

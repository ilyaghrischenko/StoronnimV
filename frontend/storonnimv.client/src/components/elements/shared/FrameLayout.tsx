import React, { FC } from 'react';
// @ts-ignore
import FrameSVG from '../../../assets/frame.svg?react';

interface FrameLayoutProps {
    /** Контент основной части (слева) */ children: React.ReactNode;
    /** Контент навбара или хедера (справа) */ nav: React.ReactNode;
}

const FrameLayout: FC<FrameLayoutProps> = ({ children, nav }) => {
    return (
        <div className="frame">
            {/* SVG фон */}
            <FrameSVG className="frame__svg" preserveAspectRatio="none" />

            {/* Контейнер содержимого */}
            <div className="frame__content">
                <div className="frame__content-main">
                    {children}
                </div>
                <div className="frame__content-navbar">
                    {nav}
                </div>
            </div>
        </div>
    );
};

export { FrameLayout };

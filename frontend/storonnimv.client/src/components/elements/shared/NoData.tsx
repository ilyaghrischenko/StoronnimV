import {CSSProperties, FC} from "react";

// @ts-expect-error vite-plugin-svgr resolves the React component query during the Vite build.
import NoDataImage from '../../../assets/no-data.svg?react';

interface IStyle {
    div?: CSSProperties;
    text?: CSSProperties;
    image?: CSSProperties;
}

interface INoDataProps {
    style?: IStyle;
    className?: string;
    message?: string;
    actionLabel?: string;
    onAction?: () => void;
    variant?: "empty" | "error";
}

const NoData: FC<INoDataProps> = ({
    style,
    className,
    message = "Даних немає",
    actionLabel,
    onAction,
    variant = "empty"
}) => {
    const classes = [
        "empty-data-container",
        variant === "error" ? "empty-data-container--error" : "",
        className ?? ""
    ].filter(Boolean).join(" ");

    return (
        <div
            className={classes}
            style={style?.div}
            role={variant === "error" ? "alert" : "status"}
        >
            <NoDataImage className='empty-data-container__image' style={style?.image} />
            <h1 className='empty-data-container__text main-text big-shadow' style={style?.text}>{message}</h1>
            {onAction && actionLabel &&
                <button
                    type='button'
                    className='empty-data-container__action basic-button'
                    onClick={onAction}
                >
                    {actionLabel}
                </button>}
        </div>
    );
};

export {NoData};

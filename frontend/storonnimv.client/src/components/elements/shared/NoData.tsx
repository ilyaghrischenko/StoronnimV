import {CSSProperties, FC} from "react";

// @ts-ignore
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
}

const NoData: FC<INoDataProps> = ({style, className, message = "Даних немає"}) => {
    return (
        <div className={`${className} empty-data-container`} style={style?.div}>
            <NoDataImage className='empty-data-container__image' style={style?.image} />
            <h1 className='empty-data-container__text main-text big-shadow' style={style?.text}>{message}</h1>
        </div>
    );
};

export {NoData};
import {FC} from 'react';

interface IPreloaderTileProps {
    className?: string;
}

const PreloaderTile: FC<IPreloaderTileProps> = ({className}) => {
    return (
        <div className={`preloader-tile ${className}`}>
            <div className="preloader-tile__animation"></div>
        </div>
    );
};

export default PreloaderTile;

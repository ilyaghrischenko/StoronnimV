import {FC} from 'react';

interface IPreloaderTileProps {
    className?: string;
    announce?: boolean;
}

const PreloaderTile: FC<IPreloaderTileProps> = ({className, announce = false}) => {
    return (
        <div
            className={`preloader-tile ${className ?? ""}`.trim()}
            role={announce ? "status" : undefined}
            aria-live={announce ? "polite" : undefined}
            aria-hidden={announce ? undefined : true}
        >
            {announce && <span className="visually-hidden-heading">Завантаження…</span>}
            <div className="preloader-tile__animation" aria-hidden="true"></div>
        </div>
    );
};

export default PreloaderTile;

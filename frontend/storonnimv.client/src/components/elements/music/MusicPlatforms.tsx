import {FC, useContext, useEffect} from "react";
import {ListGroup} from "react-bootstrap";
import {MusicPlatformItem} from "./MusicPlatformItem";
import {MusicContext} from "../../contexts/MusicContext";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {NoData} from "../shared/NoData.tsx";

const MusicPlatforms: FC = () => {
    const musicContext = useContext(MusicContext)!;
    const {musicPlatforms, musicStatus, fetchMusicPlatforms} = musicContext;

    useEffect(() => {
        void fetchMusicPlatforms();
    }, [fetchMusicPlatforms]);

    if (musicStatus === "loading") {
        return (
            <ListGroup as="ul" className='music-platforms-container'>
                {Array(3).fill(null).map((_, index) =>
                    <ListGroup.Item as="li" key={index}>
                        <PreloaderTile
                            announce={index === 0}
                            className='preloader-tile__container-music-page position-relative'/>
                    </ListGroup.Item>
                )}
            </ListGroup>
        );
    }

    if (musicStatus === "error") {
        return <NoData
            variant="error"
            message='Не вдалося завантажити музичні платформи'
            actionLabel='Спробувати ще раз'
            onAction={() => void fetchMusicPlatforms()}
        />;
    }

    if (musicStatus === "empty") {
        return <NoData message='Музичних платформ немає'/>;
    }

    return (
        <ListGroup as="ul" className='music-platforms-container'>
            {musicPlatforms.map((item) =>
                <MusicPlatformItem item={item} key={item.id}/>) }
        </ListGroup>
    );
};

export {MusicPlatforms};

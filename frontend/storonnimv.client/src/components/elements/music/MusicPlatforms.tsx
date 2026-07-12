import {FC, useContext, useEffect} from "react";
import {ListGroup} from "react-bootstrap";
import {MusicPlatformItem} from "./MusicPlatformItem";
import {MusicContext} from "../../contexts/MusicContext";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {NoData} from "../shared/NoData.tsx";

const MusicPlatforms: FC = () => {
    const musicContext = useContext(MusicContext)!;
    const globalContext = useContext(GlobalContext)!;

    const {pageLoading, checkIfNoData} = globalContext;
    const {musicPlatforms, fetchMusicPlatforms} = musicContext;

    useEffect(() => {
        fetchMusicPlatforms();
    }, []);

    return (
        <>
            {!pageLoading ?
                (checkIfNoData(() => !musicPlatforms || musicPlatforms.length === 0) ?
                 <NoData style={{div: {width: '25%'}, text: {fontSize: '1.5vw'}, image: {width: '40%'}}} message='Музичних платформ немає' /> : <ListGroup className='music-platforms-container'>
                        {musicPlatforms.map((item) =>
                            <MusicPlatformItem item={item} key={item.id}/>)}
                    </ListGroup>)
                :
                <ListGroup className='music-platforms-container'>
                    {Array(3).fill(null).map(() =>
                        <PreloaderTile className='preloader-tile__container-music-page position-relative'/>
                    )
                    }
                </ListGroup>
            }
        </>
    );
};

export {MusicPlatforms};
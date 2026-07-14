import {FC, useContext, useEffect} from "react";
import {Container} from "react-bootstrap";
import {HomeContext} from "../../contexts/HomeContext";
import {NoData} from "../shared/NoData.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {Link} from "react-router-dom";

interface PromotionVideoHomeProps {
    className?: string;
}

const PromotionVideoHome: FC<PromotionVideoHomeProps> = ({className}) => {
    const homeContext = useContext(HomeContext)!;

    const {homePromotionVideo, homePromotionVideoStatus, fetchHomePromotionVideo} = homeContext;

    useEffect(() => {
        void fetchHomePromotionVideo();
    }, [fetchHomePromotionVideo]);

    if (homePromotionVideoStatus === "loading") {
        return <PreloaderTile className={`promotion-video-home-container ${className ?? ""}`}/>;
    }

    if (homePromotionVideoStatus === "error") {
        return <NoData
            className={className}
            message='Не вдалося завантажити відео'
            actionLabel='Спробувати ще раз'
            onAction={fetchHomePromotionVideo}
        />;
    }

    if (homePromotionVideoStatus === "empty" || !homePromotionVideo?.url) {
        return <NoData className={className} message='Відео немає'/>;
    }

    return (
        <Container className={`promotion-video-home-container ${className}`}>
            <video
                className='promotion-video-home-container__video'
                controls
                preload="auto"
                autoPlay
                muted
                loop
            >
                <source src={homePromotionVideo.url} type='video/mp4'/>
            </video>
            <Link
                aria-label={homePromotionVideo.title}
                className='promotion-video-home-container__section-link basic-button'
                to={`/video/section?videoType=${homePromotionVideo.type}`}
            >
                До розділу
            </Link>
        </Container>
    );
};

export {PromotionVideoHome};

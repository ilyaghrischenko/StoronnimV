import {FC, useContext, useEffect} from "react";
import {Container} from "react-bootstrap";
import {HomeContext} from "../../contexts/HomeContext";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {NoData} from "../shared/NoData.tsx";

interface PromotionVideoHomeProps {
    className?: string;
}

const PromotionVideoHome: FC<PromotionVideoHomeProps> = ({className}) => {
    const globalContext = useContext(GlobalContext)!;
    const homeContext = useContext(HomeContext)!;

    const {checkIfNoData} = globalContext;
    const {homePromotionVideo, fetchHomePromotionVideo} = homeContext;

    useEffect(() => {
        fetchHomePromotionVideo();
    }, []);

     if (checkIfNoData(() => !homePromotionVideo)) {
         return <NoData className={className} message='Відео немає' />;
     }

    return (
        <Container className={`promotion-video-home-container ${className}`}>
            {homePromotionVideo.url && (
                <video
                    className='promotion-video-home-container__video'
                    controls
                    preload="auto"
                    autoPlay
                    muted
                    loop
                >
                    <source src={homePromotionVideo.url} type='video/mp4' />
                </video>
            )}
        </Container>
    );
};

export {PromotionVideoHome};
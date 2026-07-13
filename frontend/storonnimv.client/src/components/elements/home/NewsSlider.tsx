import {FC, useContext, useEffect} from "react";
import {HomeContext} from "../../contexts/HomeContext.tsx";
// @ts-expect-error-ignore
import "swiper/css/bundle";
import {Swiper, SwiperSlide} from "swiper/react";
import {Navigation, Autoplay} from "swiper/modules";
import {NewsHomeListItem} from "./NewsHomeListItem.tsx";
import {Container} from "react-bootstrap";
import {NoData} from "../shared/NoData.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";

interface NewsComponentProps {
    className?: string;
}


const NewsSlider: FC<NewsComponentProps> = ({className}) => {
    const homeContext = useContext(HomeContext)!;

    const {homeNewsList, homeNewsStatus, fetchHomeNewsList} = homeContext;

    useEffect(() => {
        fetchHomeNewsList();
    }, []);

    if (homeNewsStatus === "loading") {
        return <PreloaderTile className={`${className ?? ""} news-slider`}/>;
    }

    if (homeNewsStatus === "error") {
        return <NoData className={className} message='Не вдалося завантажити новини'/>;
    }

    if (homeNewsStatus === "empty") {
        return <NoData className={className} message='Важливих новин немає'/>;
    }

    return (
        <Container className={`${className} news-slider`}>
            <Swiper
                key={homeNewsList.length}
                modules={[Navigation, Autoplay]}
                slidesPerView={3}
                spaceBetween={20}
                navigation
                autoplay={{delay: 3000, disableOnInteraction: false}}
                loop={homeNewsList.length > 3}
                speed={1800}
            >
                {homeNewsList.map((news, index) => (
                    <SwiperSlide key={index}>
                        <NewsHomeListItem item={news}/>
                    </SwiperSlide>
                ))}
            </Swiper>
        </Container>
    );
};

export {NewsSlider};

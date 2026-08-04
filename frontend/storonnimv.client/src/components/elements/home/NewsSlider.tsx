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
import {usePrefersReducedMotion} from "../../../hooks/usePrefersReducedMotion.ts";

interface NewsComponentProps {
    className?: string;
}


const NewsSlider: FC<NewsComponentProps> = ({className}) => {
    const homeContext = useContext(HomeContext)!;
    const prefersReducedMotion = usePrefersReducedMotion();

    const {homeNewsList, homeNewsStatus, fetchHomeNewsList} = homeContext;

    useEffect(() => {
        void fetchHomeNewsList();
    }, [fetchHomeNewsList]);

    if (homeNewsStatus === "loading") {
        return <PreloaderTile announce className={`${className ?? ""} news-slider`}/>;
    }

    if (homeNewsStatus === "error") {
        return <NoData
            className={className}
            variant="error"
            message='Не вдалося завантажити новини'
            actionLabel='Спробувати ще раз'
            onAction={fetchHomeNewsList}
        />;
    }

    if (homeNewsStatus === "empty") {
        return <NoData className={className} message='Важливих новин немає'/>;
    }

    return (
        <Container className={`${className} news-slider`}>
            <Swiper
                key={`${homeNewsList.length}-${prefersReducedMotion ? "reduced" : "full"}`}
                modules={[Navigation, Autoplay]}
                slidesPerView={1}
                spaceBetween={12}
                breakpoints={{
                    640: {slidesPerView: 2, spaceBetween: 16},
                    1024: {slidesPerView: 3, spaceBetween: 20},
                }}
                navigation
                autoplay={prefersReducedMotion ? false : {delay: 3000, disableOnInteraction: false}}
                loop={homeNewsList.length > 3}
                speed={prefersReducedMotion ? 0 : 1800}
            >
                {homeNewsList.map((news) => (
                    <SwiperSlide key={news.id}>
                        <NewsHomeListItem item={news}/>
                    </SwiperSlide>
                ))}
            </Swiper>
        </Container>
    );
};

export {NewsSlider};

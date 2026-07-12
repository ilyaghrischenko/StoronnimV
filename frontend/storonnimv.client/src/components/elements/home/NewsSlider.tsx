import {FC, useContext, useEffect} from "react";
import {HomeContext} from "../../contexts/HomeContext.tsx";
// @ts-expect-error-ignore
import "swiper/css/bundle";
import {Swiper, SwiperSlide} from "swiper/react";
import {Navigation, Autoplay} from "swiper/modules";
import {NewsHomeListItem} from "./NewsHomeListItem.tsx";
import {Container} from "react-bootstrap";
import {NoData} from "../shared/NoData.tsx";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

interface NewsComponentProps {
    className?: string;
}


const NewsSlider: FC<NewsComponentProps> = ({className}) => {
    const globalContext = useContext(GlobalContext)!;
    const homeContext = useContext(HomeContext)!;

    const {checkIfNoData} = globalContext;
    const {homeNewsList, fetchHomeNewsList} = homeContext;

    useEffect(() => {
        fetchHomeNewsList();
    }, []);

    if (checkIfNoData(() => !homeNewsList || homeNewsList.length === 0)) {
        return <NoData className={className} message='Важливих новин немає' />
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
                loop
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

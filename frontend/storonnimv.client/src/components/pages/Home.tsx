import {FC} from "react";
import {HomeContextProvider} from "../contexts/HomeContext";
import {Container} from "react-bootstrap";
import {ScheduleHomeContainer} from "../elements/home/ScheduleHomeContainer";
import {PromotionVideoHome} from "../elements/home/PromotionVideoHome";
import {NewsSlider} from "../elements/home/NewsSlider";
import {Helmet} from "react-helmet-async";

const Home: FC = () => {
    sessionStorage.setItem('pressedButtonName', '');

    return (
        <HomeContextProvider>
            <Helmet>
                <title>Стороннім В</title>
                <meta name="description" content="УкраЇнська рок група Стороннім В" />
            </Helmet>

            <Container className='home-page page'>
                <h1 className="visually-hidden-heading">Стороннім В</h1>
                <ScheduleHomeContainer className='schedule-grid'/>
                <NewsSlider className='news-grid home-container-border'/>
                <PromotionVideoHome className='video-grid home-container-border'/>
            </Container>
        </HomeContextProvider>
    );
};

export {Home};

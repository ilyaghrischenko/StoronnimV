import {FC, useContext, useEffect} from "react";
import {HomeContextProvider} from "../contexts/HomeContext";
import {Container} from "react-bootstrap";
import {ScheduleHomeContainer} from "../elements/home/ScheduleHomeContainer";
import {PromotionVideoHome} from "../elements/home/PromotionVideoHome";
import {NewsSlider} from "../elements/home/NewsSlider";
import {GlobalContext} from "../contexts/shared/GlobalContext.tsx";
import {Helmet} from "react-helmet-async";

const Home: FC = () => {
    sessionStorage.setItem('pressedButtonName', '');

    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {fetchIsAdmin} = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);

    return (
        <HomeContextProvider>
            <Helmet>
                <title>Стороннім В</title>
                <meta name="description" content="УкраЇнська рок група Стороннім В" />
            </Helmet>

            <Container className='home-page page'>
                <ScheduleHomeContainer className='schedule-grid'/>
                <NewsSlider className='news-grid home-container-border'/>
                <PromotionVideoHome className='video-grid home-container-border'/>
            </Container>
        </HomeContextProvider>
    );
};

export {Home};
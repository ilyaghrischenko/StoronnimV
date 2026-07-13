import {FC, useContext, useEffect} from "react";
import {HomeContext} from "../../contexts/HomeContext";
import {Container, Image} from "react-bootstrap";
import {NoData} from "../shared/NoData.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";

interface ScheduleHomeContainerProps {
    className?: string;
}

const ScheduleHomeContainer: FC<ScheduleHomeContainerProps> = ({className}) => {
    const homeContext = useContext(HomeContext)!;

    const {homeSchedule, homeScheduleStatus, fetchHomeSchedule, onClickHomeElementHandler} = homeContext;

    useEffect(() => {
        fetchHomeSchedule();
    }, []);

    if (homeScheduleStatus === "loading") {
        return <PreloaderTile className={`schedule-home-container ${className ?? ""}`}/>;
    }

    if (homeScheduleStatus === "error") {
        return <NoData className={className} message='Не вдалося завантажити афішу'/>;
    }

    if (homeScheduleStatus === "empty" || !homeSchedule?.photo) {
        return <NoData className={className} message='Афіш немає'/>;
    }

    return (
        <Container
            className={`schedule-home-container ${className}`}
            onClick={() => onClickHomeElementHandler('schedule')}>
                <Image className='schedule-home-container__image' src={homeSchedule.photo}/>
        </Container>
    );
};

export {ScheduleHomeContainer};

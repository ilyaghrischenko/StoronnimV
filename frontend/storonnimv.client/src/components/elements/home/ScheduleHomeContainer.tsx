import {FC, useContext, useEffect} from "react";
import {HomeContext} from "../../contexts/HomeContext";
import {Container, Image} from "react-bootstrap";
import {NoData} from "../shared/NoData.tsx";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

interface ScheduleHomeContainerProps {
    className?: string;
}

const ScheduleHomeContainer: FC<ScheduleHomeContainerProps> = ({className}) => {
    const globalContext = useContext(GlobalContext)!;
    const homeContext = useContext(HomeContext)!;

    const {checkIfNoData} = globalContext;
    const {homeSchedule, fetchHomeSchedule, onClickHomeElementHandler} = homeContext;

    useEffect(() => {
        fetchHomeSchedule();
    }, []);

    return (
        <Container
            className={`schedule-home-container ${className}`}
            onClick={() => onClickHomeElementHandler('schedule')}>
                {checkIfNoData(() => !!homeSchedule.photo) ?
                    <Image className='schedule-home-container__image' src={homeSchedule.photo}/>
                    : <NoData message='Афіш немає' />}
        </Container>
    );
};

export {ScheduleHomeContainer};
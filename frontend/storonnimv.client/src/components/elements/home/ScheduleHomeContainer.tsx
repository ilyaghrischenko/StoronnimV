import {FC, useContext, useEffect} from "react";
import {HomeContext} from "../../contexts/HomeContext";
import {Image} from "react-bootstrap";
import {NoData} from "../shared/NoData.tsx";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {Link} from "react-router-dom";

interface ScheduleHomeContainerProps {
    className?: string;
}

const ScheduleHomeContainer: FC<ScheduleHomeContainerProps> = ({className}) => {
    const homeContext = useContext(HomeContext)!;

    const {homeSchedule, homeScheduleStatus, fetchHomeSchedule} = homeContext;

    useEffect(() => {
        void fetchHomeSchedule();
    }, [fetchHomeSchedule]);

    if (homeScheduleStatus === "loading") {
        return <PreloaderTile className={`schedule-home-container ${className ?? ""}`}/>;
    }

    if (homeScheduleStatus === "error") {
        return <NoData
            className={className}
            variant="error"
            message='Не вдалося завантажити афішу'
            actionLabel='Спробувати ще раз'
            onAction={fetchHomeSchedule}
        />;
    }

    if (homeScheduleStatus === "empty" || !homeSchedule?.photo) {
        return <NoData className={className} message='Афіш немає'/>;
    }

    return (
        <Link
            aria-label={homeSchedule.title}
            className={`schedule-home-container ${className}`}
            to='/schedule'
        >
            <Image className='schedule-home-container__image' src={homeSchedule.photo} alt=''/>
        </Link>
    );
};

export {ScheduleHomeContainer};

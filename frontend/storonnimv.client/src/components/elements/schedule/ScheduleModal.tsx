import {FC, useContext, useEffect} from "react";
import {ScheduleContext} from "../../contexts/ScheduleContext.tsx";
import {Button, Container, Image} from "react-bootstrap";
import {ModalLoading} from "../shared/ModalLoading.tsx";
import {DeleteScheduleModal} from "./forms/DeleteScheduleModal.tsx";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {LocationMap} from "./LocationMap.tsx";
import {FaEdit, FaTrash} from "react-icons/fa";
import {EditScheduleModal} from "./forms/EditScheduleModal.tsx";
import {NoData} from "../shared/NoData.tsx";

interface ScheduleModalProps {
    scheduleId: number;
}

const ScheduleModal: FC<ScheduleModalProps> = ({scheduleId}) => {
    const globalContext = useContext(GlobalContext)!;
    const scheduleContext = useContext(ScheduleContext)!;

    const {isAdmin, modalLoading, OnShowModal} = globalContext;
    const {fetchScheduleFullInfo, scheduleFullInfo, scheduleFullInfoStatus} = scheduleContext;

    useEffect(() => {
        fetchScheduleFullInfo(scheduleId);
    }, [fetchScheduleFullInfo, scheduleId]);

    if (modalLoading || scheduleFullInfoStatus === "loading") {
        return <ModalLoading/>;
    }

    if (scheduleFullInfoStatus === "error") {
        return <NoData
            variant="error"
            message="Не вдалося завантажити афішу"
            actionLabel="Спробувати ще раз"
            onAction={() => void fetchScheduleFullInfo(scheduleId)}
        />;
    }

    if (scheduleFullInfoStatus === "empty" || !scheduleFullInfo) {
        return <NoData message="Афішу не знайдено"/>;
    }

    return (
        <Container className="schedule-modal">
            <div className='schedule-modal__container'>
                <div className='schedule-modal__photo-container'>
                    {scheduleFullInfo.photo &&
                        <Image
                            alt={`Фото афіші «${scheduleFullInfo.title}»`}
                            className="schedule-modal__photo"
                            src={scheduleFullInfo.photo}
                        />}
                </div>

                <div className="schedule-modal__info">
                    <h1 className="schedule-modal__info-title main-text">{scheduleFullInfo.title}</h1>
                    <h2 className="schedule-modal__info-datetime">{scheduleFullInfo.performanceDateTime}</h2>
                    <p className="schedule-modal__info-location">{scheduleFullInfo.location}</p>
                    <p className="schedule-modal__info-status">{scheduleFullInfo.status}</p>
                    <LocationMap address={scheduleFullInfo.location}/>

                    {isAdmin &&
                        <>
                            <Button
                                aria-label="Редагувати афішу"
                                className="admin-button__edit"
                                onClick={() => OnShowModal(<EditScheduleModal item={scheduleFullInfo}/>)}
                            >
                                <FaEdit/>
                            </Button>

                            <Button
                                aria-label="Видалити афішу"
                                className="admin-button__delete"
                                onClick={() => OnShowModal(<DeleteScheduleModal itemId={scheduleFullInfo.id}/>)}
                            >
                                <FaTrash/>
                            </Button>
                        </>
                    }
                </div>
            </div>

            <div className='schedule-modal__description-container'>
                <p className="schedule-modal__description-container-description secondary-text">{scheduleFullInfo.description}</p>
            </div>
        </Container>
    );
};

export {ScheduleModal};

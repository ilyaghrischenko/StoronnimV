import {FC, useContext} from "react";
import {IScheduleListItem} from "../../../models/schedule/IScheduleListItem";
import {Image} from "react-bootstrap";
import {ScheduleContextProvider} from "../../contexts/ScheduleContext.tsx";
import {ScheduleModal} from "./ScheduleModal.tsx";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

interface ScheduleListItemProps {
    schedule: IScheduleListItem;
}

export const ScheduleListItem: FC<ScheduleListItemProps> = ({schedule}) => {
    const globalContext = useContext(GlobalContext)!;

    const {OnShowModal} = globalContext;

    return (
        <button
            type="button"
            aria-label={`Відкрити афішу «${schedule.title}»`}
            className='schedule-list-item'
            onClick={() =>
                OnShowModal(
                    <ScheduleContextProvider>
                        <ScheduleModal scheduleId={schedule.id} />
                    </ScheduleContextProvider>
                )
            }
        >
            <div className='schedule-list-item__content'>
                {schedule.photo && (
                    <Image
                        alt={`Фото афіші «${schedule.title}»`}
                        className='schedule-list-item__photo'
                        src={schedule.photo}
                        fluid
                    />
                )}
                <div className='schedule-list-item__info'>
                    <p className='schedule-list-item__title'>{schedule.title}</p>
                    <p className='schedule-list-item__date'>{schedule.performanceDateTime}</p>
                    <p className='schedule-list-item__location'>{schedule.location}</p>
                    <p className='schedule-list-item__status'>{schedule.status}</p>
                </div>
            </div>
        </button>
    );
};

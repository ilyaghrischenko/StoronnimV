import {FC, useContext, useEffect} from "react";
import {ScheduleListItem} from "./ScheduleListItem";
import {ScheduleContext} from "../../contexts/ScheduleContext";
import {GlobalContext} from "../../contexts/shared/GlobalContext";
import {Button} from "react-bootstrap";
import {PaginationSection} from "../shared/PaginationSection.tsx";
import {AddScheduleModal} from "./forms/AddScheduleModal.tsx";
import {FaPlus} from "react-icons/fa";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {NoData} from "../shared/NoData.tsx";

const SchedulesList: FC = () => {
    const scheduleContext = useContext(ScheduleContext)!;
    const globalContext = useContext(GlobalContext)!;

    const {OnShowModal, isAdmin} = globalContext;
    const {paginate, fetchSchedules, schedules, schedulesStatus, currentPage, totalPages} = scheduleContext;

    const addScheduleButton = isAdmin && (
        <Button
            aria-label="Додати афішу"
            className="admin-button__add"
            onClick={() => OnShowModal(<AddScheduleModal/>)}>
            <FaPlus/>
        </Button>
    );

    useEffect(() => {
        void fetchSchedules(1, 3);
    }, [fetchSchedules]);

    if (schedulesStatus === "loading") {
        return (
            <div className="schedules-list">
                <div className="schedules-list__items">
                    {Array.from({length: 3}, (_, index) => (
                        <PreloaderTile
                            key={index}
                            className="preloader-tile__container-schedule-page"
                        />
                    ))}
                </div>
            </div>
        );
    }

    if (schedulesStatus === "error") {
        return (
            <NoData
                variant="error"
                message="Не вдалося завантажити афіші"
                actionLabel="Спробувати ще раз"
                onAction={() => void fetchSchedules(currentPage, 3)}
            />
        );
    }

    if (schedulesStatus === "empty") {
        return (
            <div className="schedules-list">
                {addScheduleButton}
                <NoData message="Афіш немає"/>
            </div>
        );
    }

    return (
        <div className='schedules-list'>
            <div className='schedules-list__container'>
                {addScheduleButton}

                <div className="schedules-list__items">
                    {schedules.map((schedule) => (
                        <ScheduleListItem
                            key={schedule.id}
                            schedule={schedule}
                        />
                    ))}
                </div>
            </div>

            <PaginationSection
                currentPage={currentPage}
                totalPages={totalPages}
                paginate={paginate}
                compactOnMobile
            />
        </div>
    );
};

export {SchedulesList};

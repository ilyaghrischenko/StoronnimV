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

    const {OnShowModal, pageLoading, isAdmin, checkIfNoData} = globalContext;
    const {paginate, schedules, currentPage, totalPages} = scheduleContext;

    useEffect(() => {
        const savedPage = sessionStorage.getItem("schedulesCurrentPage");
        const page = savedPage ? Number(savedPage) : 1;

        paginate(page);
    }, []);

    if (checkIfNoData(() => !schedules || schedules.length === 0)) {
        return <NoData message='Афіш немає' />;
    }

    return (
        <div className='schedules-list'>
            <div className='schedules-list__container'>
                {isAdmin && (
                    <Button
                        className="admin-button__add"
                        onClick={() => OnShowModal(<AddScheduleModal/>)}>
                        <FaPlus/>
                    </Button>
                )}

                <div className="schedules-list__items">
                    {pageLoading
                        ? Array(3).fill(null).map((_, i) => (
                            <PreloaderTile
                                key={i}
                                className="preloader-tile__container-schedule-page"
                            />
                        ))
                        : schedules.map((schedule) => (
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
                paginate={paginate}/>
        </div>
    );
};

export {SchedulesList};

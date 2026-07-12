import React, {createContext, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IScheduleListItem} from "../../models/schedule/IScheduleListItem";
import {ISchedule} from "../../models/schedule/ISchedule.ts";
import {IPaginationResponse} from "../../models/shared/IPaginationResponse.ts";

// Тип контекста
interface ScheduleContextType {
    schedules: IScheduleListItem[];
    fetchSchedules: (pageNumber: number, pageSize: number) => Promise<void>;
    scheduleFullInfo: ISchedule;
    fetchScheduleFullInfo: (scheduleId: number) => Promise<void>;
    currentPage: number;
    totalPages: number;
    paginate: (pageNumber: number, pageSize?: number) => void;
}

// Создаем контекст с типизацией
const ScheduleContext = createContext<ScheduleContextType | undefined>(undefined);

interface ScheduleContextProviderProps {
    children: ReactNode;
}

const ScheduleContextProvider: React.FC<ScheduleContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setPageLoading, setModalLoading, serverRoute} = globalContext;

    const [schedules, setSchedules] = useState<IScheduleListItem[]>([]);
    const [scheduleFullInfo, setScheduleFullInfo] = useState<ISchedule>({} as ISchedule);

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(1);

    const paginate =
        async (pageNumber: number, pageSize: number = 3): Promise<void> => {

            const savedTotalPagesString = sessionStorage.getItem("schedulesTotalPages");
            const savedTotalPages = savedTotalPagesString ? Number(savedTotalPagesString) : 0;

            if (savedTotalPages === 0) {
                await fetchSchedules(pageNumber, pageSize);
            }

            if (pageNumber >= 1 && pageNumber <= savedTotalPages) {
                await fetchSchedules(pageNumber, pageSize);
            }
        }

    const fetchScheduleFullInfo = async (scheduleId: number): Promise<void> => {
        try {
            setModalLoading(true);
            const response = await sendRequest(`${serverRoute}/schedules/${scheduleId}`);

            const data: ISchedule = response.data;

            setScheduleFullInfo(data);
        } catch (error) {
            console.error("Error fetching schedule full info:", error);
            return;
        } finally {
            setModalLoading(false);
        }
    };

    const fetchSchedules =
        async (pageNumber: number = currentPage, pageSize: number = 3): Promise<void> => {
            try {
                setPageLoading(true);
                const response = await sendRequest(
                    `${serverRoute}/schedules/page/${pageNumber}?pageSize=${pageSize}`
                );

                const data: IPaginationResponse<IScheduleListItem> = response.data;

                setSchedules(data.items);
                setCurrentPage(data.currentPage);
                setTotalPages(data.totalPages);

                sessionStorage.setItem("schedulesCurrentPage", String(data.currentPage));
                sessionStorage.setItem("schedulesTotalPages", String(data.totalPages));
            } catch (error) {
                console.error("Error while fetching schedules: ", error);
            } finally {
                setPageLoading(false);
            }
        };

    const value: ScheduleContextType = {
        fetchScheduleFullInfo,
        scheduleFullInfo,
        schedules,
        fetchSchedules,
        currentPage,
        totalPages,
        paginate
    };

    return (
        <ScheduleContext.Provider value={value}>
            {children}
        </ScheduleContext.Provider>
    );
};

export {ScheduleContextProvider, ScheduleContext};

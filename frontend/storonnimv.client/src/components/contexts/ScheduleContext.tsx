import React, {createContext, ReactNode, useCallback, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IScheduleListItem} from "../../models/schedule/IScheduleListItem";
import {ISchedule} from "../../models/schedule/ISchedule.ts";
import {IPaginationResponse} from "../../models/shared/IPaginationResponse.ts";

// Тип контекста
interface ScheduleContextType {
    schedules: IScheduleListItem[];
    schedulesStatus: RequestStatus;
    fetchSchedules: (pageNumber: number, pageSize: number) => Promise<void>;
    scheduleFullInfo: ISchedule | null;
    scheduleFullInfoStatus: RequestStatus;
    fetchScheduleFullInfo: (scheduleId: number) => Promise<void>;
    currentPage: number;
    totalPages: number;
    paginate: (pageNumber: number, pageSize?: number) => void;
}

type RequestStatus = "loading" | "success" | "empty" | "error";

// Создаем контекст с типизацией
const ScheduleContext = createContext<ScheduleContextType | undefined>(undefined);

interface ScheduleContextProviderProps {
    children: ReactNode;
}

const ScheduleContextProvider: React.FC<ScheduleContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setPageLoading, setModalLoading, serverRoute} = globalContext;

    const [schedules, setSchedules] = useState<IScheduleListItem[]>([]);
    const [schedulesStatus, setSchedulesStatus] = useState<RequestStatus>("loading");
    const [scheduleFullInfo, setScheduleFullInfo] = useState<ISchedule | null>(null);
    const [scheduleFullInfoStatus, setScheduleFullInfoStatus] = useState<RequestStatus>("loading");

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(1);

    const fetchScheduleFullInfo = useCallback(async (scheduleId: number): Promise<void> => {
        try {
            setModalLoading(true);
            setScheduleFullInfo(null);
            setScheduleFullInfoStatus("loading");
            const response = await sendRequest(`${serverRoute}/schedules/${scheduleId}`);
            if (response.status === 404) {
                setScheduleFullInfoStatus("empty");
                return;
            }
            if (response.status !== 200) {
                throw new Error(`Schedule detail request failed with status ${response.status}`);
            }

            const data: ISchedule | null = response.data;
            if (!data) {
                setScheduleFullInfoStatus("empty");
                return;
            }

            setScheduleFullInfo(data);
            setScheduleFullInfoStatus("success");
        } catch (error) {
            setScheduleFullInfo(null);
            setScheduleFullInfoStatus("error");
            console.error("Error fetching schedule full info:", error);
        } finally {
            setModalLoading(false);
        }
    }, [sendRequest, serverRoute, setModalLoading]);

    const fetchSchedules = useCallback(
        async (pageNumber: number = 1, pageSize: number = 3): Promise<void> => {
            try {
                setPageLoading(true);
                setSchedules([]);
                setSchedulesStatus("loading");
                const response = await sendRequest(
                    `${serverRoute}/schedules/page/${pageNumber}?pageSize=${pageSize}`
                );
                if (response.status !== 200) {
                    throw new Error(`Schedule list request failed with status ${response.status}`);
                }

                const data: IPaginationResponse<IScheduleListItem> = response.data;
                if (!Array.isArray(data.items)) {
                    throw new Error("Schedule list response is invalid");
                }

                setSchedules(data.items);
                setCurrentPage(data.currentPage);
                setTotalPages(data.totalPages);
                setSchedulesStatus(data.items.length === 0 ? "empty" : "success");
            } catch (error) {
                setSchedules([]);
                setSchedulesStatus("error");
                console.error("Error while fetching schedules: ", error);
            } finally {
                setPageLoading(false);
            }
        }, [sendRequest, serverRoute, setPageLoading]);

    const paginate = useCallback(async (pageNumber: number, pageSize: number = 3): Promise<void> => {
        if (pageNumber >= 1) {
            await fetchSchedules(pageNumber, pageSize);
        }
    }, [fetchSchedules]);

    const value: ScheduleContextType = {
        fetchScheduleFullInfo,
        scheduleFullInfo,
        scheduleFullInfoStatus,
        schedules,
        schedulesStatus,
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

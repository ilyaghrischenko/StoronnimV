import React, {createContext, ReactNode, useCallback, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IHomeNewsItem} from "../../models/home/IHomeNewsItem";
import {IVideoModel} from "../../models/video/IVideoModel";
import {IScheduleHomeItem} from "../../models/home/IScheduleHomeItem";

// Тип контекста
interface HomeContextType {
    homeSchedule: IScheduleHomeItem | null;
    homeScheduleStatus: RequestStatus;
    fetchHomeSchedule: () => Promise<void>;
    homeNewsList: IHomeNewsItem[];
    homeNewsStatus: RequestStatus;
    fetchHomeNewsList: () => Promise<void>;
    homePromotionVideo: IVideoModel | null;
    homePromotionVideoStatus: RequestStatus;
    fetchHomePromotionVideo: () => Promise<void>;
}

type RequestStatus = "loading" | "success" | "empty" | "error";

// Создаем контекст с типизацией
const HomeContext = createContext<HomeContextType | undefined>(undefined);

interface HomeContextProviderProps {
    children: ReactNode;
}

const HomeContextProvider: React.FC<HomeContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, serverRoute} = globalContext;

    const [homeSchedule, setHomeSchedule] = useState<IScheduleHomeItem | null>(null);
    const [homeScheduleStatus, setHomeScheduleStatus] = useState<RequestStatus>("loading");

    const fetchHomeSchedule = useCallback(async (): Promise<void> => {
        setHomeScheduleStatus("loading");
        try {
            const response = await sendRequest(`${serverRoute}/home/schedule`);
            if (response.status !== 200) {
                throw new Error(`Home schedule request failed with status ${response.status}`);
            }

            const data: IScheduleHomeItem | null = response.data;
            if (!data) {
                setHomeSchedule(null);
                setHomeScheduleStatus("empty");
                return;
            }

            setHomeSchedule(data);
            setHomeScheduleStatus("success");
        } catch (error) {
            setHomeSchedule(null);
            setHomeScheduleStatus("error");
            console.error("Error while fetching schedule for home: ", error);
        }
    }, [sendRequest, serverRoute]);

    const [homeNewsList, setHomeNewsList] = useState<IHomeNewsItem[]>([]);
    const [homeNewsStatus, setHomeNewsStatus] = useState<RequestStatus>("loading");

    const fetchHomeNewsList = useCallback(async (): Promise<void> => {
        setHomeNewsStatus("loading");
        try {
            const response = await sendRequest(`${serverRoute}/home/news/6`);
            if (response.status !== 200) {
                throw new Error(`Home news request failed with status ${response.status}`);
            }

            const data: IHomeNewsItem[] = response.data;

            setHomeNewsList(data);
            setHomeNewsStatus(data.length === 0 ? "empty" : "success");
        } catch (error) {
            setHomeNewsList([]);
            setHomeNewsStatus("error");
            console.error("Error while fetching news for home: ", error);
        }
    }, [sendRequest, serverRoute]);

    const [homePromotionVideo, setHomePromotionVideo] = useState<IVideoModel | null>(null);
    const [homePromotionVideoStatus, setHomePromotionVideoStatus] = useState<RequestStatus>("loading");

    const fetchHomePromotionVideo = useCallback(async (): Promise<void> => {
        setHomePromotionVideoStatus("loading");
        try {
            const response = await sendRequest(`${serverRoute}/home/video`);
            if (response.status !== 200) {
                throw new Error(`Home video request failed with status ${response.status}`);
            }

            const data: IVideoModel | null = response.data;
            if (!data?.url) {
                setHomePromotionVideo(null);
                setHomePromotionVideoStatus("empty");
                return;
            }

            setHomePromotionVideo(data);
            setHomePromotionVideoStatus("success");
        } catch (error) {
            setHomePromotionVideo(null);
            setHomePromotionVideoStatus("error");
            console.error("Error while fetching video for home: ", error);
        }
    }, [sendRequest, serverRoute]);

    const value: HomeContextType = {
        homeSchedule,
        homeScheduleStatus,
        fetchHomeSchedule,
        homeNewsList,
        homeNewsStatus,
        fetchHomeNewsList,
        homePromotionVideo,
        homePromotionVideoStatus,
        fetchHomePromotionVideo
    };

    return (
        <HomeContext.Provider value={value}>
            {children}
        </HomeContext.Provider>
    );
};

export {HomeContextProvider, HomeContext};

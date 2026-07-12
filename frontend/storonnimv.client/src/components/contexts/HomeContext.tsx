import React, {createContext, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IHomeNewsItem} from "../../models/home/IHomeNewsItem";
import {IVideoModel} from "../../models/video/IVideoModel";
import {IScheduleHomeItem} from "../../models/home/IScheduleHomeItem";
import {useNavigate} from "react-router-dom";

// Тип контекста
interface HomeContextType {
    homeSchedule: IScheduleHomeItem;
    fetchHomeSchedule: () => Promise<void>;
    homeNewsList: IHomeNewsItem[];
    fetchHomeNewsList: () => Promise<void>;
    onClickHomeElementHandler: (section: string) => void;
    homePromotionVideo: IVideoModel;
    fetchHomePromotionVideo: () => Promise<void>;
}

// Создаем контекст с типизацией
const HomeContext = createContext<HomeContextType | undefined>(undefined);

interface HomeContextProviderProps {
    children: ReactNode;
}

const HomeContextProvider: React.FC<HomeContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, serverRoute} = globalContext;

    const navigate = useNavigate();

    const [homeSchedule, setHomeSchedule] = useState<IScheduleHomeItem>({
        id: 1,
        photo: '',
        title: '',
        performanceDateTime: '',
        location: ''
    });

    const fetchHomeSchedule = async (): Promise<void> => {
        try {
            const response = await sendRequest(`${serverRoute}/home/schedule`);

            const data: IScheduleHomeItem = response.data;

            setHomeSchedule(data);
        } catch (error) {
            console.error("Error while fetching schedule for home: ", error);
        }
    };

    const [homeNewsList, setHomeNewsList] = useState<IHomeNewsItem[]>([]);

    const fetchHomeNewsList = async (): Promise<void> => {
        try {
            const response = await sendRequest(`${serverRoute}/home/news/6`);

            const data: IHomeNewsItem[] = response.data;

            setHomeNewsList(data);
        } catch (error) {
            console.error("Error while fetching news for home: ", error);
        }
    };

    const [homePromotionVideo, setHomePromotionVideo] = useState<IVideoModel>({} as IVideoModel);

    const fetchHomePromotionVideo = async (): Promise<void> => {
        try {
            const response = await sendRequest(`${serverRoute}/home/video`);

            const data: IVideoModel = response.data;

            setHomePromotionVideo(data);
        } catch (error) {
            console.error("Error while fetching video for home: ", error);
        }
    };

    const onClickHomeElementHandler = (section: string) => {
        navigate(`/${section}`, {replace: true});
    };

    const value: HomeContextType = {
        homeSchedule,
        fetchHomeSchedule,
        homeNewsList,
        fetchHomeNewsList,
        onClickHomeElementHandler,
        homePromotionVideo,
        fetchHomePromotionVideo
    };

    return (
        <HomeContext.Provider value={value}>
            {children}
        </HomeContext.Provider>
    );
};

export {HomeContextProvider, HomeContext};

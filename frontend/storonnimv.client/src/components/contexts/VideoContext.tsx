import React, {createContext, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IVideoModel} from "../../models/video/IVideoModel";
import {IPaginationResponse} from "../../models/shared/IPaginationResponse";


// Тип контекста
interface VideoContextType {
    videoList: IVideoModel[];
    currentPage: number;
    totalPages: number;
    fetchVideos: (videoType: string, pageNumber: number, pageSize?: number) => Promise<void>;
    paginate: (videoType: string, pageNumber?: number, pageSize?: number) => Promise<void>;
}

// Создаем контекст с типизацией
const VideoContext = createContext<VideoContextType | undefined>(undefined);

interface VideoContextProviderProps {
    children: ReactNode;
}

const VideoContextProvider: React.FC<VideoContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {sendRequest, setPageLoading, serverRoute} = globalContext;

    const [videoList, setVideoList] = useState<IVideoModel[]>([]);
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(1);

    const fetchVideos =
        async (videoType: string, pageNumber: number, pageSize: number = 2): Promise<void> => {
            try {
                setPageLoading(true);
                const response = await sendRequest(
                    `${serverRoute}/videos/page/${videoType}/${pageNumber}?pageSize=${pageSize}`
                );

                const data: IPaginationResponse<IVideoModel> = response.data;

                // Проверяем, если данные пустые
                if (!data.items || data.items.length === 0) {
                    console.warn(`No videos found for category: ${videoType}`);
                    setVideoList([]);  // Обнуляем список
                    sessionStorage.setItem("videoTotalPages", "0"); // Сбрасываем totalPages
                    setPageLoading(false);
                    return;
                }

                setVideoList(data.items);
                setCurrentPage(data.currentPage);
                setTotalPages(data.totalPages);

                sessionStorage.setItem("videoCurrentPage", String(data.currentPage));
                sessionStorage.setItem("videoTotalPages", String(data.totalPages));
                sessionStorage.setItem("currentVideoType", videoType);
                setPageLoading(false);
            } catch (error) {
                console.error("Error while fetching videos: ", error);
                setVideoList([]); // Обнуляем список в случае ошибки
                sessionStorage.setItem("videoTotalPages", "0"); // Сбрасываем totalPages
                setPageLoading(false);
            }
        };


    const paginate =
        async (videoType: string, pageNumber: number = currentPage, pageSize: number = 5): Promise<void> => {

            const savedTotalPages = Number(sessionStorage.getItem("videoTotalPages")) || 0;
            const savedVideoType = sessionStorage.getItem("currentVideoType");

            if (savedVideoType !== videoType) {
                // Сбрасываем текущую страницу на 1, если тип видео поменялся
                setCurrentPage(1);
                setTotalPages(0); // Также сбрасываем количество страниц
                await fetchVideos(videoType, 1, pageSize); // Загружаем видео для первого типа
                return;
            }

            // Если videoList пуст, сбрасываем сохранённые totalPages и загружаем видео заново
            if (savedTotalPages === 0 || videoList.length === 0) {
                await fetchVideos(videoType, pageNumber, pageSize);
                return;
            }

            if (pageNumber >= 1 && pageNumber <= savedTotalPages) {
                await fetchVideos(videoType, pageNumber, pageSize);
            }
        };


    const value: VideoContextType = {
        videoList,
        currentPage,
        totalPages,
        fetchVideos,
        paginate,
    };

    return (
        <VideoContext.Provider value={value}>
            {children}
        </VideoContext.Provider>
    );
};

export {VideoContextProvider, VideoContext};

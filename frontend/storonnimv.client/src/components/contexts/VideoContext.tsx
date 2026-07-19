import React, {createContext, ReactNode, useCallback, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IVideoModel, VideoCategory} from "../../models/video/IVideoModel";
import {IPaginationResponse} from "../../models/shared/IPaginationResponse";

type VideoListStatus = "idle" | "loading" | "success" | "empty" | "error";

interface VideoContextType {
    videoList: IVideoModel[];
    currentPage: number;
    totalPages: number;
    videoListStatus: VideoListStatus;
    fetchVideos: (videoType: VideoCategory, pageNumber: number, pageSize?: number) => Promise<void>;
    paginate: (videoType: VideoCategory, pageNumber?: number, pageSize?: number) => Promise<void>;
}

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
    const [totalPages, setTotalPages] = useState<number>(0);
    const [videoListStatus, setVideoListStatus] = useState<VideoListStatus>("idle");

    const fetchVideos = useCallback(
        async (videoType: VideoCategory, pageNumber: number, pageSize: number = 2): Promise<void> => {
            try {
                setPageLoading(true);
                setVideoListStatus("loading");
                const response = await sendRequest(
                    `${serverRoute}/videos/page/${videoType}/${pageNumber}?pageSize=${pageSize}`
                );

                if (response.status !== 200) {
                    throw new Error(`Video request failed with status ${response.status}`);
                }

                const data: IPaginationResponse<IVideoModel> = response.data;
                setCurrentPage(data.currentPage);
                setTotalPages(data.totalPages);

                if (!data.items || data.items.length === 0) {
                    setVideoList([]);
                    setVideoListStatus("empty");
                    return;
                }

                setVideoList(data.items);
                setVideoListStatus("success");
            } catch (error) {
                console.error("Error while fetching videos: ", error);
                setVideoList([]);
                setTotalPages(0);
                setVideoListStatus("error");
            } finally {
                setPageLoading(false);
            }
        },
        [sendRequest, serverRoute, setPageLoading]
    );

    const paginate = useCallback(
        async (videoType: VideoCategory, pageNumber: number = 1, pageSize: number = 2): Promise<void> => {
            if (pageNumber >= 1) {
                await fetchVideos(videoType, pageNumber, pageSize);
            }
        },
        [fetchVideos]
    );

    const value: VideoContextType = {
        videoList,
        currentPage,
        totalPages,
        videoListStatus,
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

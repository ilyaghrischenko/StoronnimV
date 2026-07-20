import React, {createContext, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IMusicPlatformItem} from "../../models/music/IMusicPlatformItem";
import {useCallback} from "react";

// Тип контекста
interface MusicContextType {
    musicPlatforms: IMusicPlatformItem[];
    musicStatus: RequestStatus;
    fetchMusicPlatforms: () => Promise<void>;
}

type RequestStatus = "loading" | "success" | "empty" | "error";

// Создаем контекст с типизацией
const MusicContext = createContext<MusicContextType | undefined>(undefined);

interface MusicContextProviderProps {
    children: ReactNode;
}

const MusicContextProvider: React.FC<MusicContextProviderProps> = ({ children }) => {
    const globalContext = useContext(GlobalContext)!;

    const { sendRequest, setPageLoading, serverRoute } = globalContext;

    const [musicPlatforms, setMusicPlatforms] = useState<IMusicPlatformItem[]>([]);
    const [musicStatus, setMusicStatus] = useState<RequestStatus>("loading");

    const fetchMusicPlatforms = useCallback(async () : Promise<void> => {
        setPageLoading(true);
        setMusicPlatforms([]);
        setMusicStatus("loading");
        try {
            const response = await sendRequest(`${serverRoute}/music`);
            if (response.status !== 200) {
                throw new Error(`Music request failed with status ${response.status}`);
            }

            const data: unknown = response.data;
            if (!Array.isArray(data)) {
                throw new Error("Music response is invalid");
            }

            setMusicPlatforms(data);
            setMusicStatus(data.length === 0 ? "empty" : "success");
        } catch (error) {
            setMusicPlatforms([]);
            setMusicStatus("error");
            console.error('Error fetching music platforms', error);
        }
        finally {
            setPageLoading(false);
        }
    }, [sendRequest, serverRoute, setPageLoading]);

    const value: MusicContextType = {
        musicPlatforms,
        musicStatus,
        fetchMusicPlatforms
    };

    return (
        <MusicContext.Provider value={value}>
            {children}
        </MusicContext.Provider>
    );
};

export { MusicContextProvider, MusicContext };

import React, {createContext, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IMusicPlatformItem} from "../../models/music/IMusicPlatformItem";
import {useCallback} from "react";

// Тип контекста
interface MusicContextType {
    musicPlatforms: IMusicPlatformItem[];
    fetchMusicPlatforms: () => Promise<void>;
}

// Создаем контекст с типизацией
const MusicContext = createContext<MusicContextType | undefined>(undefined);

interface MusicContextProviderProps {
    children: ReactNode;
}

const MusicContextProvider: React.FC<MusicContextProviderProps> = ({ children }) => {
    const globalContext = useContext(GlobalContext)!;

    const { sendRequest, setPageLoading, serverRoute } = globalContext;

    const [musicPlatforms, setMusicPlatforms] = useState<IMusicPlatformItem[]>([]);

    const fetchMusicPlatforms = useCallback(async () : Promise<void> => {
        try {
            setPageLoading(true);
            const response = await sendRequest(`${serverRoute}/music`);

            const data: IMusicPlatformItem[] = response.data;

            setMusicPlatforms(data);
        } catch (error) {
            console.error('Error fetching music platforms', error);
        }
        finally {
            setPageLoading(false);
        }
    }, [sendRequest, serverRoute, setPageLoading]);

    const value: MusicContextType = {
        musicPlatforms,
        fetchMusicPlatforms
    };

    return (
        <MusicContext.Provider value={value}>
            {children}
        </MusicContext.Provider>
    );
};

export { MusicContextProvider, MusicContext };

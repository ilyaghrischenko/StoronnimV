import React, {createContext} from "react";

const MusicContext = createContext();

const MusicContextProvider = ({children}) => {
    const value = {
        
    };
    
    return (
        <MusicContext.Provider value={value}>
            {children}
        </MusicContext.Provider>
    );
};

export {MusicContextProvider, MusicContext};
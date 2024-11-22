import React, {createContext} from "react";

const HomeContext = createContext();

const HomeContextProvider = ({children}) => {
    const value = {
        
    };
    
    return (
        <HomeContext.Provider value={value}>
            {children}
        </HomeContext.Provider>
    );
};

export { HomeContextProvider, HomeContext };
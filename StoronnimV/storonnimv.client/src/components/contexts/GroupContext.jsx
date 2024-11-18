import React, {createContext} from "react";

const GroupContext = createContext();

const GroupContextProvider = ({ children }) => {
    const value = {
        
    };
    
    return (
        <GroupContext.Provider value={value}>
            {children}
        </GroupContext.Provider>
    );
}

export {GroupContext, GroupContextProvider};
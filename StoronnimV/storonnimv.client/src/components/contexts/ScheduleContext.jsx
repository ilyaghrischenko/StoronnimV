import React, {createContext} from "react";

const ScheduleContext = createContext();

const ScheduleContextProvider = ({children}) => {
    const value = {
        
    };
    
    return (
        <ScheduleContext.Provider value={value}>
            {children}
        </ScheduleContext.Provider>  
    );
}

export {ScheduleContextProvider, ScheduleContext};
import React, {createContext} from "react";

const Context = createContext();

const ContextProvider = ({ children }) => {
    const value = {
        
    };
    
    return (
        <Context.Provider value={value}>
            {children}
        </Context.Provider>
    );
};

export { ContextProvider };
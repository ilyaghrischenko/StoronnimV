import {createContext} from "react";

const NewsContext = createContext();

const NewsContextProvider = ({children}) => {
    const value = {
        
    };
    
    return (
        <NewsContext.Provider value={value}>
            {children}
        </NewsContext.Provider>
    );
};

export {NewsContext, NewsContextProvider};
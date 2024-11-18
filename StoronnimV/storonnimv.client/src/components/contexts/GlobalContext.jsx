import React, {createContext} from "react";
import axios from "axios";

const GlobalContext = createContext();

const GlobalContextProvider = ({ children }) => {
    async function sendRequest(apiUrl, method = 'GET', body = null, headers = {}) {
        try {
            const config = {
                method,
                apiUrl,
                headers
            };
            
            if (body) {
                config.data = body;
            }
            
            const response = await axios(config);
            return response.data;
        }   
        catch(err) {
            console.error('HTTP Request failed: ', err.message);
        }
    }
    
    const value = {
        sendRequest
    };
    
    return (
        <GlobalContext.Provider value={value}>
            {children}
        </GlobalContext.Provider>
    );
};

export { GlobalContextProvider, GlobalContext };
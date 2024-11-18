import React, {createContext} from "react";

const ContactsContext = createContext();

const ContactsContextProvider = ({children}) => {
    const value = {
        
    };
    
    return (
        <ContactsContext.Provider value={value}>
            {children}
        </ContactsContext.Provider>
    );
};

export {ContactsContext, ContactsContextProvider};
import React, {ReactNode, useContext, useEffect} from "react";
import { Navigate } from "react-router-dom";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

interface ProtectedRouteProps {
    children: ReactNode;  
    requiredRole: string; 
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, requiredRole }) => {
    const globalContext = useContext(GlobalContext);

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const { fetchIsAdmin } = globalContext;

    useEffect(() => {
        fetchIsAdmin();
    }, []);

    //TODO: решить
    // if (!isAdmin) {
    //     return <Navigate to="/error?statusCode=401&message=Unauthorised" replace />;
    // }

    const adminRole = sessionStorage.getItem("role");

    if (!adminRole) {
        return <Navigate to="/error?statusCode=401&message=Unauthorised" replace />;
    }

    if (adminRole !== requiredRole) {
        return <Navigate to="/error?statusCode=403&message=Forbidden" replace />;
    }

    return <>{children}</>;
};

export { ProtectedRoute };

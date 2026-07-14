import React, {ReactNode, useContext, useEffect, useState} from "react";
import { Navigate } from "react-router-dom";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";

interface ProtectedRouteProps {
    children: ReactNode;  
    requiredRole: string; 
}

type AuthorizationState = "loading" | "authorized" | "unauthorized" | "forbidden";

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, requiredRole }) => {
    const globalContext = useContext(GlobalContext);
    const [authorizationState, setAuthorizationState] = useState<AuthorizationState>("loading");

    if (!globalContext) {
        throw new Error("GlobalContext must be used within a GlobalContextProvider");
    }

    const {sendRequest, serverRoute} = globalContext;

    useEffect(() => {
        let isCurrent = true;

        const validateRole = async () => {
            setAuthorizationState("loading");

            try {
                const response = await sendRequest(`${serverRoute}/admin/role`);

                if (!isCurrent) {
                    return;
                }

                if (response.status === 401) {
                    setAuthorizationState("unauthorized");
                } else if (response.status === 200 && response.data === requiredRole) {
                    setAuthorizationState("authorized");
                } else {
                    setAuthorizationState("forbidden");
                }
            } catch (error) {
                if (isCurrent) {
                    setAuthorizationState("forbidden");
                }

                console.error("Error while validating admin role", error);
            }
        };

        void validateRole();

        return () => {
            isCurrent = false;
        };
    }, [requiredRole, sendRequest, serverRoute]);

    if (authorizationState === "loading") {
        return <div role="status" aria-live="polite">Перевірка доступу...</div>;
    }

    if (authorizationState === "unauthorized") {
        return <Navigate to="/error?statusCode=401&message=Unauthorised" replace />;
    }

    if (authorizationState === "forbidden") {
        return <Navigate to="/error?statusCode=403&message=Forbidden" replace />;
    }

    return <>{children}</>;
};

export { ProtectedRoute };

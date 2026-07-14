import {createContext, FC, ReactNode, useCallback, useEffect, useState} from "react";
import axios, {AxiosError, AxiosResponse} from "axios";

// Определяем интерфейс для значения контекста
interface GlobalContextType {
    sendRequest: (
        apiUrl: string,
        method?: string,
        body?: unknown,
        headers?: Record<string, string>
    ) => Promise<AxiosResponse>;
    pageLoading: boolean,
    setPageLoading: (pageLoading: boolean) => void;
    modalLoading: boolean,
    setModalLoading: (modalLoading: boolean) => void;
    showModal: boolean,
    OnShowModal: (mContent: ReactNode, mTitle?: string) => void;
    OnHideModal: () => void;
    modalContent: ReactNode;
    modalTitle: string;
    isAdminRoute: () => boolean;
    isAdmin: boolean;
    setIsAdmin: (isAdmin: boolean) => void;
    fetchIsAdmin: () => Promise<void>;
    validationErrors: Record<string, string[]>;
    setValidationErrors: (validationErrors: Record<string, string[]>) => void;
    checkIfNoData: (callback: () => boolean) => boolean;
    serverRoute: string;
}

// Создаем контекст с типизацией
const GlobalContext = createContext<GlobalContextType | undefined>(undefined);

interface GlobalContextProviderProps {
    children: ReactNode;
}

const GlobalContextProvider: FC<GlobalContextProviderProps> = ({children}) => {
    const [showModal, setShowModal] = useState<boolean>(false);
    const [modalContent, setModalContent] = useState<ReactNode>(null);
    const [modalTitle, setModalTitle] = useState<string>("");

    const [isAdmin, setIsAdmin] = useState<boolean>(false);

    const [validationErrors, setValidationErrors] = useState<Record<string, string[]>>({} as Record<string, string[]>);

    const serverRoute = import.meta.env.VITE_API_URL;

    const sendRequest = useCallback(async (
        apiUrl: string,
        method: string = "GET",
        body: unknown = null,
        headers: Record<string, string> = {}
    ): Promise<AxiosResponse> => {
        try {
            const normalizedMethod = method.toUpperCase();
            const isUnsafeMethod = !["GET", "HEAD", "OPTIONS", "TRACE"].includes(normalizedMethod);
            let requestHeaders = headers;

            if (isUnsafeMethod) {
                const tokenResponse = await axios.get<{requestToken: string}>(
                    `${serverRoute}/account/csrf-token`,
                    {withCredentials: true}
                );
                requestHeaders = {
                    ...headers,
                    "X-CSRF-TOKEN": tokenResponse.data.requestToken
                };
            }

            const config = {
                method: normalizedMethod,
                url: apiUrl,
                headers: requestHeaders,
                data: body,
                withCredentials: true
            };

            return await axios(config);
        } catch (err: unknown) {
            const error = err as AxiosError;

            if (error.response?.status === 429) {
                alert('Дуже багато запитів на сервер за короткий термін. Спробуйте пізніше.');
            }

            if (error.response) {
                return error.response;
            } else {
                throw new Error(error.message || "Network error");
            }
        }
    }, [serverRoute]);

    const fetchIsAdmin = useCallback(async () => {
        try {
            const response = await sendRequest(`${serverRoute}/admin/isAdmin`);

            if (response.status === 200) {
                setIsAdmin(true);
            } else {
                setIsAdmin(false);
                sessionStorage.removeItem('role');
            }
        } catch (error) {
            setIsAdmin(false);
            console.error('Error while checking admin session', error);
        }
    }, [sendRequest, serverRoute]);

    useEffect(() => {
        void fetchIsAdmin();
    }, [fetchIsAdmin]);

    const OnShowModal = (mContent: ReactNode, mTitle: string = "") => {
        setModalTitle(mTitle);
        setModalContent(mContent);
        setShowModal(true);
    };

    const OnHideModal = () => {
        setModalContent(null);
        setShowModal(false);
    };

    const [pageLoading, setPageLoading] = useState<boolean>(false);
    const [modalLoading, setModalLoading] = useState<boolean>(false);

    const isAdminRoute = (): boolean => {
        return window.location.pathname.startsWith("/admin");
    };

    const checkIfNoData = (callback: () => boolean) => {
        const isEmpty = callback();
        return isEmpty && !pageLoading;
    };

    // Значение контекста
    const value: GlobalContextType = {
        modalTitle,
        modalContent,
        showModal,
        OnShowModal,
        OnHideModal,
        sendRequest,
        pageLoading,
        setPageLoading,
        modalLoading,
        setModalLoading,
        isAdminRoute,
        isAdmin,
        setIsAdmin,
        fetchIsAdmin,
        validationErrors,
        setValidationErrors,
        checkIfNoData,
        serverRoute
    };

    return (
        <GlobalContext.Provider value={value}>
            {children}
        </GlobalContext.Provider>
    );
};

export {GlobalContextProvider, GlobalContext};

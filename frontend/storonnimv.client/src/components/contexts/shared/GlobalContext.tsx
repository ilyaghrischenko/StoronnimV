import {createContext, FC, ReactNode, useState} from "react";
import axios, {AxiosError, AxiosResponse} from "axios";

// Определяем интерфейс для значения контекста
interface GlobalContextType {
    sendRequest: (
        apiUrl: string,
        method?: string,
        body?: any,
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

    const fetchIsAdmin = async () => {
        try {
            const response = await sendRequest(`${serverRoute}/admin/isAdmin`);

            if (response.status === 200) {
                setIsAdmin(true);
            } else {
                setIsAdmin(false);
            }
        } catch (error) {
            console.error('Error fetching SoundCloud embed data', error);
        }
    };

    const OnShowModal = (mContent: ReactNode, mTitle: string = "") => {
        setModalTitle(mTitle);
        setModalContent(mContent);
        setShowModal(true);
    };

    const OnHideModal = () => {
        setModalContent(null);
        setShowModal(false);
    };

    // Асинхронная функция для отправки запросов
    async function sendRequest(
        apiUrl: string,
        method: string = "GET",
        body: unknown = null,
        headers: Record<string, string> = {}
    ): Promise<AxiosResponse> {
        try {
            const config = {
                method,
                url: apiUrl,
                headers,
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
    }

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

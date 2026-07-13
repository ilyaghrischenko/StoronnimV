import {INewsShortItem} from "../../models/news/INewsShortItem";
import {createContext, FC, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IPaginationResponse} from "../../models/shared/IPaginationResponse";
import {INewsFullItem} from "../../models/news/INewsFullItem.ts";

interface NewsContextType {
    newsList: INewsShortItem[];
    newsStatus: RequestStatus;
    currentPage: number;
    totalPages: number;
    fetchNews: (pageNumber?: number, pageSize?: number) => Promise<void>;
    paginate: (pageNumber: number, pageSize?: number) => void;
    newsFullItem: INewsFullItem | null;
    newsFullItemStatus: RequestStatus;
    fetchNewsFullItem: (id: number) => Promise<void>;
}

type RequestStatus = "loading" | "success" | "empty" | "error";

const NewsContext = createContext<NewsContextType | undefined>(undefined);

interface NewsContextProviderProps {
    children: ReactNode;
}

const NewsContextProvider: FC<NewsContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setPageLoading, setModalLoading, serverRoute} = globalContext;

    const [newsList, setNewsList] = useState<INewsShortItem[]>([]);
    const [newsStatus, setNewsStatus] = useState<RequestStatus>("loading");
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(1);
    const [newsFullItem, setNewsFullItem] = useState<INewsFullItem | null>(null);
    const [newsFullItemStatus, setNewsFullItemStatus] = useState<RequestStatus>("loading");

    const fetchNewsFullItem = async (id: number): Promise<void> => {
        try {
            setModalLoading(true);
            setNewsFullItem(null);
            setNewsFullItemStatus("loading");
            const response = await sendRequest(
                `${serverRoute}/news/${id}`
            );
            if (response.status === 404) {
                setNewsFullItemStatus("empty");
                return;
            }
            if (response.status !== 200) {
                throw new Error(`News detail request failed with status ${response.status}`);
            }

            const data: INewsFullItem | null = response.data;
            if (!data) {
                setNewsFullItemStatus("empty");
                return;
            }

            setNewsFullItem(data);
            setNewsFullItemStatus("success");
        } catch (error) {
            setNewsFullItem(null);
            setNewsFullItemStatus("error");
            console.error("Error fetching news full item ", error);
        }
        finally {
            setModalLoading(false);
        }
    };

    const fetchNews = async (pageNumber: number = currentPage, pageSize: number = 6): Promise<void> => {
        try {
            setPageLoading(true);
            setNewsList([]);
            setNewsStatus("loading");
            const response = await sendRequest(
                `${serverRoute}/news/page/${pageNumber}?pageSize=${pageSize}`
            );
            if (response.status !== 200) {
                throw new Error(`News list request failed with status ${response.status}`);
            }

            const data: IPaginationResponse<INewsShortItem> = response.data;
            if (!Array.isArray(data.items)) {
                throw new Error("News list response is invalid");
            }

            setNewsList(data.items);
            setCurrentPage(data.currentPage);
            setTotalPages(data.totalPages);
            setNewsStatus(data.items.length === 0 ? "empty" : "success");

            sessionStorage.setItem("newsCurrentPage", String(data.currentPage));
            sessionStorage.setItem("newsTotalPages", String(data.totalPages));
        } catch (error) {
            setNewsList([]);
            setNewsStatus("error");
            console.error("Error while fetching news: ", error);
        }
        finally {
            setPageLoading(false);
        }
    };

    const paginate =
        async (pageNumber: number, pageSize: number = 6): Promise<void> => {

            const savedTotalPagesString = sessionStorage.getItem("newsTotalPages");
            const savedTotalPages = savedTotalPagesString ? Number(savedTotalPagesString) : 0;

            if (savedTotalPages === 0) {
                await fetchNews(pageNumber, pageSize);
            }

            if (pageNumber >= 1 && pageNumber <= savedTotalPages) {
                await fetchNews(pageNumber, pageSize);
            }
        };

    const value: NewsContextType = {
        newsFullItem,
        newsFullItemStatus,
        fetchNewsFullItem,
        newsList,
        newsStatus,
        currentPage,
        totalPages,
        fetchNews,
        paginate,
    };

    return <NewsContext.Provider value={value}>{children}</NewsContext.Provider>;
};

export {NewsContext, NewsContextProvider};

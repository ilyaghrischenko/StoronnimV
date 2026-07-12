import {INewsShortItem} from "../../models/news/INewsShortItem";
import {createContext, FC, ReactNode, useContext, useState} from "react";
import {GlobalContext} from "./shared/GlobalContext";
import {IPaginationResponse} from "../../models/shared/IPaginationResponse";
import {INewsFullItem} from "../../models/news/INewsFullItem.ts";

interface NewsContextType {
    newsList: INewsShortItem[];
    currentPage: number;
    totalPages: number;
    fetchNews: (pageNumber?: number, pageSize?: number) => Promise<void>;
    paginate: (pageNumber: number, pageSize?: number) => void;
    newsFullItem: INewsFullItem;
    fetchNewsFullItem: (id: number) => Promise<void>;
}

const NewsContext = createContext<NewsContextType | undefined>(undefined);

interface NewsContextProviderProps {
    children: ReactNode;
}

const NewsContextProvider: FC<NewsContextProviderProps> = ({children}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, setPageLoading, setModalLoading, serverRoute} = globalContext;

    const [newsList, setNewsList] = useState<INewsShortItem[]>([]);
    const [currentPage, setCurrentPage] = useState<number>(1);
    const [totalPages, setTotalPages] = useState<number>(1);
    const [newsFullItem, setNewsFullItem] = useState<INewsFullItem>({} as INewsFullItem);

    const fetchNewsFullItem = async (id: number): Promise<void> => {
        try {
            setModalLoading(true);
            const response = await sendRequest(
                `${serverRoute}/news/${id}`
            );

            const data: INewsFullItem = response.data;
            setNewsFullItem(data);
        } catch (error) {
            console.error("Error fetching news full item ", error);
        }
        finally {
            setModalLoading(false);
        }
    };

    const fetchNews = async (pageNumber: number = currentPage, pageSize: number = 6): Promise<void> => {
        try {
            setPageLoading(true);
            const response = await sendRequest(
                `${serverRoute}/news/page/${pageNumber}?pageSize=${pageSize}`
            );

            const data: IPaginationResponse<INewsShortItem> = response.data;

            setNewsList(data.items);
            setCurrentPage(data.currentPage);
            setTotalPages(data.totalPages);

            sessionStorage.setItem("newsCurrentPage", String(data.currentPage));
            sessionStorage.setItem("newsTotalPages", String(data.totalPages));
        } catch (error) {
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
        fetchNewsFullItem,
        newsList,
        currentPage,
        totalPages,
        fetchNews,
        paginate,
    };

    return <NewsContext.Provider value={value}>{children}</NewsContext.Provider>;
};

export {NewsContext, NewsContextProvider};

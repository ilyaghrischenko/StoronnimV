import {FC, useContext, useEffect} from "react";
import {NewsContext, NewsContextProvider} from "../../contexts/NewsContext";
import {Button, Container} from "react-bootstrap";
import {NewsListItem} from "./NewsListItem";

import {List} from "../shared/GenericList/List";
import {ListItem} from "../shared/GenericList/ListItem";
import {INewsShortItem} from "../../../models/news/INewsShortItem";
import {GlobalContext} from "../../contexts/shared/GlobalContext";
import {NewsModal} from "./NewsModal.tsx";
import {PaginationSection} from "../shared/PaginationSection.tsx";
import {AddNewsItemModal} from "./forms/AddNewsItemModal.tsx";
import {FaPlus} from "react-icons/fa";
import PreloaderTile from "../shared/PreloaderTile.tsx";
import {NoData} from "../shared/NoData.tsx";

const NewsList: FC = () => {
    const newsContext = useContext(NewsContext)!;
    const globalContext = useContext(GlobalContext)!;

    const {OnShowModal, isAdmin} = globalContext;

    const {newsList, newsStatus, currentPage, totalPages, paginate} = newsContext;

    useEffect(() => {
        const savedPage = sessionStorage.getItem("newsCurrentPage");
        const page = savedPage ? Number(savedPage) : 1;

        paginate(page, 6);
    }, []);

    if (newsStatus === "loading") {
        return (
            <Container className="news-list">
                <List
                    className="news-list__items"
                    items={Array.from({length: 6}, (_, index) => index)}
                    renderItem={(index: number) => (
                        <ListItem
                            key={index}
                            item={index}
                            renderItem={() => <PreloaderTile className='preloader-tile__container-news-page'/>}
                        />
                    )}
                />
            </Container>
        );
    }

    if (newsStatus === "error") {
        return <NoData message='Не вдалося завантажити новини'/>;
    }

    if (newsStatus === "empty") {
        return <NoData message='Новин немає'/>;
    }

    return (
        <Container className="news-list">
            {isAdmin && <Button
                className="admin-button__add"
                onClick={() => OnShowModal(<AddNewsItemModal/>)}>
                <FaPlus/>
            </Button>}
            <List
                className="news-list__items"
                items={newsList}
                renderItem={(item: INewsShortItem) => (
                    <ListItem
                        key={item.id}
                        item={item}
                        renderItem={(item: INewsShortItem) => <NewsListItem newsItem={item}/>}
                        onClick={() =>
                            OnShowModal(
                                <NewsContextProvider>
                                    <NewsModal newsId={item.id}/>
                                </NewsContextProvider>
                            )
                        }
                    />
                )}
            />

            <PaginationSection currentPage={currentPage} totalPages={totalPages} paginate={paginate}/>
        </Container>
    );
};

export {NewsList};

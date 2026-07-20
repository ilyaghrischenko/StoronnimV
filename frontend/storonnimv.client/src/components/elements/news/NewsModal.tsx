// NewsModal.tsx
import {FC, useContext, useEffect} from "react";
import {GlobalContext} from "../../contexts/shared/GlobalContext.tsx";
import {NewsContext} from "../../contexts/NewsContext.tsx";
import {Container, Image, Button} from "react-bootstrap";
import {ModalLoading} from "../shared/ModalLoading.tsx";
import {EditNewsItemModal} from "./forms/EditNewsItemModal.tsx";
import {FaEdit, FaTrash} from "react-icons/fa";
import {DeleteNewsItemModal} from "./forms/DeleteNewsItemModal.tsx";
import {NoData} from "../shared/NoData.tsx";

interface NewsModalProps {
    newsId?: number;
}

const NewsModal: FC<NewsModalProps> = ({newsId}) => {
    const newsContext = useContext(NewsContext);
    const globalContext = useContext(GlobalContext);

    if (!newsContext || !globalContext) {
        throw new Error("Context are not defined");
    }

    const {isAdmin, OnShowModal, modalLoading} = globalContext;
    const {newsFullItem, newsFullItemStatus, fetchNewsFullItem} = newsContext;

    useEffect(() => {
        if (newsId) {
            fetchNewsFullItem(newsId);
        }
    }, [fetchNewsFullItem, newsId]);

    if (modalLoading || newsFullItemStatus === "loading") {
        return <ModalLoading/>;
    }

    if (newsFullItemStatus === "error") {
        return <NoData
            variant="error"
            message='Не вдалося завантажити новину'
            actionLabel='Спробувати ще раз'
            onAction={() => newsId && void fetchNewsFullItem(newsId)}
        />;
    }

    if (newsFullItemStatus === "empty" || !newsFullItem) {
        return <NoData message='Новину не знайдено'/>;
    }

    return (
        <Container className="news-modal">
            <h1 className="news-modal__title main-text">{newsFullItem?.title}</h1>

            <div className='news-modal__main'>
                <div className='news-modal__photo-container'>
                    {newsFullItem?.photo && <Image
                        alt={`Фото новини «${newsFullItem.title}»`}
                        className="news-modal__photo"
                        src={newsFullItem.photo}
                    />}
                </div>
                <p
                    className="news-modal__description secondary-text"
                    // style={{textAlign: `${newsFullItem?.photo ? 'left' : 'center'}`}}
                >
                    {newsFullItem?.description}
                </p>
            </div>

            <div className="news-modal__info">
                {newsFullItem?.video && (
                    <video
                        className="news-modal__video"
                        src={newsFullItem.video}
                        aria-label={`Відео новини «${newsFullItem.title}»`}
                        controls
                        preload="metadata"
                        playsInline
                    />
                )}
            </div>

            <div className="news-modal__details">
                <p className="news-modal__details-date">{newsFullItem?.date}</p>
            </div>

            {newsFullItem && isAdmin && (
                <>
                    <Button
                        aria-label="Редагувати новину"
                        className="admin-button__edit"
                        onClick={() => OnShowModal(<EditNewsItemModal newsItem={newsFullItem}/>)}
                    >
                        <FaEdit/>
                    </Button>
                    <Button
                        aria-label="Видалити новину"
                        className="admin-button__delete"
                        onClick={() => OnShowModal(<DeleteNewsItemModal newsItem={newsFullItem}/>)}
                    >
                        <FaTrash/>
                    </Button>
                </>
            )}
        </Container>
    );
};

export {NewsModal};

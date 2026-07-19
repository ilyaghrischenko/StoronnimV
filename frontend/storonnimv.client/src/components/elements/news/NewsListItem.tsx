import { FC } from "react";
import { INewsShortItem } from "../../../models/news/INewsShortItem";
import { Image } from "react-bootstrap";
import default_photo from "../../../assets/default-news-photo.jpg";

interface INewsListItemProps {
    newsItem: INewsShortItem;
    onOpen: () => void;
}

const NewsListItem: FC<INewsListItemProps> = ({ newsItem, onOpen }) => {
    return (
        <button
            type="button"
            aria-label={newsItem.title}
            className='news-list-item'
            onClick={onOpen}
        >
            <div className='news-list-item__content'>
                <Image className='news-list-item__photo'
                       alt={`Фото новини «${newsItem.title}»`}
                       src={newsItem.photo === null
                           ? default_photo : newsItem.photo} fluid />
                <div className='news-list-item__overlay'>
                    <p className='news-list-item__date big-shadow text-with-border'>{newsItem.date}</p>
                    <p className='news-list-item__title'>{newsItem.title}</p>
                </div>
            </div>
        </button>
    );
};

export { NewsListItem };

import { FC } from "react";
import { INewsShortItem } from "../../../models/news/INewsShortItem";
import { Image, ListGroupItem } from "react-bootstrap";
import default_photo from "../../../assets/default-news-photo.jpg";

interface INewsListItemProps {
    newsItem: INewsShortItem;
}

const NewsListItem: FC<INewsListItemProps> = ({ newsItem }) => {
    return (
        <ListGroupItem className='news-list-item'>
            <div className='news-list-item__content'>
                <Image className='news-list-item__photo'
                       src={newsItem.photo === null
                           ? default_photo : newsItem.photo} fluid />
                <div className='news-list-item__overlay'>
                    <p className='news-list-item__date big-shadow text-with-border'>{newsItem.date}</p>
                    <p className='news-list-item__title'>{newsItem.title}</p>
                </div>
            </div>
        </ListGroupItem>
    );
};

export { NewsListItem };

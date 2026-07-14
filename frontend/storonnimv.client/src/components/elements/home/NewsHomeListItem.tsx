import {FC} from "react";
import {IHomeNewsItem} from "../../../models/home/IHomeNewsItem";
import {Image} from "react-bootstrap";
import default_photo from "../../../assets/default-news-photo.jpg";
import {Link} from "react-router-dom";

interface INewsHomeListItemProps {
    item: IHomeNewsItem;
}

const NewsHomeListItem: FC<INewsHomeListItemProps> = ({item}) => {
    return (
        <Link aria-label={item.title} className='news-home-list-item' to='/news'>
            <div className="news-home-list-item__content">
                <Image className='news-home-list-item__photo'
                   src={item.photo === null ?
                       default_photo : item.photo}/>
                <div className='news-home-list-item__darken'/>
                <div className='news-home-list-item__overlay'>
                    <p className='news-home-list-item__title'>{item.title}</p>
                </div>
            </div>
        </Link>
    );
};

export {NewsHomeListItem};

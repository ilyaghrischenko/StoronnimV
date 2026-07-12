import {FC} from "react";
import {IHomeNewsItem} from "../../../models/home/IHomeNewsItem";
import {Container, Image} from "react-bootstrap";
import default_photo from "../../../assets/default-news-photo.jpg";

interface INewsHomeListItemProps {
    item: IHomeNewsItem;
}

const NewsHomeListItem: FC<INewsHomeListItemProps> = ({item}) => {
    return (
        <Container className='news-home-list-item'>
            <div className="news-home-list-item__content">
                <Image className='news-home-list-item__photo'
                   src={item.photo === null ?
                       default_photo : item.photo}/>
                <div className='news-home-list-item__darken'/>
                <div className='news-home-list-item__overlay'>
                    <p className='news-home-list-item__title'>{item.title}</p>
                </div>
            </div>
        </Container>
    );
};

export {NewsHomeListItem};
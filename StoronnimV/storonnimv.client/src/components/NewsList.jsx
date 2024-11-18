import {useContext} from "react";

import {ListGroup} from "react-bootstrap";

import {Context} from './Context';
import {NewsListItem} from "./NewsListItem";

const NewsList = () => {
    const {sendRequest} = useContext(Context);
    
    const newsList = [];
    
    return (
        <ListGroup className="news-list">
            {newsList.length > 0 ? (
                newsList.map((item) => (
                    <NewsListItem newsItem={item} key={item.id} />
                ))
            ) : (
                <p>Новин поки нема</p>
            )}
        </ListGroup>
    );
}

export {NewsList};
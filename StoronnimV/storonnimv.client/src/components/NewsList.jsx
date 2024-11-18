import {useContext} from "react";

import {ListGroup} from "react-bootstrap";

import {NewsListItem} from "./NewsListItem";
import {GlobalContext} from "./contexts/GlobalContext";

const NewsList = () => {
    const {sendRequest} = useContext(GlobalContext);
    
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
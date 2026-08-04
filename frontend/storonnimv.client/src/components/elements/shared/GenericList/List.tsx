import {ListGroup} from "react-bootstrap";
import {Key, ReactNode} from "react";

interface ListProps<T> {
    className?: string;
    items: T[];
    getKey: (item: T, index: number) => Key;
    renderItem: (item: T, index: number) => ReactNode;
}

export function List<T>(props: ListProps<T>) {
    return (
        <ListGroup as="ul" className={props.className}>
            {props.items.map((item, index) => (
                <ListGroup.Item as="li" key={props.getKey(item, index)}>
                    {props.renderItem(item, index)}
                </ListGroup.Item>
            ))}
        </ListGroup>
    );
}

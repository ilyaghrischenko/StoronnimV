import {ListGroupItem} from "react-bootstrap";

interface ListItemProps<T> {
    className?: string;
    item: T; // Один объект списка
    renderItem: (item: T) => React.ReactNode; // Функция рендеринга объекта
    onClick?: (item: T) => void; // Опциональный обработчик клика
}

export function ListItem<T>({ item, renderItem, onClick, className}: ListItemProps<T>) {
    const handleClick = () => {
        if (onClick) {
            onClick(item);
        }
    };

    return (
        <ListGroupItem className={className} onClick={handleClick} style={{ cursor: onClick ? "pointer" : "default" }}>
            {renderItem(item)}
        </ListGroupItem>
    );
}

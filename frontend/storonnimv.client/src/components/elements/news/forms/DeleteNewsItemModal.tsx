import {FC, useContext} from "react";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {Button, Container} from "react-bootstrap";
import {INewsFullItem} from "../../../../models/news/INewsFullItem.ts";

interface DeleteNewsItemModalProps {
    newsItem: INewsFullItem;
}

const DeleteNewsItemModal: FC<DeleteNewsItemModalProps> = ({newsItem}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, OnHideModal, serverRoute} = globalContext;

    const handleDelete = async () => {
        try {
            const response = await sendRequest(
                `${serverRoute}/admin/news/${newsItem.id}`,
                "DELETE");
            if (response.status === 204) {
                OnHideModal();
                window.location.reload();
            } else {
                alert("Error while deleting news");
            }
        } catch (error) {
            console.error("Помилка при видаленні новини:", error);
        }
    };

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Ви впевнені, що хочете видалити цю новину?</h2>
            <Container className="form-modal__form">
                <Button className="form-modal__button form-modal__button--delete" onClick={handleDelete}>
                    Видалити
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                    Скасувати
                </Button>
            </Container>
        </Container>
    );

}

export {DeleteNewsItemModal};
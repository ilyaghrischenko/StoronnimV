import {FC, useContext} from "react";
import {Button} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";

interface DeleteScheduleModalProps {
    itemId: number;
}

const DeleteScheduleModal: FC<DeleteScheduleModalProps> = ({itemId}) => {
    const globalContext = useContext(GlobalContext)!;

    const {OnHideModal, sendRequest, serverRoute} = globalContext;

    const handleDelete = async () => {
        try {
            const response = await sendRequest(
                `${serverRoute}/admin/schedules/${itemId}`,
                'DELETE'
            );

            if (response.status === 204) {
                alert('Aфішу успішно видалено!');
                window.location.reload();
            } else {
                alert('Aфішу не видалено');
            }
        } catch (error) {
            console.error('Помилка при видаленні афіші', error);
            alert("Помилка при видаленні афіші");
        } finally {
            OnHideModal();
        }
    };

    return (
        <div className="form-modal">
            <h2 className="form-modal__title">Ви впевнені, що хочете видалити афішу?</h2>
            <div className="form-modal__form">
                <Button
                    className="form-modal__button form-modal__button--delete"
                    onClick={handleDelete}
                >
                    Видалити
                </Button>
                <Button className="form-modal__button form-modal__button--cancel"
                        onClick={OnHideModal}
                >
                    Скасувати
                </Button>
            </div>
        </div>
    );
};

export {DeleteScheduleModal};
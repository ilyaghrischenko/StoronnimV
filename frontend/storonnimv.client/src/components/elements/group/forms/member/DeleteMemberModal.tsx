import {FC, useContext} from "react";
import {Button} from "react-bootstrap";
import {IMemberFullInfo} from "../../../../../models/group/IMemberInfo.ts";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";

interface DeleteMemberModalProps {
    item: IMemberFullInfo;
}

const DeleteMemberModal: FC<DeleteMemberModalProps> = ({item}) => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, OnHideModal, serverRoute} = globalContext;

    const handleDelete = async () => {
        try {
            const response = await sendRequest(
                `${serverRoute}/admin/group/members/${item.id}`,
                "DELETE"
            );

            if (response.status === 204) {
                console.log("Учасника успішно видалено:", item);
                alert("Учасника групи успішно видалено!");
            } else {
                alert("Помилка при видаленні учасника.");
            }
        } catch (error) {
            console.error("Помилка при видаленні учасника групи:", error);
            alert("Помилка при видаленні.");
        } finally {
            OnHideModal();
        }
    };

    return (
        <div className="form-modal">
            <h2 className="form-modal__title">Ви впевнені, що хочете видалити цього учасника?</h2>
            <div className="form-modal__form">
                <Button onClick={handleDelete} className="form-modal__button form-modal__button--delete">
                    Так, видалити
                </Button>
                <Button onClick={OnHideModal} className="form-modal__button form-modal__button--cancel">
                    Скасувати
                </Button>
            </div>
        </div>
    )
}

export {DeleteMemberModal};
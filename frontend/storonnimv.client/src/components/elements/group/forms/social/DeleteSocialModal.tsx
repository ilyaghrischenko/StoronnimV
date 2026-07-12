import {FC, useContext} from "react";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";
import {Button} from "react-bootstrap";

interface ISocialDeleteModalProps {
    itemId: number;
}

const DeleteSocialModal: FC<ISocialDeleteModalProps> = ({ itemId }) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const handleDelete = async () => {
        try {
            const response = await sendRequest(
                `${serverRoute}/admin/socials/${itemId}`,
                'DELETE'
            );

            if (response.status === 204) {
                alert('Соціальну мережу успішно видалено');
                OnHideModal();
            } else {
                alert('Соціальну мережу не видалено');
            }
        } catch (error) {
            console.error(error);
            alert('Помилка при видаленні соціальної мережі');
        }
    };

    return (
        <div className='form-modal'>
            <h1 className='form-modal__title'>Ви дійсно хочете видалити соціальну мережу?</h1>
            <div className="form-modal__form">
                <Button
                    className="form-modal__button form-modal__button--delete"
                    onClick={handleDelete}>
                    Видалити
                </Button>
                <Button
                    className="form-modal__button form-modal__button--cancel"
                    onClick={OnHideModal}>
                    Скасувати
                </Button>
            </div>
        </div>
    );
};

export { DeleteSocialModal };
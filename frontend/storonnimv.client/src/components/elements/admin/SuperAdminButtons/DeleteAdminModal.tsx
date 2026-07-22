import React, {useContext} from "react";
import {Button, Modal} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext";
import {ValidationErrors} from "../ValidationErrors.tsx";

interface DeleteAdminModalProps {
    adminId: number;
    onDelete: (id: number) => Promise<boolean>;
}

const DeleteAdminModal: React.FC<DeleteAdminModalProps> = ({adminId, onDelete}) => {
    const globalContext = useContext(GlobalContext)!;

    const {OnHideModal, validationErrors, setModalLoading, modalLoading} = globalContext;

    const handleDeleteAdmin = async () => {
        setModalLoading(true);
        try {
            const deleted = await onDelete(adminId);
            if (!deleted) {
                return;
            }

            alert("Адмін успішно видалений!");
            OnHideModal();
        } catch (error) {
            console.error("Помилка при видаленні адміна:", error);
            alert("Сталася помилка при видаленні адміна!");
        } finally {
            setModalLoading(false);
        }
    };

    return (
        <Modal.Dialog className="form-modal">
            <Modal.Header closeButton>
                <Modal.Title style={{color: "white"}} className="me-3">Підтвердження видалення</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <label style={{color: "white"}} className="me-3">
                    Ви дійсно хочете видалити цього адміна?
                </label>
                {Object.keys(validationErrors).length > 0 &&
                    <ValidationErrors errors={validationErrors}/>}
            </Modal.Body>
            <Modal.Footer className="form-modal__form">
                <Button className="form-modal__button form-modal__button--delete" variant="danger" type="button" onClick={handleDeleteAdmin} disabled={modalLoading}>
                    {modalLoading ? "Завантаження..." : "Так"}
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" variant="secondary" type="button" onClick={OnHideModal}>
                    Ні
                </Button>
            </Modal.Footer>
        </Modal.Dialog>
    );
};

export {DeleteAdminModal};

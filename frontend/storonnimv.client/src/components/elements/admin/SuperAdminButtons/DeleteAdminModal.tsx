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
        <Modal.Dialog>
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
            <Modal.Footer>
                <Button variant="danger" onClick={handleDeleteAdmin} disabled={modalLoading}>
                    {modalLoading ? "Завантаження..." : "Так"}
                </Button>
                <Button variant="secondary" onClick={OnHideModal}>
                    Ні
                </Button>
            </Modal.Footer>
        </Modal.Dialog>
    );
};

export {DeleteAdminModal};

import React, {useContext} from "react";
import {Button, Modal} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext";

interface DeleteAdminModalProps {
    adminId: number;
    onDelete: (id: number) => Promise<void>;
}

const DeleteAdminModal: React.FC<DeleteAdminModalProps> = ({adminId, onDelete}) => {
    const globalContext = useContext(GlobalContext)!;

    const {OnHideModal, setModalLoading, modalLoading} = globalContext;

    const handleDeleteAdmin = async () => {
        setModalLoading(true);
        try {
            await onDelete(adminId);
            alert("Адмін успішно видалений!");
            window.location.reload();
        } catch (error) {
            console.error("Помилка при видаленні адміна:", error);
            alert("Сталася помилка при видаленні адміна!");
        } finally {
            setModalLoading(false);
            OnHideModal();
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

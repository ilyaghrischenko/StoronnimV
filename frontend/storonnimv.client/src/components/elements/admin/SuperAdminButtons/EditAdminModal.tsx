import React, {useContext, useState} from "react";
import {Button, Modal, Form} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext";
import {ValidationErrors} from "../ValidationErrors.tsx";

interface EditAdminModalProps {
    admin: { id: number; login: string };
    onLoginEdit: (adminId: number, newLogin: string) => Promise<boolean>;
    onPasswordEdit: (adminId: number, oldPassword: string, newPassword: string) => Promise<boolean>;
}

const EditAdminModal: React.FC<EditAdminModalProps> = ({admin, onLoginEdit, onPasswordEdit}) => {
    const globalContext = useContext(GlobalContext)!;

    const {
        OnHideModal,
        validationErrors,
        setValidationErrors,
        setModalLoading,
        modalLoading
    } = globalContext;

    const [login, setLogin] = useState<string>(admin.login);
    const [password, setPassword] = useState<string>("");
    const [newPassword, setNewPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState<string>("");

    const handleLoginEdit = async (newLogin: string) => {
        setModalLoading(true);
        try {
            const edited = await onLoginEdit(admin.id, newLogin);
            if (!edited) {
                return;
            }

            alert("Логін успішно змінений!");
            OnHideModal();
        } catch (error) {
            console.error("Помилка при зміні логіна адміна:", error);
            alert("Сталася помилка при зміні логіна адміна!");
        } finally {
            setModalLoading(false);
        }
    };

    const handlePasswordEdit = async (oldPassword: string, newPassword: string) => {
        setValidationErrors({});

        if (newPassword !== confirmPassword) {
            setValidationErrors({ConfirmPassword: ["New password and confirmation must match"]});
            return;
        }

        if (oldPassword === newPassword) {
            setValidationErrors({NewPassword: ["New password must not be the same as the old password"]});
            return;
        }

        setModalLoading(true);
        try {
            const edited = await onPasswordEdit(admin.id, oldPassword, newPassword);
            if (!edited) {
                return;
            }

            alert("Пароль успішно змінений!");
            OnHideModal();
        } catch (error) {
            console.error("Помилка при зміні пароля адміна:", error);
            alert("Сталася помилка при зміні пароля адміна!");
        } finally {
            setModalLoading(false);
        }
    };

    return (
        <Modal.Dialog className='form-modal'>
            <Modal.Header>
                <Modal.Title className='form-modal__title'>Змінити дані Адміністратора</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form className='form-modal__form'>
                    <Form.Group controlId="edit-admin-login" className="form-modal__group">
                        <Form.Label className='form-modal__label'>Новий Логін: </Form.Label>
                        <Form.Control
                            type="text"
                            autoComplete="username"
                            value={login}
                            onChange={(e) => setLogin(e.target.value)}
                            className='form-modal__input'
                        />
                    </Form.Group>
                    <Button className="form-modal__button form-modal__button--confirm" variant="primary" type="button" onClick={() => handleLoginEdit(login)} disabled={modalLoading}>
                        {modalLoading ? "Завантаження..." : "Змінити логін"}
                    </Button>

                    <Form.Group controlId="edit-admin-old-password" className="form-modal__group">
                        <Form.Label className='form-modal__label'>Старий Пароль: </Form.Label>
                        <Form.Control
                            type="password"
                            autoComplete="current-password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            className='form-modal__input'
                            placeholder="Введіть старий пароль"
                        />
                    </Form.Group>
                    <Form.Group controlId="edit-admin-new-password" className="form-modal__group">
                        <Form.Label className='form-modal__label'>Новий Пароль: </Form.Label>
                        <Form.Control
                            type="password"
                            autoComplete="new-password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            className='form-modal__input'
                            placeholder="Введіть новий пароль"
                        />
                    </Form.Group>
                    <Form.Group controlId="edit-admin-confirm-password" className="form-modal__group">
                        <Form.Label className='form-modal__label'>Підтвердження пароля: </Form.Label>
                        <Form.Control
                            type="password"
                            autoComplete="new-password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            className='form-modal__input'
                            placeholder="Підтвердження пароля"
                        />
                    </Form.Group>

                    {validationErrors && Object.keys(validationErrors).length > 0 &&
                        <ValidationErrors errors={validationErrors}/>}

                    <Button className="form-modal__button form-modal__button--confirm" variant="primary" type="button" onClick={() => handlePasswordEdit(password, newPassword)}
                            disabled={modalLoading}>
                        {modalLoading ? "Завантаження..." : "Змінити пароль"}
                    </Button>
                    <Button className="form-modal__button form-modal__button--cancel" variant="secondary" type="button" onClick={OnHideModal}>
                        Закрити
                    </Button>
                </Form>
            </Modal.Body>
        </Modal.Dialog>
    );
};

export {EditAdminModal};

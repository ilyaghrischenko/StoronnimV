import React, {useContext, useState} from "react";
import {Button, Modal, Form} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext";
import {ValidationErrors} from "../ValidationErrors.tsx";

interface IAddAdminModalProps {
    onAdding: (login: string, password: string) => Promise<boolean>;
}

const AddAdminModal: React.FC<IAddAdminModalProps> = ({onAdding}) => {
    const globalContext = useContext(GlobalContext)!;

    const {OnHideModal, validationErrors, setModalLoading, modalLoading} = globalContext;

    const [login, setLogin] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    const handleAddAdmin = async () => {
        setModalLoading(true);
        try {
            const added = await onAdding(login, password);
            if (!added) {
                return;
            }

            alert("Адмін успішно доданий!");
            OnHideModal();
        } catch (error) {
            console.error("Помилка при додаванні адміна:", error);
            alert("Сталася помилка при додаванні адміна!");
        } finally {
            setModalLoading(false);
        }
    };

    return (
        <Modal.Dialog className='form-modal'>
            <Modal.Header>
                <Modal.Title className="form-modal__title">Додати Адміна</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form className='form-modal__form'
                onSubmit={(e) => {
                    e.preventDefault();
                    handleAddAdmin();
                }}>
                    <Form.Group controlId="add-admin-login" className="form-modal__group">
                        <Form.Label className='form-modal__label'>Логін: </Form.Label>
                        <Form.Control
                            type="text"
                            autoComplete="username"
                            placeholder="Введіть логін"
                            value={login}
                            onChange={(e) => setLogin(e.target.value)}
                            required
                            className='form-modal__input'
                        />
                    </Form.Group>

                    <Form.Group controlId="add-admin-password" className="form-modal__group">
                        <Form.Label className='form-modal__label'>Пароль: </Form.Label>
                        <Form.Control
                            type="password"
                            autoComplete="new-password"
                            placeholder="Введіть пароль"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            className='form-modal__input'
                        />
                    </Form.Group>

                    {validationErrors && Object.keys(validationErrors).length > 0 &&
                        <ValidationErrors errors={validationErrors}/>}

                    <Button className="form-modal__button form-modal__button--cancel" variant="secondary" type="button" onClick={OnHideModal}>
                        Закрити
                    </Button>
                    <Button className="form-modal__button form-modal__button--confirm" type='submit' variant="primary" disabled={modalLoading}>
                        {modalLoading ? "Завантаження..." : "Додати"}
                    </Button>
                </Form>
            </Modal.Body>
        </Modal.Dialog>
    );
};

export {AddAdminModal};

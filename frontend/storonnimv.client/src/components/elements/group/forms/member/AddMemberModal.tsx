import React, {useContext, useState, FC} from "react";
import {Container, Form, Button} from "react-bootstrap";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";

const AddMemberModal: FC = () => {
    const {sendRequest, OnHideModal, serverRoute} = useContext(GlobalContext)!;

    const [fullName, setFullName] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [role, setRole] = useState<string>("");
    const [photo, setPhoto] = useState<File>({} as File);

    const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setPhoto(file);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        try {
            const formData = new FormData();
            formData.append("fullName", fullName);
            formData.append("description", description);
            formData.append("role", role);
            formData.append("photoUrl", photo);

            const response = await sendRequest(
                `${serverRoute}/admin/group/members`,
                "POST",
                formData
            );

            if (response.status === 201) {
                alert("Учасник успішно додано!");
                window.location.reload();
            } else {
                alert("Помилка при додаванні учасника");
            }
        } catch (error) {
            console.error(error);
            alert("Помилка при додаванні учасника");
        } finally {
            OnHideModal();
        }
    };

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Додати учасника групи</h2>
            <Form className="form-modal__form" onSubmit={handleSubmit}>
                <Form.Group controlId="formFullName" className="form-modal__group">
                    <Form.Label className="form-modal__label">ПІБ: </Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="text"
                        placeholder="Введіть повне ім'я"
                        required
                        onChange={e => setFullName(e.target.value)}
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Опис: </Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        as="textarea"
                        rows={3}
                        placeholder="Введіть опис"
                        required
                        onChange={e => setDescription(e.target.value)}
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Роль: </Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="text"
                        placeholder="Введіть роль"
                        required
                        onChange={e => setRole(e.target.value)}
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Фото учасника: </Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="file"
                        accept="image/*"
                        required
                        onChange={handlePhotoUpload}
                    />
                </Form.Group>

                <Button className="form-modal__button form-modal__button--confirm" type="submit">
                    Додати
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                    Скасувати
                </Button>
            </Form>
        </Container>
    );
};

export {AddMemberModal};

import React, {FC, useContext, useState} from "react";
import {Container, Form, Button} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";

const AddScheduleModal: FC = () => {
    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, OnHideModal, serverRoute} = globalContext;

    const [title, setTitle] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [location, setLocation] = useState<string>("");
    const [performanceDateTime, setPerformanceDateTime] = useState<string>("");
    const [photo, setPhoto] = useState<File | null>(null);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const {name, value} = e.target;
        if (name === "title") setTitle(value);
        else if (name === "description") setDescription(value);
        else if (name === "location") setLocation(value);
        else if (name === "performanceDateTime") setPerformanceDateTime(value);
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file && file.type.startsWith("image/")) {
            setPhoto(file);
        } else {
            alert("Будь ласка, завантажте тільки зображення.");
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const formData = new FormData();
        formData.append("title", title);
        formData.append("description", description);
        formData.append("location", location);
        formData.append("status", "Active");  // Статус автоматически установлен как "active"
        formData.append("performanceDateTime", performanceDateTime);
        if (photo) formData.append("photo", photo);

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/schedules`,
                "POST",
                formData
            );

            if (response.status === 201) {
                alert(`Афішу успішно додано!`);
                window.location.reload();
            } else {
                alert("Сталася помилка при додаванні афіши");
            }
        } catch (error) {
            alert(`Помилка при додаванні афіши ${error}`);
        } finally {
            OnHideModal();
        }
    };

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Додати афішу</h2>
            <Form className="form-modal__form" onSubmit={handleSubmit}>
                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="text"
                        name="title"
                        value={title}
                        onChange={handleChange}
                        placeholder={`Введіть заголовок`}
                        required
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Опис:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        as="textarea"
                        rows={3}
                        name="description"
                        value={description}
                        onChange={handleChange}
                        placeholder={`Введіть опис`}
                        required
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Місце проведення:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="text"
                        name="location"
                        value={location}
                        onChange={handleChange}
                        placeholder={`Введіть місце проведення`}
                        required
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Дата та час проведення:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="datetime-local"
                        name="performanceDateTime"
                        value={performanceDateTime}
                        onChange={handleChange}
                        required
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Фото:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="file"
                        accept="image/*"
                        id="imageUpload"
                        onChange={handleFileChange}
                    />
                </Form.Group>

                <Button type="submit" className="form-modal__button form-modal__button--confirm">
                    Додати
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                    Скасувати
                </Button>
            </Form>
        </Container>
    )
        ;
};

export {AddScheduleModal};

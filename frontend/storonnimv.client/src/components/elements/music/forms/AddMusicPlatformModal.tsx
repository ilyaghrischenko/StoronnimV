import React, {useState, useContext, FC} from "react";
import {Button, Form, Container} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";

const AddMusicPlatformModal: FC = () => {
    const [platformUrl, setPlatformUrl] = useState<string>("");
    const [bgImage, setBgImage] = useState<File | null>(null);

    const globalContext = useContext(GlobalContext)!;

    const {sendRequest, OnHideModal, serverRoute} = globalContext;

    const handleChangePhoto = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setBgImage(file);
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const formData = new FormData();
            formData.append("platformUrl", platformUrl);
            if (bgImage) formData.append("bgImageUrl", bgImage);

            const response = await sendRequest(
                `${serverRoute}/admin/music`,
                "POST",
                formData);

            if (response.status === 201) {
                alert("Музична платформа додана!");
                window.location.reload();
            } else {
                alert("Помилка при додаванні музичної платформи");
            }
        } catch (error) {
            console.error("Помилка при додаванні музичної платформи:", error);
            alert("Помилка при додаванні музичної платформи");
        } finally {
            OnHideModal();
        }
    };

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Додати музичну платформу</h2>
            <Form className="form-modal__form" onSubmit={handleSubmit}>
                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">URL платформи:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="url"
                        name="platformUrl"
                        onChange={e => setPlatformUrl(e.target.value)}
                        placeholder="Введіть URL платформи"
                        pattern="https?://.*"
                        required
                    />
                </Form.Group>
                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Виберіть файл зображення:</Form.Label>
                    <Form.Control
                        className="form-modal__input"
                        type="file"
                        accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
                        id="imageUpload"
                        onChange={handleChangePhoto}
                        required
                    />
                </Form.Group>

                <Button
                    className="form-modal__button form-modal__button--confirm"
                    type="submit"
                    disabled={!platformUrl || !bgImage}
                >
                    Додати платформу
                </Button>
                <Button className="form-modal__button form-modal__button--cancel"
                        onClick={OnHideModal}>Скасувати</Button>
            </Form>
        </Container>
    );
};

export {AddMusicPlatformModal};

import React, {FC, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";

const AddGroupSocialModal: FC = () => {
    const globalContext = useContext(GlobalContext)!;

    const {OnHideModal, sendRequest, setModalLoading, serverRoute} = globalContext;

    const [photo, setPhoto] = useState<File | null>(null);
    const [name, setName] = useState<string>("Other");
    const [linkUrl, setLinkUrl] = useState<string>("");

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file && file.type.startsWith("image/")) {
            setPhoto(file);
        } else {
            alert("Будь ласка, завантажте тільки зображення.");
        }
    };

    const formIsValid = photo && linkUrl != "";

    const handleSubmit = async () => {
        if (!formIsValid) return;

        setModalLoading(true);
        const formData = new FormData();
        formData.append("photo", photo!);
        formData.append("name", name);
        formData.append("linkUrl", linkUrl);

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/group-socials`,
                "POST",
                formData
            );

            if (response.status === 201) {
                alert('Соціальна мережа групи успішно додана!');
                OnHideModal();
                window.location.reload();
            }
        } catch (error) {
            alert('Помилка при додаванні соціальної мережі групи');
            console.error(error);
        } finally {
            setModalLoading(false);
        }
    };

    return (
        <div className='form-modal'>
            <h2 className='form-modal__title'>Додати соціальну мережу групи</h2>
            <Form
                className='form-modal__form'
                onSubmit={(e) => {
                    e.preventDefault();
                    handleSubmit();
                }}
            >
                <Form.Group className='form-modal__group'>
                    <Form.Label className='form-modal__label'>Фото:</Form.Label>
                    <Form.Control
                        className='form-modal__input'
                        type='file'
                        accept='.jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp'
                        id='imageUpload'
                        onChange={handleFileChange}
                    />
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Соціальна мережа:</Form.Label>
                    <Form.Select
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        className="form-modal__select"
                    >
                        <option value="Instagram">Instagram</option>
                        <option value="Facebook">Facebook</option>
                        <option value="Twitter">Twitter</option>
                        <option value="Telegram">Telegram</option>
                        <option value="YouTube">YouTube</option>
                        <option value="Email">Email</option>
                        <option value="Other">Other</option>
                    </Form.Select>
                </Form.Group>

                <Form.Group className="form-modal__group">
                    <Form.Label className="form-modal__label">Посилання:</Form.Label>
                    <Form.Control
                        type="url"
                        value={linkUrl}
                        onChange={(e) => setLinkUrl(e.target.value)}
                        pattern="https?://.*"
                        required
                        className="form-modal__input"
                    />
                </Form.Group>

                <Button type="submit" className="form-modal__button form-modal__button--confirm">
                    Додати
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                    Скасувати
                </Button>
            </Form>
        </div>
    );
};

export {AddGroupSocialModal};

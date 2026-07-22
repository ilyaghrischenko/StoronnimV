import {IGroupSocial} from "../../../../../models/groupSocials/IGroupSocial.ts";
import {FC, useContext, useState} from "react";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";
import {Button, Form} from "react-bootstrap";

interface IEditGroupSocialModalProps {
    item: IGroupSocial;
}

const EditGroupSocialModal: FC<IEditGroupSocialModalProps> = ({item}) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const [linkUrl, setLinkUrl] = useState<string>(item.linkUrl);
    const [photo, setPhoto] = useState<File | null>(null);

    const handleSubmit = async () => {
        const data = {
            id: item.id,
            linkUrl
        };

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/group-socials`,
                "PATCH",
                data,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert("Дані успішно змінено!");
                OnHideModal();
                window.location.reload();
            } else {
                alert("Помилка при зміні даних");
            }
        } catch (error) {
            console.error("Помилка при зміні даних", error);
            alert("Помилка при зміні даних");
        }
    };

    const handlePhotoSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!photo) return;

        const data = new FormData();
        data.append("id", item.id.toString());
        data.append("photo", photo);

        try {
            const response = await sendRequest(
                `${serverRoute}/admin/group-socials/photo`,
                "PATCH",
                data
            );

            if (response.status === 204) {
                alert("Фото успішно змінено!");
                window.location.reload();
            } else {
                alert("Помилка при зміні фото");
            }
        } catch (error) {
            console.error("Помилка при зміні фото", error);
            alert("Помилка при зміні фото");
        } finally {
            OnHideModal();
        }
    };

    return (
        <div className='form-modal'>
            <h2 className='form-modal__title'>Редагувати соціальну мережу групи</h2>

            <Form
                className='form-modal__form'
                onSubmit={(e) => {
                    e.preventDefault();
                    handleSubmit();
                }}
            >
                <Form.Group controlId="edit-footer-social-url" className='form-modal__group'>
                    <Form.Label className="form-modal__label">Посилання:</Form.Label>
                    <Form.Control
                        type="url"
                        value={linkUrl}
                        required
                        onChange={(e) => setLinkUrl(e.target.value)}
                        pattern="https?://.*"
                        className="form-modal__input"
                    />
                </Form.Group>

                <Button type="submit" className="form-modal__button form-modal__button--confirm">
                    Зберегти зміни
                </Button>
            </Form>

            <Form className='form-modal__form' onSubmit={handlePhotoSubmit}>
                <Form.Group controlId="edit-footer-social-photo" className='form-modal__group'>
                    <Form.Label className="form-modal__label">Фото:</Form.Label>
                    <Form.Control
                        type="file"
                        accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
                        onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                            setPhoto(e.target.files?.[0] ?? null)}
                        className="form-modal__input"
                        required
                    />
                </Form.Group>

                <Button
                    type="submit"
                    disabled={!photo}
                    className="form-modal__button form-modal__button--confirm"
                >
                    Зберегти фото
                </Button>
                <Button
                    type="button"
                    className="form-modal__button form-modal__button--cancel"
                    onClick={OnHideModal}
                >
                    Скасувати
                </Button>
            </Form>
        </div>
    );
};

export {EditGroupSocialModal};

import React, {FC, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";
import {IGroupPageFullInfo} from "../../../../../models/group/IGroupInfo.ts";

interface IEditGroupModalProps {
    fullInfo: IGroupPageFullInfo;
}

const EditGroupModal: FC<IEditGroupModalProps> = ({fullInfo}) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const [description, setDescription] = useState<string>(fullInfo.groupPage.description);
    const [photo, setPhoto] = useState<File>({} as File);

    const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setPhoto(file);
        }
    };

    const handleDescriptionChange = async () => {
        try {
            const formData = new FormData();
            formData.append("id", fullInfo.groupPage.id.toString());
            formData.append("description", description);

            const response = await sendRequest(
                `${serverRoute}/admin/group-pages`,
                "PATCH",
                formData,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert('Опис групи успішно змінено');
                window.location.reload();
            } else {
                alert('Помилка при зміні опису групи');
            }
        } catch (error) {
            console.error("Помилка при зміні опису групи", error);
            alert('Помилка при зміні опису групи');
        } finally {
            OnHideModal();
        }
    };

    const handlePhotoChange = async () => {
        try {
            const formData = new FormData();
            formData.append("id", fullInfo.groupPage.id.toString());
            formData.append("photo", photo);

            const response = await sendRequest(
                `${serverRoute}/admin/group-page/photo`,
                "PATCH",
                formData
            );

            if (response.status === 204) {
                alert('Фото групи успішно змінено');
                window.location.reload();
            } else {
                alert('Помилка при зміні фото групи');
            }
        } catch (error) {
            console.error("Ошибка при изменении фото: ", error);
        } finally {
            OnHideModal();
        }
    };

    return (
        <>
            <div className="form-modal form-modal__container">
                <h2 className="form-modal__title">Редагувати групу</h2>
                <Form className="form-modal__form"
                      onSubmit={(e) => {
                          e.preventDefault();
                          handleDescriptionChange();
                      }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Опис групи:</Form.Label>
                        <Form.Control
                            className="form-modal__input"
                            as="textarea"
                            rows={3}
                            id="descriptionInput"
                            value={description}
                            required
                            onChange={(e) => setDescription(e.target.value)}
                            placeholder="Введіть новий опис групи"
                        />
                    </Form.Group>
                    <Button className="form-modal__button form-modal__button--confirm" type="submit">
                        Зберегти
                    </Button>
                </Form>
            </div>
            <div className="form-modal form-modal__container">
                <Form className="form-modal__form"
                      onSubmit={(e) => {
                          e.preventDefault();
                          handlePhotoChange();
                      }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Фото групи:</Form.Label>
                        <Form.Control
                            type="file"
                            accept="image/*"
                            required
                            onChange={handlePhotoUpload}
                            className="form-modal__input"
                        />
                    </Form.Group>
                    <Button className="form-modal__button form-modal__button--confirm" type="submit">
                        Зберегти
                    </Button>
                </Form>
            </div>

            <div className="form-modal">
                <div className="form-modal__form">
                    <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                        Скасувати
                    </Button>
                </div>
            </div>
        </>
    );
};

export {EditGroupModal};
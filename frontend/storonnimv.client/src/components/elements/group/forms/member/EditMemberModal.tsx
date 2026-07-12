import React, {FC, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {IMemberFullInfo} from "../../../../../models/group/IMemberInfo.ts";
import {GlobalContext} from "../../../../contexts/shared/GlobalContext.tsx";

interface IEditMemberModalProps {
    item: IMemberFullInfo;
}

const EditMemberModal: FC<IEditMemberModalProps> = ({item}) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const [fullName, setFullName] = useState<string>(item.fullName);
    const [description, setDescription] = useState<string>(item.description);
    const [role, setRole] = useState<string>(item.role);
    const [photo, setPhoto] = useState<File>({} as File);

    const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setPhoto(file);
        }
    };

    const handlePhotoEdit = async () => {
        try {
            if (!photo) {
                alert("Спочатку оберіть фото");
                return;
            }

            const formData = new FormData();
            formData.append("id", item.id.toString());
            formData.append("photo", photo);

            const response = await sendRequest(
                `${serverRoute}/admin/group-page/members/photo`,
                "PATCH",
                formData
            );

            if (response.status === 204) {
                alert("Дані успішно змінено");
                window.location.reload();
            } else {
                alert("Сталася помилка при зміні");
            }
        } catch (error) {
            alert("Помилка при збереженні даних.");
            console.error(error);
        } finally {
            OnHideModal();
        }
    };

    const handleEdit = async () => {
        try {
            const formData = new FormData();
            formData.append("id", item.id.toString());
            formData.append("fullName", fullName);
            formData.append("description", description);
            formData.append("role", role);

            const response = await sendRequest(
                `${serverRoute}/admin/group-pages/members`,
                "PATCH",
                formData,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert("Інформацію про учасника успішно оновлено!");
                window.location.reload();
            } else {
                alert("Помилка при оновленні інформації учасника.");
            }
        } catch (error) {
            alert("Помилка при збереженні даних.");
            console.error(error);
        } finally {
            OnHideModal();
        }
    };

    return (
        <>
            <div className="form-modal form-modal__container">
                <h2 className="form-modal__title">Редагувати учасника</h2>
                <Form className="form-modal__form"
                      onSubmit={(e) => {
                          e.preventDefault();
                          handlePhotoEdit();
                      }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Фото учасника:</Form.Label>
                        <Form.Control
                            type="file"
                            accept="image/*"
                            className="form-modal__input"
                            required
                            onChange={handlePhotoUpload}
                        />
                    </Form.Group>

                    <Button type="submit" className="form-modal__button form-modal__button--confirm">
                        Змінити фото
                    </Button>
                </Form>
            </div>
            <div className="form-modal form-modal__container">
                <Form
                    className="form-modal__form"
                    onSubmit={(e) => {
                        e.preventDefault();
                        handleEdit();
                    }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">
                            Повне ім’я:
                        </Form.Label>
                        <Form.Control
                            type="text"
                            required
                            value={fullName}
                            onChange={(e) => setFullName(e.target.value)}
                            className="form-modal__input"
                        />
                    </Form.Group>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">
                            Опис:
                        </Form.Label>
                        <Form.Control
                            type="text"
                            required
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            className="form-modal__input"
                        />
                    </Form.Group>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">
                            Роль:
                        </Form.Label>
                        <Form.Control
                            type="text"
                            required
                            value={role}
                            onChange={(e) => setRole(e.target.value)}
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Button type="submit" className="form-modal__button form-modal__button--confirm">
                        Змінити
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

export {EditMemberModal};
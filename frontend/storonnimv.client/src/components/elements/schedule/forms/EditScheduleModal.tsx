import React, {ChangeEvent, FC, useContext, useState} from "react";
import {Button, Form} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {ISchedule} from "../../../../models/schedule/ISchedule.ts";

interface IScheduleEditModalProps {
    item: ISchedule;
}

const EditScheduleModal: FC<IScheduleEditModalProps> = ({item}) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const [title, setTitle] = useState<string>(item.title);
    const [description, setDescription] = useState<string>(item.description);
    const [location, setLocation] = useState<string>(item.location);
    const [performanceDateTime, setPerformanceDateTime] = useState<string>(item.performanceDateTime);
    const [photo, setPhoto] = useState<File>({} as File);

    const formatDateTimeForInput = (dateStr: string): string => {
        if (!dateStr) return "";

        const [datePart, timePart] = dateStr.split(" ");
        const [day, month, year] = datePart?.split(".") ?? [];

        let hours = "00";
        let minutes = "00";

        if (timePart) {
            const timeSplit = timePart.split(":");
            hours = timeSplit[0] ?? "00";
            minutes = timeSplit[1] ?? "00";
        }

        if (!day || !month || !year) return ""; // захист від неправильного формату

        return `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}T${hours.padStart(2, "0")}:${minutes.padStart(2, "0")}`;
    };


    const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setPhoto(file);
        }
    };

    const handleDateTimeChange = (e: ChangeEvent<HTMLInputElement>) => {
        setPerformanceDateTime(e.target.value);
    };

    const handlePhotoEdit = async () => {
        try {
            if (!photo) {
                alert("Спочатку завантажте фото");
                return;
            }

            const formData = new FormData();
            formData.append("id", item.id.toString());
            formData.append("photo", photo);

            const response = await sendRequest(
                `${serverRoute}/admin/schedules/photo`,
                "PATCH",
                formData
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

    const handleEdit = async () => {
        try {
            const data = {
                id: item.id,
                title,
                performanceDateTime: performanceDateTime.includes("T")
                    ? performanceDateTime
                    : formatDateTimeForInput(performanceDateTime),
                description,
                location
            };

            const response = await sendRequest(
                `${serverRoute}/admin/schedules`,
                "PATCH",
                data,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert("Дані успішно змінено!");
                window.location.reload();
            } else {
                alert("Помилка при зміні даних");
            }
        } catch (error) {
            console.error("Помилка при зміні даних", error);
            alert("Помилка при зміні даних");
        } finally {
            OnHideModal();
        }
    };

    return (
        <>
            <div className='form-modal form-modal__container'>
                <h2 className="form-modal__title">Редагувати афішу</h2>

                <Form
                    className='form-modal__form'
                    onSubmit={(e) => {
                        e.preventDefault();
                        handleEdit();
                    }}>
                    <Form.Group className='form-modal__group' controlId="formTitle">
                        <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                        <Form.Control
                            type="text"
                            value={title}
                            required
                            onChange={(e) => setTitle(e.target.value)}
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Form.Group controlId="formDescription" className="form-modal__group">
                        <Form.Label className="form-modal__label">Опис:</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={3}
                            value={description}
                            required
                            onChange={(e) => setDescription(e.target.value)}
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Form.Group controlId="formLocation" className="form-modal__group">
                        <Form.Label className="form-modal__label">Місце проведення:</Form.Label>
                        <Form.Control
                            type="text"
                            value={location}
                            required
                            onChange={(e) => setLocation(e.target.value)}
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Form.Group controlId="formPerformanceDateTime" className="form-modal__group">
                        <Form.Label className="form-modal__label">Дата та час проведення:</Form.Label>
                        <Form.Control
                            type="datetime-local"
                            value={formatDateTimeForInput(performanceDateTime)}
                            required
                            onChange={handleDateTimeChange}
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Button type="submit" className="form-modal__button form-modal__button--confirm">
                        Зберегти зміни
                    </Button>
                </Form>
            </div>
            <div className='form-modal form-modal__container'>
                <Form
                    className='form-modal__form'
                    onSubmit={(e) => {
                    e.preventDefault();
                    handlePhotoEdit();
                }}>
                    <Form.Group controlId="formPhoto" className="form-modal__group">
                        <Form.Label className="form-modal__label">Фото:</Form.Label>
                        <Form.Control
                            type="file"
                            required
                            onChange={handlePhotoUpload}
                            accept="image/*"
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Button type="submit" className="form-modal__button form-modal__button--confirm">
                        Зберегти фото
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

export {EditScheduleModal};

import React, {FC, useContext, useEffect, useState} from "react";
import {INewsFullItem} from "../../../../models/news/INewsFullItem.ts";
import {Button, Container, Form} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";

interface EditNewsItemModalContentProps {
    newsItem: INewsFullItem;
}

const EditNewsItemModal: FC<EditNewsItemModalContentProps> = ({newsItem}) => {
    const globalContext = useContext(GlobalContext);
    const [editedNews, setEditedNews] = useState<INewsFullItem>(newsItem);
    const [initialNews, setInitialNews] = useState<INewsFullItem>(newsItem);
    const [newPhoto, setNewPhoto] = useState<File | null>(null);
    const [newVideoId, setNewVideoId] = useState<string | null>(null);

    useEffect(() => {
        setEditedNews(newsItem);
        setInitialNews(newsItem);
    }, [newsItem]);


    if (!globalContext) {
        return null;
    }

    const {sendRequest, OnHideModal, serverRoute} = globalContext;

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        const {name, value} = e.target;

        if (name === "date") {
            const [year, month, day] = value.split("-");
            const formattedDate = `${day}.${month}.${year}`;

            setEditedNews((prev) => ({
                ...prev,
                [name]: formattedDate,
            }));
        } else {
            setEditedNews((prev) => ({
                ...prev,
                [name]: value,
            }));
        }
    };


    const handleChangePhoto = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setNewPhoto(file);
        }
    };

    const handleChangeVideoId = (e: React.ChangeEvent<HTMLInputElement>) => {
        setNewVideoId(e.target.value);
    };

    const isTextEdited = (): boolean => {
        return (
            editedNews.title !== initialNews.title ||
            editedNews.description !== initialNews.description ||
            editedNews.priority !== initialNews.priority ||
            editedNews.date !== initialNews.date
        );
    };
    const formatDateForInput = (dateStr: string): string => {
        if (!dateStr) return "";

        const [day, month, year] = dateStr.split(".");
        return `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}`;
    }


    const handleDeletePhoto = async () => {
        try {
            const id = JSON.stringify(editedNews.id);

            const response = await sendRequest(
                `${serverRoute}/admin/news/delete-photo`,
                "PATCH",
                id,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert("Фото для новини успішно видалено!");
                window.location.reload();
            } else {
                alert("Помилка під час видалення фото новини!");
            }

        } catch (error) {
            console.error("Помилка під час оновлення новини:", error);
        }
    };

    const handleDeleteVideo = async () => {
        try {
            const id = JSON.stringify(editedNews.id);

            const response = await sendRequest(
                `${serverRoute}/admin/news/delete-video`,
                "PATCH",
                id,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                alert("Відео для новини успішно видалено!");
                window.location.reload();
            } else {
                alert("Помилка під час видалення відео новини!");
            }

        } catch (error) {
            console.error("Помилка під час оновлення новини:", error);
        }
    };

    const handleSaveTextInformation = async () => {
        try {
            const formData = new FormData();
            formData.append("id", editedNews.id.toString());
            formData.append("title", editedNews.title);
            formData.append("description", editedNews.description);
            formData.append("priority", editedNews.priority);
            formData.append("date", editedNews.date);

            const response = await sendRequest(
                `${serverRoute}/admin/news`,
                "PATCH",
                formData,
                {"Content-Type": "application/json"}
            );

            OnHideModal();

            if (response.status === 204) {
                alert("Новина успішно оновлена!");
                window.location.reload();
            } else {
                alert("Помилка під час оновлення новини!");
            }
        } catch (error) {
            console.error("Помилка під час оновлення новини:", error);
        }
    };

    const handleSavePhoto = async () => {
        try {
            const formData = new FormData();
            formData.append("id", editedNews.id.toString());
            formData.append("photo", newPhoto!);

            const response = await sendRequest(
                `${serverRoute}/admin/news/photo`,
                "PATCH",
                formData
            );

            OnHideModal();

            if (response.status === 204) {
                alert("Фото для новини успішно оновлено!");
                window.location.reload();
            } else {
                alert("Помилка під час оновлення фото новини!");
            }
        } catch (error) {
            console.error("Помилка під час оновлення новини:", error);
        }
    };

    const handleSaveVideoId = async () => {
        try {
            const formData = new FormData();
            formData.append("id", editedNews.id.toString());
            formData.append("videoId", newVideoId!);

            const response = await sendRequest(
                `${serverRoute}/admin/news/video`,
                "PATCH",
                formData,
                {"Content-Type": "application/json"}
            );

            OnHideModal();

            if (response.status === 204) {
                alert("Відео для новини успішно оновлено!");
                window.location.reload();
            } else {
                alert("Помилка під час оновлення відео новини!");
            }
        } catch (error) {
            console.error("Помилка під час оновлення новини:", error);
        }
    };

    return (
        <>
            <Container className="form-modal form-modal__container">
                <h2 className="form-modal__title">Редагувати новину</h2>

                <Form className="form-modal__form" onSubmit={(e) => {
                    e.preventDefault();
                    if (!isTextEdited()) {
                        alert("Жодне поле не змінено!");
                        return;
                    }
                    handleSaveTextInformation();
                }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                        <Form.Control
                            type="text"
                            name="title"
                            value={editedNews.title || ""}
                            onChange={handleChange}
                            className="form-modal__input"
                            placeholder="Введіть заголовок новини"
                            required
                        />
                    </Form.Group>

                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Опис:</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={3}
                            name="description"
                            value={editedNews.description || ""}
                            onChange={handleChange}
                            className="form-modal__input"
                            placeholder="Напишіть опис новини"
                            required
                        />
                    </Form.Group>

                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Пріоритет:</Form.Label>
                        <Form.Select
                            name="priority"
                            value={editedNews.priority || ""}
                            onChange={handleChange}
                            className="form-modal__select"
                            required
                        >
                            <option value="Secondary">Secondary</option>
                            <option value="Main">Main</option>
                        </Form.Select>
                    </Form.Group>

                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Дата:</Form.Label>
                        <Form.Control
                            type="date"
                            name="date"
                            value={formatDateForInput(editedNews.date || "")}
                            onChange={handleChange}
                            className="form-modal__input"
                        />
                    </Form.Group>

                    <Button
                        type="submit"
                        className="form-modal__button form-modal__button--confirm"
                        disabled={!isTextEdited()}
                    >
                        Зберегти
                    </Button>
                </Form>
            </Container>

            <Container className="form-modal form-modal__container">
                <Form className="form-modal__form" onSubmit={(e) => {
                    e.preventDefault();
                    if (!newPhoto) {
                        alert("Оберіть фото перед збереженням!");
                        return;
                    }
                    handleSavePhoto();
                }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Фото:</Form.Label>
                        <Form.Control
                            type="file"
                            onChange={handleChangePhoto}
                            className="form-modal__input"
                            required
                            accept="image/*"
                        />
                    </Form.Group>

                    <Button
                        type="submit"
                        className="form-modal__button form-modal__button--confirm"
                        disabled={!newPhoto}
                    >
                        Зберегти
                    </Button>
                    <Button
                        className="form-modal__button form-modal__button--delete"
                        disabled={editedNews.photo === null}
                        onClick={handleDeletePhoto}
                    >
                        Видалити фото
                    </Button>
                </Form>
            </Container>

            <Container className="form-modal form-modal__container">
                <Form className="form-modal__form" onSubmit={(e) => {
                    e.preventDefault();
                    if (!newVideoId || newVideoId.trim() === "") {
                        alert("Введіть ID відео перед збереженням!");
                        return;
                    }

                    if (!/^\d+$/.test(newVideoId.trim())) {
                        alert("ID відео має містити тільки цифри!");
                        return;
                    }

                    handleSaveVideoId();
                }}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Відео:</Form.Label>
                        <Form.Control
                            type="text"
                            name="videoId"
                            value={newVideoId || ""}
                            onChange={handleChangeVideoId}
                            className="form-modal__input"
                            placeholder="Введіть id відео для новини"
                            required
                        />
                    </Form.Group>

                    <Button
                        type="submit"
                        className="form-modal__button form-modal__button--confirm"
                        disabled={!newVideoId || newVideoId.trim() === ""}
                    >
                        Зберегти
                    </Button>
                    <Button
                        className="form-modal__button form-modal__button--delete"
                        disabled={editedNews.video === null}
                        onClick={handleDeleteVideo}
                    >
                        Видалити відео
                    </Button>
                </Form>
            </Container>
            <Container className="form-modal">
                <Container className="form-modal__form">
                    <Button className="form-modal__button form-modal__button--cancel" onClick={OnHideModal}>
                        Скасувати
                    </Button>
                </Container>
            </Container>
        </>
    );
};

export {EditNewsItemModal};

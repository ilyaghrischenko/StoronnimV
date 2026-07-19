import React, {FC, useContext, useEffect, useState} from "react";
import {Button, Container, Form} from "react-bootstrap";
import {IMusicPlatformItem} from "../../../../models/music/IMusicPlatformItem.ts";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";

interface EditMusicPlatformProps {
    item: IMusicPlatformItem;
}

const EditMusicPlatformModal: FC<EditMusicPlatformProps> = ({item}) => {
    const {OnHideModal, sendRequest, serverRoute} = useContext(GlobalContext)!;

    const [newPlatformUrl, setNewPlatformUrl] = useState<string | null>(item.platformUrl);
    const [initialPlatformUrl, setInitialPlatformUrl] = useState<string | null>(item.platformUrl);

    const [bgImage, setBgImage] = useState<File | null>(null);

    useEffect(() => {
        setNewPlatformUrl(item.platformUrl);
        setInitialPlatformUrl(item.platformUrl);
    }, [item]);

    const isTextEdited = (): boolean => {
        return (
            newPlatformUrl !== initialPlatformUrl
        );
    };

    const handleChangePhoto = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            setBgImage(file);
        }
    };

    const handleSavePlatformUrl = async (e: React.FormEvent) => {
        try {
            e.preventDefault();

            const data = {
                id: item.id,
                platformUrl: newPlatformUrl
            }

            const response = await sendRequest(
                `${serverRoute}/admin/music-platforms`,
                "PATCH",
                JSON.stringify(data),
                {"Content-Type": "application/json"}
            )

            if (response.status === 204) {
                alert("Посилання на музичну платформу успішно оновлено!");
                window.location.reload();
            } else {
                alert("Сталася помилка при оновленні посилання на музичну платформу!")
            }
        } catch (error) {
            console.error("Помилка при оновленні музичної платформи:", error);
        }
        finally {
            OnHideModal();
        }
    };

    const handleSavePlatformPhoto = async (e: React.FormEvent) => {
        try {
            e.preventDefault();

            const data = new FormData();
            data.append("id", item.id.toString());
            data.append("photo", bgImage!);

            const response = await sendRequest(
                `${serverRoute}/admin/music-platforms/photo`,
                "PATCH",
                data
            )

            if (response.status === 204) {
                alert("Фото музичної платформи успішно оновлено!");
                window.location.reload();
            } else {
                alert("Сталася помилка при оновленні фото музичної платформи!")
            }

            OnHideModal();
        } catch (error) {
            console.error("Помилка при оновленні музичної платформи:", error);
        }
    };


    return (
        <>
            <Container className="form-modal form-modal__container">
                <h2 className="form-modal__title">Редагувати музичну платформу</h2>
                <Form className="form-modal__form"
                      onSubmit={handleSavePlatformUrl}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Посилання на платформу:</Form.Label>
                        <Form.Control
                            type="url"
                            name="platformUrl"
                            value={newPlatformUrl || ""}
                            onChange={(e) => setNewPlatformUrl(e.target.value)}
                            pattern="https?://.*"
                            className="form-modal__input"
                            required
                        />
                    </Form.Group>
                    <Button type="submit"
                            disabled={!isTextEdited()}
                            className="form-modal__button form-modal__button--confirm"
                    >
                        Зберегти
                    </Button>
                </Form>
            </Container>

            <Container className="form-modal form-modal__container">
                <Form className="form-modal__form" onSubmit={handleSavePlatformPhoto}>
                    <Form.Group className="form-modal__group">
                        <Form.Label className="form-modal__label">Зображення музичної платформи:</Form.Label>
                        <Form.Control
                            type="file"
                            onChange={handleChangePhoto}
                            accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp"
                            className="form-modal__input"
                            required
                        />
                    </Form.Group>
                    <Button
                        type="submit"
                        disabled={!bgImage}
                        className="form-modal__button form-modal__button--confirm"
                    >
                        Зберегти
                    </Button>
                    <Button
                        className="form-modal__button form-modal__button--cancel"
                        onClick={OnHideModal}
                    >
                        Скасувати
                    </Button>
                </Form>
            </Container>
        </>
    );

}

export {EditMusicPlatformModal};

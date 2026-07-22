import React, {FC, useContext, useState, useEffect} from "react";
import {Button, Container, Form} from "react-bootstrap";
import {GlobalContext} from "../../../contexts/shared/GlobalContext.tsx";
import {IVideoModel, VideoCategory, videoCategories} from "../../../../models/video/IVideoModel.ts";
import {ModalLoading} from "../../shared/ModalLoading.tsx";

interface VideoEditButtonProps {
    video: IVideoModel;
}

const EditVideoModal: FC<VideoEditButtonProps> = ({video}) => {
    const globalContext = useContext(GlobalContext)!;
    const [editedVideo, setEditedVideo] = useState<IVideoModel>(video);

    const {sendRequest, OnHideModal, modalLoading, setModalLoading, serverRoute} = globalContext;

    useEffect(() => {
        setEditedVideo(video);
    }, [video]);

    const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEditedVideo({
            ...editedVideo,
            title: e.target.value,
        });
    };

    const handleTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setEditedVideo({
            ...editedVideo,
            type: e.target.value as VideoCategory,
        });
    };

    const isFormValid = editedVideo.title.trim() !== "" && editedVideo.type !== "Promotion";

    const handleSave = async (event: React.FormEvent) => {
        event.preventDefault();
        if (!isFormValid) return;

        setModalLoading(true);
        try {
            const data = {
                id: editedVideo.id,
                title: editedVideo.title.trim(),
                type: editedVideo.type
            };

            const response = await sendRequest(
                `${serverRoute}/admin/videos`,
                "PATCH",
                data,
                {"Content-Type": "application/json"}
            );

            if (response.status === 204) {
                OnHideModal();
                alert("Збережено!");
                window.location.reload();
            } else {
                alert("Помилка при збереженні відео");
            }
        } catch (error) {
            console.error("Помилка при збереженні відео:", error);
            alert("Помилка при збереженні відео");
        } finally {
            setModalLoading(false);
        }
    };

    if (modalLoading) return <ModalLoading/>;

    return (
        <Container className="form-modal">
            <h2 className="form-modal__title">Редагувати відео</h2>
            <Form className="form-modal__form"
                  onSubmit={handleSave}
            >
                <Form.Group controlId="edit-video-title" className="form-modal__group">
                    <Form.Label className="form-modal__label">Заголовок:</Form.Label>
                    <Form.Control
                        type="text"
                        name="title"
                        value={editedVideo.title || ""}
                        onChange={handleTitleChange}
                        className="form-modal__input"
                        placeholder="Введіть назву відео"
                        required
                    />
                </Form.Group>
                <Form.Group controlId="edit-video-type" className="form-modal__group">
                    <Form.Label className="form-modal__label">Змінити тип відео:</Form.Label>
                    <Form.Select
                        name="type"
                        value={editedVideo.type || ""}
                        onChange={handleTypeChange}
                        className="form-modal__select"
                    >
                        {videoCategories.map(category => (
                            <option key={category} value={category}>{category}</option>
                        ))}
                    </Form.Select>
                </Form.Group>
                <Button
                    className="form-modal__button form-modal__button--confirm"
                    type='submit'
                    disabled={!isFormValid}
                >
                    Зберегти
                </Button>
                <Button className="form-modal__button form-modal__button--cancel" type="button" onClick={OnHideModal}>
                    Закрити
                </Button>
            </Form>
        </Container>
    );
};

export {EditVideoModal};
